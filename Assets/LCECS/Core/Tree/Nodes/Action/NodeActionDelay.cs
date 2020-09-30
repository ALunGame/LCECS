using LCECS.Core.Tree.Base;

namespace LCECS.Core.Tree.Nodes.Action
{
    /// <summary>
    /// 延时行为            （延时指定时间后，退出）
    /// </summary>
    [Node(ViewName = "延时行为",IsCommonAction =true)]
    public class NodeActionDelay:NodeAction
    {
        [NodeValue(ViewEditor = true)]
        public float WaitTime = 0;

        private float Timer;

        protected override void OnEnter(NodeData wData)
        {
            Timer = NodeTime.TotalTime;
        }

        protected override int OnRunning(NodeData wData)
        {
            if (NodeTime.TotalTime - Timer > WaitTime)
                return NodeState.FINISHED;
            return NodeState.EXECUTING;
        }

        protected override void OnExit(NodeData wData, int runningStatus)
        {
            Timer = 0;
        }
    }
}
