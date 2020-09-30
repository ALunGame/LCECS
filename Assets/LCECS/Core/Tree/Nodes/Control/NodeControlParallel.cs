using LCECS.Core.Tree.Base;
using System.Collections.Generic;

namespace LCECS.Core.Tree.Nodes.Control
{
    /// <summary>
    /// 控制并行节点
    /// </summary>
    [Node(ViewName = "并行节点")]
    public class NodeControlParallel:NodeControl
    {
        class NodeControlParallelContext : NodeContext
        {
            internal List<bool> evaluationStatus;
            internal List<int> runningStatus;

            public NodeControlParallelContext()
            {
                evaluationStatus = new List<bool>();
                runningStatus = new List<int>();
            }
        }

        //评估并行关系
        public NodeParallelType evaluateType;
        //运行并行关系
        public NodeParallelType excuteType;

        protected override bool OnEvaluate(NodeData wData)
        {
            NodeControlParallelContext thisContext = GetContext<NodeControlParallelContext>(wData);
            //默认为False
            InitListTo(thisContext.evaluationStatus, false);

            bool finalResult = false;
            for (int i = 0; i < GetChildCount(); ++i)
            {
                Node node = GetChild<Node>(i);
                bool ret = node.Evaluate(wData);

                //并关系
                if (evaluateType == NodeParallelType.AND && ret == false)
                {
                    finalResult = false;
                    break;
                }
                if (ret == true)
                {
                    finalResult = true;
                }
                //保存状态数据
                thisContext.evaluationStatus[i] = ret;
            }

            return finalResult;
        }

        protected override int OnExcute(NodeData wData)
        {
            NodeControlParallelContext thisContext = GetContext<NodeControlParallelContext>(wData);
            //初始化状态
            if (thisContext.runningStatus.Count != GetChildCount())
            {
                InitListTo(thisContext.runningStatus, NodeState.EXECUTING);
            }

            //执行并刷新状态数据
            bool hasFinished = false;
            bool hasExecuting = false;
            for (int i = 0; i < GetChildCount(); ++i)
            {
                //此节点评估未通过，返回
                if (thisContext.evaluationStatus[i] == false)
                    continue;
                //已经完成了
                if (NodeState.IsFinished(thisContext.runningStatus[i]))
                {
                    hasFinished = true;
                    continue;
                }
                //执行
                Node node = GetChild<Node>(i);
                int runningStatus = node.Execute(wData);
                if (NodeState.IsFinished(runningStatus))
                {
                    hasFinished = true;
                }
                else
                {
                    hasExecuting = true;
                }
                thisContext.runningStatus[i] = runningStatus;
            }

            //1，或条件 有已经完成了就是完成  2，并条件 都完成的就是完成
            if (excuteType == NodeParallelType.OR && hasFinished || excuteType == NodeParallelType.AND && hasExecuting == false)
            {
                InitListTo(thisContext.runningStatus, NodeState.EXECUTING);
                return NodeState.FINISHED;
            }
            return NodeState.EXECUTING;
        }

        protected override void OnTransition(NodeData wData)
        {
            NodeControlParallelContext thisContext = GetContext<NodeControlParallelContext>(wData);
            for (int i = 0; i < GetChildCount(); ++i)
            {
                Node node = GetChild<Node>(i);
                node.Transition(wData);
            }
            //clear running status
            InitListTo<int>(thisContext.runningStatus, NodeState.EXECUTING);
        }

        //初始化列表
        private void InitListTo<T>(List<T> list, T value)
        {
            int childCount = GetChildCount();
            if (list.Count != childCount)
            {
                list.Clear();
                for (int i = 0; i < childCount; ++i)
                {
                    list.Add(value);
                }
            }
            else
            {
                for (int i = 0; i < childCount; ++i)
                {
                    list[i] = value;
                }
            }
        }
    }
}
