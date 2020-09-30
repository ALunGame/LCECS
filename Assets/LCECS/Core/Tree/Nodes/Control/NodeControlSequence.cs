using LCECS.Core.Tree.Base;

namespace LCECS.Core.Tree.Nodes.Control
{
    /// <summary>
    /// 控制序列节点  (执行一个后继续下一个）
    /// </summary>
    [Node(ViewName = "序列节点")]
    public class NodeControlSequence : NodeControl
    {
        class NodeControlSequenceContext : NodeContext
        {
            internal int currentSelectedIndex;
            public NodeControlSequenceContext()
            {
                currentSelectedIndex = -1;
            }
        }

        protected override bool OnEvaluate(NodeData wData)
        {
            NodeControlSequenceContext thisContext = GetContext<NodeControlSequenceContext>(wData);
            //获取当前选择的索引
            int checkedNodeIndex = -1;
            if (IsIndexValid(thisContext.currentSelectedIndex))
            {
                checkedNodeIndex = thisContext.currentSelectedIndex;
            }
            else
            {
                checkedNodeIndex = 0;
            }
            if (IsIndexValid(checkedNodeIndex))
            {
                Node node = GetChild<Node>(checkedNodeIndex);
                if (node.Evaluate(wData))
                {
                    thisContext.currentSelectedIndex = checkedNodeIndex;
                    return true;
                }
            }
            return false;
        }

        protected override int OnExcute(NodeData wData)
        {
            NodeControlSequenceContext thisContext = GetContext<NodeControlSequenceContext>(wData);
            int runningStatus = NodeState.FINISHED;

            //执行一下选择节点
            Node node = GetChild<Node>(thisContext.currentSelectedIndex);
            runningStatus = node.Execute(wData);

            //错误判断
            if (NodeState.IsError(runningStatus))
            {
                thisContext.currentSelectedIndex = -1;
                return runningStatus;
            }

            //完成判断
            if (NodeState.IsFinished(runningStatus))
            {
                //下移索引
                thisContext.currentSelectedIndex++;
                if (IsIndexValid(thisContext.currentSelectedIndex))
                {
                    runningStatus = NodeState.EXECUTING;
                }
                else
                {
                    thisContext.currentSelectedIndex = -1;
                }
            }
            return runningStatus;
        }

        protected override void OnTransition(NodeData wData)
        {
            NodeControlSequenceContext thisContext = GetContext<NodeControlSequenceContext>(wData);
            Node node = GetChild<Node>(thisContext.currentSelectedIndex);
            if (node != null)
            {
                node.Transition(wData);
            }
            thisContext.currentSelectedIndex = -1;
        }
    }
}
