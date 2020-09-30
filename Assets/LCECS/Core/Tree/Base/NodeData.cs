
using System.Collections.Generic;

namespace LCECS.Core.Tree.Base
{
    /// <summary>
    /// 节点数据
    /// </summary>
    public class NodeData : AnyData
    {
        public int Id = 0;
        public Dictionary<int, NodeContext> Context;

        public NodeData(int id)
        {
            Id = id;
            Context = new Dictionary<int, NodeContext>();
        }

        ~NodeData()
        {
            Context = null;
        }
    }

    public class AnyData
    {
        public T As<T>() where T : AnyData
        {
            return (T)this;
        }
    } 
}
