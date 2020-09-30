using LCECS.Core.Tree.Base;
using LCECS.Data;
using LCECS.Help;
using LCECS.Layer.Behavior;
using LCECS.Layer.Decision;
using LCECS.Layer.Info;
using LCECS.Layer.Request;
using System;
using System.Collections.Generic;

namespace LCECS.Server.Config
{
    public class ConfigServer : IConfigServer
    {
        private EntityJsonList entityJsons = null;
        private ReqWeightJson  reqWeightConf;

        public void SetDecTrees(DecTrees trees)
        {
            InitDecLayer(trees);
        }

        public void SetBevTrees(BevTrees trees)
        {
            InitBevLayer(trees);
        }

        public void SetEntityConf(EntityJsonList entityConf)
        {
            this.entityJsons = entityConf;
        }

        public void SetReqWeightConf(ReqWeightJson reqWeightConf)
        {
            this.reqWeightConf = reqWeightConf;
            InitInfoLayer();
            InitReqLayer();
        }

        public EntityJson GetEntityData(string entityName)
        {
            if (entityJsons == null)
                return null;
            for (int i = 0; i < entityJsons.List.Count; i++)
            {
                EntityJson json = entityJsons.List[i];
                if (json.EntityName==entityName)
                {
                    return json;
                }
            }
            return null;
        }

        public int GetRequestWeight(int reqId, bool isEntity)
        {
            if (reqId<=0)
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
                if (json.Key==(int)reqId)
                {
                    return json.Weight;
                }
            }
            ECSLocate.ECSLog.LogR("有实体请求没有设置权重>>>>",reqId.ToString());
            return 0;
        }

        private int GetWorldRequestWeight(WorldReqId reqId)
        {
            if (reqWeightConf == null)
                return 0;
            for (int i = 0; i < reqWeightConf.WorldReqWeight.Count; i++)
            {
                WeightJson json = reqWeightConf.WorldReqWeight[i];
                if (json.Key==(int)reqId)
                {
                    return json.Weight;
                }
            }
            ECSLocate.ECSLog.LogR("有世界请求没有设置权重>>>>",reqId.ToString());
            return 0;
        }

        #region 信息层

        private void InitInfoLayer()
        {
            List<Type> worldSensorTypes = ReflectHelp.GetInterfaceByType<IWorldSensor>();
            List<Type> entitySensorTypes = ReflectHelp.GetInterfaceByType<IEntitySensor>();
            if (worldSensorTypes == null && entitySensorTypes==null)
                return;

            //世界信息
            foreach (Type type in worldSensorTypes)
            {
                WorldSensorAttribute attr = ReflectHelp.GetTypeAttr<WorldSensorAttribute>(type);
                if(attr==null)
                {
                    ECSLocate.ECSLog.LogR("有世界信息没有加入特性 >>>>>>", type.Name);
                    return;
                }

                IWorldSensor sensor = ReflectHelp.CreateInstanceByType<IWorldSensor>(type);
                ECSLayerLocate.Info.RegWorldSensor(attr.InfoKey, sensor);
            }

            //实体信息
            foreach (Type type in entitySensorTypes)
            {
                EntitySensorAttribute attr = ReflectHelp.GetTypeAttr<EntitySensorAttribute>(type);
                if (attr == null)
                {
                    ECSLocate.ECSLog.LogR("有实体信息没有加入特性 >>>>>>", type.Name);
                    return;
                }

                IEntitySensor sensor = ReflectHelp.CreateInstanceByType<IEntitySensor>(type);
                ECSLayerLocate.Info.RegEntitySensor(attr.InfoKey, sensor);
            }
        }

        #endregion

        #region 行为层

        private void InitBevLayer(BevTrees bevTrees)
        {
            if (bevTrees == null)
                return;

            CreateBev(bevTrees);
        }

        //创建行为
        private void CreateBev(BevTrees BevTrees)
        {
            //实体
            foreach (string key in BevTrees.EntityTrees.Keys)
            {
                EntityReqId reqId = (EntityReqId)Enum.Parse(typeof(EntityReqId), key);

                //创建树
                NodeDataJson nodeJson = BevTrees.EntityTrees[key];
                Node rootNode = CreateNodeInstance(nodeJson);
                CreateNodeRelation(rootNode, nodeJson.ChildNodes);

                //TODO
                //扩展
                //判断是否需要其他实例处理
                BaseEntityBehavior request = new BaseEntityBehavior(rootNode);
                ECSLayerLocate.Behavior.RegEntityBev(reqId, request);
            }

            //世界
            foreach (string key in BevTrees.WorldTrees.Keys)
            {
                WorldReqId reqId = (WorldReqId)Enum.Parse(typeof(WorldReqId), key);

                //创建树
                NodeDataJson nodeJson = BevTrees.WorldTrees[key];
                Node rootNode = CreateNodeInstance(nodeJson);
                CreateNodeRelation(rootNode, nodeJson.ChildNodes);

                //TODO
                //扩展
                //判断是否需要其他实例处理
                BaseWorldBehavior request = new BaseWorldBehavior(rootNode);
                ECSLayerLocate.Behavior.RegWorldBev(reqId, request);
            }
        }


        #endregion

        #region 决策层

