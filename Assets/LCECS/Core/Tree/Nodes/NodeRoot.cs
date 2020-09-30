using LCECS.Core.Tree.Base;
using LCECS.Help;

namespace LCECS.Core.Tree.Nodes
{
    /// <summary>
    /// 根节点             (只能放一个子节点)
    /// </summary>
    [Node(ViewName = "根节点",MaxChildCnt = 1)]
    public class NodeRoot:Node
    {
        protected override bool OnEvaluate(NodeData wData)
        {
            if (IsIndexValid(0))
            {
                Node node = GetChild<Node>(0);
                return node.Evaluate(wData);
            }
            return false;
        }

        protected override int OnExcute(NodeData wData)
        {
#if UNITY_EDITOR
            NodeRunSelEntityHelp.SetNeedRefresh(wData.Id);
#endif
            if (IsIndexValid(0))
            {
                Node node = GetChild<Node>(0);
                return node.Execute(wData);
            }
            return base.OnExcute(wData);
        }

        protected override void OnTransition(NodeData wData)
        {
            if (IsIndexValid(0))
            {
                Node node = GetChild<Node>(0);
                node.Transition(wData);
            }
        }
    } 
}
