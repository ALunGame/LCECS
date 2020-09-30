

using System;

namespace LCECS.Core.Tree.Base
{

    /// <summary>
    /// 节点前提特性 类可用
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class NodePremiseAttribute : Attribute
    {
        /// <summary>
        /// 编辑器显示名
        /// </summary>
        public string ViewName { get; set; } = "";

        /// <summary>
        /// 编辑器显示名
        /// </summary>
        public string GroupName { get; set; } = "";

        public NodePremiseAttribute(string name)
        {
            ViewName = name;
        }

        public NodePremiseAttribute(string name, string groupName)
        {
            ViewName = name;
            GroupName = groupName;
        }
    }

    ///// <summary>
    ///// 节点字段特性 字段可用
    ///// </summary>
    //[AttributeUsage(AttributeTargets.Field)]
    //public class NodePremiseValueAttribute : Attribute
    //{
    //    /// <summary>
    //    /// 可以在编辑器模式下编辑
    //    /// </summary>
    //    public bool ViewEditor { get; set; } = false;
    //}

    /// <summary>
    /// 节点前提----------单链表
    /// </summary>
    public class NodePremise
    {
        //节点Id
        protected int nodeKey;

        //前提种类
        protected PremiseType premiseType;

        //什么才是真
        private bool trueValue = true;

        //下一个前提
        protected NodePremise otherPremise;

        //重写哈希值
        public override int GetHashCode()
        {
            return nodeKey;
        }

        //初始化
        public virtual void Init(int nodeKey,PremiseType premiseType= PremiseType.AND,bool trueValue=true)
        {
            this.nodeKey = nodeKey;
            this.premiseType = premiseType;
            this.trueValue = trueValue;
        }

        //添加前提
        public void AddOtherPrecondition(NodePremise premise)
        {
            otherPremise = premise;
        }

        //检测方法
        public bool IsTrue(NodeData wData)
        {
            bool resValue = OnMakeTrue(wData);
            resValue = trueValue == resValue ? true : false;
            if (otherPremise != null)
            {
                switch (premiseType)
                {
                    case PremiseType.AND:
                        return resValue && otherPremise.IsTrue(wData);
                    case PremiseType.OR:
                        return resValue || otherPremise.IsTrue(wData);
                    case PremiseType.XOR:
                        return resValue ^ otherPremise.IsTrue(wData);
                }
                return resValue && otherPremise.IsTrue(wData);
            }
            else
            {
                return resValue;
            }
        }

        //子类重写
        public virtual bool OnMakeTrue(NodeData wData)
        {
            return true;
        }
    }
}
