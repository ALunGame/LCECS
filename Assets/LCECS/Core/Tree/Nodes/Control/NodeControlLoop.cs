using LCECS.Core.Tree.Base;

namespace LCECS.Core.Tree.Nodes.Control
{
    /// <summary>
    /// 循环控制节点          （循环调用一个节点）
    /// </summary>
    [Node(ViewName = "循环节点",MaxChildCnt = 1)]
    public sealed class NodeControlLoop: NodeControl
    {
        class NodeControlLoopContext : NodeContext
        {
            //循环次数
            internal int currentCount;

            public NodeControlLoopContext()
            {
                currentCount = 0;
            }
        }

        //设定循环次数
        public int loopCount=-1;

        //评估
        protected override bool OnEvaluate(NodeData wData)
        {
            NodeControlLoopContext thisContext = GetContext<NodeControlLoopContext>(wData);
            bool checkLoopCount = (loopCount == -1 || thisContext.currentCount < loopCount);
            //循环次数
            if (checkLoopCount==false)
                return false;
            if (IsIndexValid(0))
            {
                Node node = GetChild<Node>(0);
                return node.Evaluate(wData);
            }
            return base.OnEvaluate(wData);
        }

        //执行
        protected override int OnExcute(NodeData wData)
        {
            NodeControlLoopContext thisContext = GetContext<NodeControlLoopContext>(wData);
            int runningStatus = NodeState.FINISHED;

            if (IsIndexValid(0))
            {
                Node node = GetChild<Node>(0);
                runningStatus = node.Execute(wData);
                if (NodeState.IsFinished(runningStatus))
                {
                    thisContext.currentCount++;
                    if (thisContext.currentCount < loopCount || loopCount == -1)
                    {
                        runningStatus = NodeState.EXECUTING;
                    }
                }
            }
            return runningStatus;
        }

        //转换
        protected override void OnTransition(NodeData wData)
        {
            NodeControlLoopContext thisContext = GetContext<NodeControlLoopContext>(wData);
            if (IsIndexValid(0))
            {
                Node node = GetChild<Node>(0);
                node.Transition(wData);
            }
        }
    }
}