        private void InitDecLayer(DecTrees decTrees)
        {
            if (decTrees == null)
                return;

            CreateDec(decTrees);
        }

        //创建决策
        private void CreateDec(DecTrees DecTrees)
        {
            //实体
            foreach (string key in DecTrees.EntityTrees.Keys)
            {
                EntityDecGroup group = (EntityDecGroup)Enum.Parse(typeof(EntityDecGroup), key);

                //创建树
                NodeDataJson nodeJson = DecTrees.EntityTrees[key];
                Node rootNode = CreateNodeInstance(nodeJson);
                CreateNodeRelation(rootNode, nodeJson.ChildNodes);

                //TODO
                //扩展
                //判断是否需要其他实例处理
                BaseEntityDecision decision = new BaseEntityDecision(rootNode); ;
                ECSLayerLocate.Decision.RegEntityDecision(group, decision);
            }

            //世界
            foreach (string key in DecTrees.WorldTrees.Keys)
            {
                WorldDecGroup group = (WorldDecGroup)Enum.Parse(typeof(WorldDecGroup), key);

                //创建树
                NodeDataJson nodeJson = DecTrees.WorldTrees[key];
                Node rootNode = CreateNodeInstance(nodeJson);
                CreateNodeRelation(rootNode, nodeJson.ChildNodes);

                //TODO
                //扩展
                //判断是否需要其他实例处理
                BaseWorldDecision decision = new BaseWorldDecision(rootNode); ;
                ECSLayerLocate.Decision.RegWorldDecision(group, decision);
            }
        }

        #endregion

        #region 请求层

        private void InitReqLayer()
        {
            List<Type> worldRequestTypes = ReflectHelp.GetInterfaceByType<IWorldRequest>();
            List<Type> entityRequestTypes = ReflectHelp.GetInterfaceByType<IEntityRequest>();
            if (worldRequestTypes == null && entityRequestTypes == null)
                return;

            //世界请求
            foreach (Type type in worldRequestTypes)
            {
                WorldRequestAttribute attr = ReflectHelp.GetTypeAttr<WorldRequestAttribute>(type);
                if (attr == null)
                {
                    ECSLocate.ECSLog.Log("有请求没有加特性 走权重 >>>>>>", type.Name);
                    return;
                }

                IWorldRequest request = ReflectHelp.CreateInstanceByType<IWorldRequest>(type);
                ECSLayerLocate.Request.RegWorldRequest(attr.ReqId, request);
            }

            //实体请求
            foreach (Type type in entityRequestTypes)
            {
                EntityRequestAttribute attr = ReflectHelp.GetTypeAttr<EntityRequestAttribute>(type);
                if (attr == null)
                {
                    ECSLocate.ECSLog.Log("有请求没有加特性 走权重 >>>>>>", type.Name);
                    return;
                }

                IEntityRequest request = ReflectHelp.CreateInstanceByType<IEntityRequest>(type);
                ECSLayerLocate.Request.RegEntityRequest(attr.ReqId, request);
            }
        }
        
        #endregion

        private void CreateNodeRelation(Node parNode, List<NodeDataJson> childNodes)
        {
            if (parNode == null)
                return;

            if (childNodes == null)
                return;

            for (int i = 0; i < childNodes.Count; i++)
            {
                NodeDataJson childAsset = childNodes[i];
                Node childNode = CreateNodeInstance(childAsset);

                //递归
                CreateNodeRelation(childNode, childAsset.ChildNodes);

                if (childNode != null)
                    parNode.AddChild(childNode);
            }
        }

        private Node CreateNodeInstance(NodeDataJson node)
        {
            if (string.IsNullOrEmpty(node.TypeFullName))
                return null;

            Node rootNode = ReflectHelp.CreateInstanceByType<Node>(node.TypeFullName);
            rootNode.Init(node.NodeId, node.Type, node.ChildMaxCnt);
            if (node.Premise != null)
            {
                NodePremise premise = ReflectHelp.CreateInstanceByType<NodePremise>(node.Premise.TypeFullName);
                premise.Init(rootNode.GetHashCode(),node.Premise.Type, node.Premise.TrueValue);

                if (node.Premise.OtherPremise!=null)
                {
                    CreateNodePremise(rootNode.GetHashCode(), premise, node.Premise.OtherPremise);
                }

                rootNode.SetPremise(premise);
            }
            
            //属性设置
            for (int i = 0; i < node.KeyValues.Count; i++)
            {

                NodeKeyValue keyValue = node.KeyValues[i];
                object value = ReflectHelp.StrChangeToObject(keyValue.Value, keyValue.TypeFullName);
                ReflectHelp.SetTypeFieldValue(rootNode, keyValue.KeyName, value);
            }
            return rootNode;
        }

        private void CreateNodePremise(int nodeId,NodePremise premise,NodePremiseJson premiseJson)
        {
            if (premiseJson == null)
                return;
            NodePremise otherPremise = ReflectHelp.CreateInstanceByType<NodePremise>(premiseJson.TypeFullName);
            otherPremise.Init(nodeId, premiseJson.Type);
            premise.AddOtherPrecondition(otherPremise);

            if (premiseJson.OtherPremise != null)
                CreateNodePremise(nodeId, otherPremise, premiseJson.OtherPremise);
        }
    }
}
