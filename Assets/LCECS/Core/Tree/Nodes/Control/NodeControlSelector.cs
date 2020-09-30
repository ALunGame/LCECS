using LCECS.Core.Tree.Base;

namespace LCECS.Core.Tree.Nodes.Control
{
    /// <summary>
    /// 控制选择节点    （有一个子节点可运行，就直接运行）
    /// </summary>
    [Node(ViewName = "选择节点")]
    public class NodeControlSelector: NodeControl
    {
        class NodeControlSelectorContext : NodeContext
        {
            internal int currentSelectedIndex;
            internal int lastSelectedIndex;

            public NodeControlSelectorContext()
            {
                currentSelectedIndex = -1;
                lastSelectedIndex = -1;
            }
        }

        protected override bool OnEvaluate(NodeData wData)
        {
            NodeControlSelectorContext thisContext = GetContext<NodeControlSelectorContext>(wData);
            thisContext.currentSelectedIndex = -1;

            //寻找可以运行的
            int childCount = GetChildCount();
            for (int i = 0; i < childCount; ++i)
            {
                Node node = GetChild<Node>(i);
                if (node.Evaluate(wData))
                {
                    thisContext.currentSelectedIndex = i;
                    return true;
                }
            }
            return false;
        }

        protected override int OnExcute(NodeData wData)
        {
            NodeControlSelectorContext thisContext = GetContext<NodeControlSelectorContext>(wData);
            int runningState = NodeState.FINISHED;

            //当前选择的不是上次选择的 （执行下上次的切换方法）
            if (thisContext.currentSelectedIndex != thisContext.lastSelectedIndex)
            {
                if (IsIndexValid(thisContext.lastSelectedIndex))
                {
                    Node node = GetChild<Node>(thisContext.lastSelectedIndex);
                    node.Transition(wData);
                }
                thisContext.lastSelectedIndex = thisContext.currentSelectedIndex;
            }

            //执行下选择的子节点
            if (IsIndexValid(thisContext.lastSelectedIndex))
            {
                Node node = GetChild<Node>(thisContext.lastSelectedIndex);
                runningState = node.Execute(wData);
                if (NodeState.IsFinished(runningState))
                {
                    thisContext.lastSelectedIndex = -1;
                }
            }
            return runningState;
        }

        protected override void OnTransition(NodeData wData)
        {
            NodeControlSelectorContext thisContext = GetContext<NodeControlSelectorContext>(wData);
            Node node = GetChild<Node>(thisContext.lastSelectedIndex);
            if (node != null)
            {
                node.Transition(wData);
            }
            thisContext.lastSelectedIndex = -1;
        }

    }
}
