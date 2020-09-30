using LCECS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LCECS.Layer.Request
{
    /// <summary>
    /// 请求中心
    /// </summary>
    public class RequestCenter
    {
        #region 注册请求

        private Dictionary<int, IWorldRequest> WorldRequestDict = new Dictionary<int, IWorldRequest>();
        private Dictionary<int, IEntityRequest> EntityRequestDict = new Dictionary<int, IEntityRequest>();

        public void RegWorldRequest(int key, IWorldRequest request)
        {
            if (WorldRequestDict.ContainsKey(key))
                return;
            WorldRequestDict.Add(key, request);
        }

        public void RegEntityRequest(int key, IEntityRequest request)
        {
            if (EntityRequestDict.ContainsKey(key))
                return;
            EntityRequestDict.Add(key, request);
        }

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
        private bool CheckCanSwitch(int pushId, ref int nextId, ref int curId,ref int clearId,bool isEntity)
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
                curId   = pushId;
                nextId  = 0;
                return true;
            }

            int pushWeight = ECSLocate.Config.GetRequestWeight(pushId, isEntity);
            int nextWeight = ECSLocate.Config.GetRequestWeight(nextId, isEntity);
            int curWeight =  ECSLocate.Config.GetRequestWeight(curId, isEntity);

            //强制置换
            if (pushWeight == ECSDefinition.REForceSwithWeight)
            {
                clearId = curId;
                curId   = pushId;
                nextId  = 0;
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
                curId   = pushId;
                return true;
            }
            return false;
        }

        #endregion

        #region 执行请求

        /// <summary>
        /// 放入实体请求
        /// </summary>
        public void PushEntityRequest(int entityId,int reqId)
        {
            //数据
            EntityWorkData workData = ECSLayerLocate.Info.GetEntityWorkData(entityId);
            if (workData==null)
            {
                return;
            }

            //请求
            IEntityRequest pushRequest = GetEntityRequest(reqId);

            //请求不需要处理
            if (pushRequest==null)
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
                workData.CurrReqId  = selfSwId;

                //执行请求
                ECSLayerLocate.Behavior.PushEntityBev(workData);
                return;
            }

            //权重置换规则
            bool bSw = CheckCanSwitch(reqId, ref workData.NextReqId, ref workData.CurrReqId, ref workData.ClearReqId,true);
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
            bool bSw = CheckCanSwitch(reqId, ref workData.NextReqId, ref workData.CurrReqId, ref workData.ClearReqId,false);
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
