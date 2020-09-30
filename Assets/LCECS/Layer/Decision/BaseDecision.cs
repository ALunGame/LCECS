using LCECS.Core.Tree.Base;
using LCECS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LCECS.Layer.Decision
{
    /// <summary>
    /// 实体决策
    /// </summary>
    public class BaseEntityDecision
    {
        //决策树
        private Node Tree;
        private List<EntityWorkData> EntityList = new List<EntityWorkData>();

        public BaseEntityDecision(Node tree)
        {
            Tree = tree;
        }

        /// <summary>
        /// 添加决策实体
        /// </summary>
        public void AddEntity(EntityWorkData workData)
        {
            if (EntityList.Contains(workData))
            {
                return;
            }
            EntityList.Add(workData);
        }

        /// <summary>
        /// 获得决策实体
        /// </summary>
        public EntityWorkData GetEntity(int entityId)
        {
            for (int i = 0; i < EntityList.Count; i++)
            {
                EntityWorkData workData = EntityList[i];
                if (workData.MEntity.GetHashCode() == entityId)
                {
                    return workData;
                }
            }
            return null;
        }

        /// <summary>
        /// 删除决策实体
        /// </summary>
        public void RemoveEntity(int entityId)
        {
            for (int i = 0; i < EntityList.Count; i++)
            {
                EntityWorkData workData = EntityList[i];
                if (workData.MEntity.GetHashCode() == entityId)
                {
                    EntityList.RemoveAt(i);
                    return;
                }
            }
        }

        /// <summary>
        /// 执行决策
        /// </summary>
        public virtual void Execute()
        {
            for (int i = 0; i < EntityList.Count; i++)
            {
                EntityWorkData workData = EntityList[i];
                if (Tree.Evaluate(workData))
                {
                    Tree.Execute(workData);
                }
                else
                {
                    Tree.Transition(workData);
                }
            }
        }
    }

    /// <summary>
    /// 世界决策
    /// </summary>
    public class BaseWorldDecision
    {
        //决策树
        private Node Tree;
        private List<WorldWorkData> WorldList = new List<WorldWorkData>();

        public BaseWorldDecision(Node tree)
        {
            Tree = tree;
        }

        /// <summary>
        /// 添加决策实体
        /// </summary>
        public void AddWorld(WorldWorkData workData)
        {
            if (WorldList.Contains(workData))
            {
                return;
            }
            WorldList.Add(workData);
        }

        /// <summary>
        /// 获得决策实体
        /// </summary>
        public WorldWorkData GetWorld(int worldId)
        {
            for (int i = 0; i < WorldList.Count; i++)
            {
                WorldWorkData workData = WorldList[i];
                if (workData.Id == worldId)
                {
                    return workData;
                }
            }
            return null;
        }

        /// <summary>
        /// 添加决策实体
        /// </summary>
        public void RemoveWorld(int worldId)
        {
            for (int i = 0; i < WorldList.Count; i++)
            {
                WorldWorkData workData = WorldList[i];
                if (workData.Id == worldId)
                {
                    WorldList.RemoveAt(i);
                    return;
                }
            }
        }

        /// <summary>
        /// 执行决策
        /// </summary>
        public virtual void Execute()
        {
            for (int i = 0; i < WorldList.Count; i++)
            {
                WorldWorkData workData = WorldList[i];
                if (Tree.Evaluate(workData))
                {
                    Tree.Execute(workData);
                }
                else
                {
                    Tree.Transition(workData);
                }
            }
        }
    }
}
