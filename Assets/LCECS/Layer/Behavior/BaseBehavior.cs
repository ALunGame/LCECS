using LCECS.Core.Tree;
using LCECS.Core.Tree.Base;
using LCECS.Core.Tree.Nodes.Action;
using LCECS.Data;
using System;
using System.Collections.Generic;

namespace LCECS.Layer.Behavior
{

    /// <summary>
    /// 实体行为
    /// </summary>
    public class BaseEntityBehavior
    {
        //行为树
        private Node tree;

        public Node Tree
        {
            get
            {
                return tree;
            }
        }

        //需要处理的工作数据
        private List<EntityWorkData> HandleList = new List<EntityWorkData>();

        public BaseEntityBehavior(Node tree)
        {
            this.tree = tree;
        }

        /// <summary>
        /// 添加工作（现在不允许重复添加）
        /// </summary>
        public void AddWorkData(EntityWorkData workData)
        {
            if (HandleList.Contains(workData))
            {
                return;
            }
            HandleList.Add(workData);
        }

        /// <summary>
        /// 删除工作（在新的行为过来前，删除）
        /// </summary>
        public void RemoveWorkData(EntityWorkData workData)
        {
            if (!HandleList.Contains(workData))
            {
                return;
            }

            //清理
            Tree.Transition(workData);
            HandleList.Remove(workData);
        }

        /// <summary>
        /// 执行行为
        /// </summary>
        public void Execute()
        {
            if (tree==null)
            {
                return;
            }
            for (int i = 0; i < HandleList.Count; i++)
            {
                EntityWorkData data = HandleList[i];
                if (tree.Evaluate(data))
                {
                    int treeState = tree.Execute(data);
                    //节点运行完成
                    //工作结束（删除数据）
                    if (treeState== NodeState.FINISHED)
                    {
                        HandleList.RemoveAt(i);
                    }
                }
                else
                {
                    tree.Transition(data);
                }
            }
        }
    }

    /// <summary>
    /// 世界行为
    /// </summary>
    public class BaseWorldBehavior
    {
        //行为树
        private Node tree;

        public Node Tree
        {
            get
            {
                return tree;
            }
        }

        //需要处理的工作数据
        private List<WorldWorkData> HandleList = new List<WorldWorkData>();

        public BaseWorldBehavior(Node tree)
        {
            this.tree = tree;
        }

        /// <summary>
        /// 删除工作（在新的行为过来前，删除）
        /// </summary>
        public void RemoveWorkData(WorldWorkData workData)
        {
            if (!HandleList.Contains(workData))
            {
                return;
            }
            //清理
            Tree.Transition(workData);
            HandleList.Remove(workData);
        }

        /// <summary>
        /// 添加工作（现在不允许重复添加）
        /// </summary>
        public void AddWorkData(WorldWorkData workData)
        {
            if (HandleList.Contains(workData))
            {
                return;
            }
            HandleList.Add(workData);
        }

        /// <summary>
        /// 执行行为
        /// </summary>
        public void Execute()
        {
            if (tree == null)
            {
                return;
            }
            for (int i = 0; i < HandleList.Count; i++)
            {
                WorldWorkData data = HandleList[i];
                if (tree.Evaluate(data))
                {
                    int treeState = tree.Execute(data);
                    //节点运行完成
                    //工作结束（删除数据）
                    if (treeState == NodeState.FINISHED)
                    {
                        HandleList.RemoveAt(i);
                    }
                }
                else
                {
                    tree.Transition(data);
                }
            }
        }
    }
}
