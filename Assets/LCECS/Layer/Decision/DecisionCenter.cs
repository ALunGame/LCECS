using LCECS.Core.ECS;
using LCECS.Core.Tree.Base;
using LCECS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LCECS.Layer.Decision
{
    /// <summary>
    /// 决策中心
    /// 1，将游戏中各个实体分组决策
    /// 2，更新每个组决策结果
    /// </summary>
    public class DecisionCenter
    {
        private Dictionary<int, BaseEntityDecision> EntityDesDict = new Dictionary<int, BaseEntityDecision>();
        private Dictionary<int, BaseWorldDecision> WorldDesDict = new Dictionary<int, BaseWorldDecision>();

        public void Init()
        {
            TextAsset jsonData = ECSLocate.Factory.GetProduct<TextAsset>(FactoryType.Asset, null, ECSDefinitionPath.DecTreePath);
            DecTrees decTrees = LitJson.JsonMapper.ToObject<DecTrees>(jsonData.text);
            SetDecTrees(decTrees);
        }

        //设置决策树
        private void SetDecTrees(DecTrees trees)
        {
            if (trees == null)
                return;
            CreateDec(trees);
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
                Node rootNode = Node.CreateNodeInstance(nodeJson);
                Node.CreateNodeRelation(rootNode, nodeJson.ChildNodes);

                BaseEntityDecision decision = new BaseEntityDecision(rootNode); ;
                EntityDesDict.Add((int)group, decision);
            }

            //世界
            foreach (string key in DecTrees.WorldTrees.Keys)
            {
                WorldDecGroup group = (WorldDecGroup)Enum.Parse(typeof(WorldDecGroup), key);

                //创建树
                NodeDataJson nodeJson = DecTrees.WorldTrees[key];
                Node rootNode = Node.CreateNodeInstance(nodeJson);
                Node.CreateNodeRelation(rootNode, nodeJson.ChildNodes);

                BaseWorldDecision decision = new BaseWorldDecision(rootNode);
                WorldDesDict.Add((int)group, decision);
            }
        }

        //添加实体决策器
        public void RegEntityDecision(int decId, BaseEntityDecision decision)
        {
            if (EntityDesDict.ContainsKey(decId))
            {
                return;
            }
            EntityDesDict.Add(decId, decision);
        }

        /// <summary>
        /// 添加世界决策器
        /// </summary>
        public void RegWorldDecision(int decId, BaseWorldDecision decision)
        {
            if (WorldDesDict.ContainsKey(decId))
            {
                return;
            }
            WorldDesDict.Add(decId, decision);
        }

        /// <summary>
        /// 添加决策实体
        /// </summary>
        public void AddDecisionEntity(int decId, EntityWorkData workData)
        {
            if (!EntityDesDict.ContainsKey(decId))
            {
                ECSLocate.ECSLog.LogR("添加决策实体错误，没有对应决策树>>>>>>>",decId);
                return;
            }
            BaseEntityDecision decision = EntityDesDict[decId];
            decision.AddEntity(workData);
        }

        /// <summary>
        /// 添加决策世界
        /// </summary>
        public void AddDecisionWorld(int decId, WorldWorkData workData)
        {
            if (!WorldDesDict.ContainsKey(decId))
            {
                return;
            }
            BaseWorldDecision decision = WorldDesDict[decId];
            decision.AddWorld(workData);
        }

        /// <summary>
        /// 获得决策实体
        /// </summary>
        public EntityWorkData GetEntityWorkData(int entityId)
        {
            Entity entity = ECSLocate.ECS.GetEntity(entityId);
            EntityDecGroup group = entity.GetEntityDecGroup();
            if (!EntityDesDict.ContainsKey((int)group))
            {
                return null;
            }

            BaseEntityDecision decision = EntityDesDict[(int)group];
            return decision.GetEntity(entityId);
        }

        /// <summary>
        /// 获得决策世界
        /// </summary>
        public WorldWorkData GetWorldWorkData(int decId, int worldId)
        {
            if (!WorldDesDict.ContainsKey(decId))
            {
                return null;
            }
            BaseWorldDecision decision = WorldDesDict[decId];
            return decision.GetWorld(worldId);
        }

        /// <summary>
        /// 删除决策实体
        /// </summary>
        public void RemoveDecisionEntity(int decId, int entityId)
        {
            if (!EntityDesDict.ContainsKey(decId))
            {
                return;
            }
            BaseEntityDecision decision = EntityDesDict[decId];
            decision.RemoveEntity(entityId);
        }

        /// <summary>
        /// 删除决策世界
        /// </summary>
        public void RemoveDecisionWorld(int decId, int worldId)
        {
            if (!WorldDesDict.ContainsKey(decId))
            {
                return;
            }
            BaseWorldDecision decision = WorldDesDict[decId];
            decision.RemoveWorld(worldId);
        }

        /// <summary>
        /// 执行实体决策
        /// </summary>
        public void ExecuteEntityDecision()
        {
            foreach (BaseEntityDecision item in EntityDesDict.Values)
            {
                if (item!=null)
                {
                    item.Execute();
                }
            }
        }

        /// <summary>
        /// 执行世界决策
        /// </summary>
        public void ExecuteWorldDecision()
        {
            foreach (BaseWorldDecision item in WorldDesDict.Values)
            {
                if (item != null)
                {
                    item.Execute();
                }
            }
        }
    }
}
