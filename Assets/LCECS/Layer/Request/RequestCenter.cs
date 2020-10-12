using LCECS.Data;
using LCHelp;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LCECS.Layer.Request
{
    /// <summary>
    /// 请求中心
    /// </summary>
    public class RequestCenter
    {

        #region 初始化

        public void Init()
        {
            TextAsset jsonData = ECSLocate.Factory.GetProduct<TextAsset>(FactoryType.Asset, null, ECSDefinitionPath.LogicReqWeightPath);
            ReqWeightJson reqWeightJson = LitJson.JsonMapper.ToObject<ReqWeightJson>(jsonData.text);
            SetReqWeight(reqWeightJson);
            RegAllRequest();
        }

        private void SetReqWeight(ReqWeightJson json)
        {
            reqWeightConf = json;
        }

        private void RegAllRequest()
        {
            List<Type> worldRequestTypes = LCReflect.GetInterfaceByType<IWorldRequest>();
            List<Type> entityRequestTypes = LCReflect.GetInterfaceByType<IEntityRequest>();
            if (worldRequestTypes == null && entityRequestTypes == null)
                return;

            //世界请求
            foreach (Type type in worldRequestTypes)
            {
                WorldRequestAttribute attr = LCReflect.GetTypeAttr<WorldRequestAttribute>(type);
                if (attr == null)
                {
                    ECSLocate.ECSLog.Log("有请求没有加特性 走权重 >>>>>>", type.Name);
                    return;
                }

                IWorldRequest request = LCReflect.CreateInstanceByType<IWorldRequest>(type.FullName);
                WorldRequestDict.Add((int)attr.ReqId, request);
            }

            //实体请求
            foreach (Type type in entityRequestTypes)
            {
                EntityRequestAttribute attr = LCReflect.GetTypeAttr<EntityRequestAttribute>(type);
                if (attr == null)
                {
                    ECSLocate.ECSLog.Log("有请求没有加特性 走权重 >>>>>>", type.Name);
                    return;
                }

                IEntityRequest request = LCReflect.CreateInstanceByType<IEntityRequest>(type.FullName);
                EntityRequestDict.Add((int)attr.ReqId, request);
            }
        }

        #endregion

        #region 请求权重
        private ReqWeightJson reqWeightConf;
        public int GetRequestWeight(int reqId, bool isEntity)
        {
            if (reqId <= 0)
                return 0;
            if (isEntity)
                return GetEntityRequestWeight((EntityReqId)reqId);
            else
                return GetWorldRequestWeight((WorldReqId)reqId);
        }

        private int GetEntityRequestWeight(EntityReqId reqId)
        {
            if (reqWeightConf == null)
                return 0;
            for (int i = 0; i < reqWeightConf.EntityReqWeight.Count; i++)
            {
                WeightJson json = reqWeightConf.EntityReqWeight[i];
                if (json.Key == (int)reqId)
                {
                    return json.Weight;
                }
            }
            ECSLocate.ECSLog.LogR("有实体请求没有设置权重>>>>", reqId.ToString());
            return 0;
        }

        private int GetWorldRequestWeight(WorldReqId reqId)
        {
            if (reqWeightConf == null)
                return 0;
            for (int i = 0; i < reqWeightConf.WorldReqWeight.Count; i++)
            {
                WeightJson json = reqWeightConf.WorldReqWeight[i];
                if (json.Key == (int)reqId)
                {
                    return json.Weight;
                }
            }
            ECSLocate.ECSLog.LogR("有世界请求没有设置权重>>>>", reqId.ToString());
            return 0;
        }

        #endregion

        #region 注册请求

        private Dictionary<int, IWorldRequest> WorldRequestDict = new Dictionary<int, IWorldRequest>();
        private Dictionary<int, IEntityRequest> EntityRequestDict = new Dictionary<int, IEntityRequest>();

        private IWorldRequest GetWorldRequest(int key)
        {
            if (!WorldRequestDict.ContainsKey(key))
                return null;
            return WorldRequestDict[key];
        }

        private IEntityRequest GetEntityRequest(int key)
        {
            if (!EntityRequestDict.ContainsKey(key))
                return null;
            return EntityRequestDict[key];
        }

        #endregion

        #region 置换规则

        /// <summary>
        /// 置换规则
        /// </summary>
        /// <returns>是否置换成功</returns>
        private bool CheckCanSwitch(int pushId, ref int nextId, ref int curId, ref int clearId, bool isEntity)
        {
            //小于零错误
            if (pushId <= 0)
            {
                return false;
            }

            //没有执行的请求---直接换
            if (curId == 0)
            {
                clearId = nextId;
                curId = pushId;
                nextId = 0;
                return true;
            }

            int pushWeight = GetRequestWeight(pushId, isEntity);
            int nextWeight = GetRequestWeight(nextId, isEntity);
            int curWeight  = GetRequestWeight(curId, isEntity);

            //强制置换
            if (pushWeight == ECSDefinition.REForceSwithWeight)
            {
                clearId = curId;
                curId = pushId;
                nextId = 0;
                return true;
            }

            //判断本来的下一个
            if (pushWeight > nextWeight)
            {
                nextId = pushId;
            }

            //判断当前的
            if (pushWeight > curWeight)
            {
                clearId = curId;
                curId = pushId;
                return true;
            }
            return false;
        }

        #endregion

        #region 执行请求

        /// <summary>
        /// 放入实体请求
        /// </summary>
        public void PushEntityRequest(int entityId, int reqId)
        {
            //数据
            EntityWorkData workData = ECSLayerLocate.Info.GetEntityWorkData(entityId);
            if (workData == null)
            {
                return;
            }

            //请求
            IEntityRequest pushRequest = GetEntityRequest(reqId);

            //请求不需要处理
            if (pushRequest == null)
            {
                //权重置换规则
                bool bTmpSw = CheckCanSwitch(reqId, ref workData.NextReqId, ref workData.CurrReqId, ref workData.ClearReqId, true);
                if (!bTmpSw)
                {
                    return;
                }
                //执行请求
                ECSLayerLocate.Behavior.PushEntityBev(workData);
                return;
            }

            int selfSwId = reqId;
            //请求内部置换
            int rule = pushRequest.SwitchRequest(reqId, ref selfSwId);
            //只需要自身判断
            if (rule == ECSDefinition.RESwithRuleSelf)
            {
                //没有变化
                if (workData.CurrReqId == selfSwId)
                {
                    return;
                }
                workData.ClearReqId = workData.CurrReqId;
                workData.CurrReqId = selfSwId;

                //执行请求
                ECSLayerLocate.Behavior.PushEntityBev(workData);
                return;
            }

            //权重置换规则
            bool bSw = CheckCanSwitch(reqId, ref workData.NextReqId, ref workData.CurrReqId, ref workData.ClearReqId, true);
            if (!bSw)
            {
                return;
            }
            //执行请求
            ECSLayerLocate.Behavior.PushEntityBev(workData);
        }

        /// <summary>
        /// 放入世界请求
        /// </summary>
        public void PushWorldRequest(int worldId, int reqId)
        {
            //数据
            WorldWorkData workData = ECSLayerLocate.Info.GetWorldWorkData(worldId); ;
            //ECSLayerLocate.Decision.GetWorldWorkData(worldId);
            if (workData == null)
            {
                return;
            }

            //请求
            IWorldRequest pushRequest = GetWorldRequest(reqId);
            if (pushRequest == null)
            {
                //权重置换规则
                bool bTmpSw = CheckCanSwitch(reqId, ref workData.NextReqId, ref workData.CurrReqId, ref workData.ClearReqId, true);
                if (!bTmpSw)
                {
                    return;
                }
                //执行请求
                ECSLayerLocate.Behavior.PushWorldBev(workData);
                return;
            }

            int selfSwId = reqId;
            //请求内部置换
            int rule = pushRequest.SwitchRequest(reqId, ref selfSwId);
            //只需要自身判断
            if (rule == ECSDefinition.RESwithRuleSelf)
            {
                //没有变化
                if (workData.CurrReqId == selfSwId)
                {
                    return;
                }
                workData.ClearReqId = workData.CurrReqId;
                workData.CurrReqId = selfSwId;

                //执行请求
                ECSLayerLocate.Behavior.PushWorldBev(workData);
                return;
            }

            //权重置换规则
            bool bSw = CheckCanSwitch(reqId, ref workData.NextReqId, ref workData.CurrReqId, ref workData.ClearReqId, false);
            if (!bSw)
            {
                return;
            }
            //执行请求
            ECSLayerLocate.Behavior.PushWorldBev(workData);
        }

        #endregion

    }
}
