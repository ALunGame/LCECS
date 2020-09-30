using LCECS.Core.Tree.Base;

namespace LCECS.Core.Tree.Nodes.Action
{
    /// <summary>
    /// 行为节点
    /// </summary>
    [Node(ViewName = "空行为",MaxChildCnt = -1)]
    public class NodeAction : Node
    {
        private const int ACTION_READY = 0;                     //准备中
        private const int ACTION_RUNNING = 1;                   //运行中
        private const int ACTION_FINISHED = 2;                  //完成

        //行为节点环境
        class NodeActionContext : NodeContext
        {
            internal int status;
            internal bool needExit;

            //自定义数据
            private object _userData;

            public T GetUserData<T>() where T : class, new()
            {
                if (_userData == null)
                {
                    _userData = new T();
                }
                return (T)_userData;
            }

            public NodeActionContext()
            {
                status = ACTION_READY;
                needExit = false;
                _userData = null;
            }
        }

        //重写执行
        protected sealed override int OnExcute(NodeData wData)
        {
            int runningState = NodeState.FINISHED;
            //获取环境数据
            NodeActionContext context = GetContext<NodeActionContext>(wData);
            //准备进入
            if (context.status == ACTION_READY)
            {
                OnEnter(wData);
                context.needExit = true;
                context.status = ACTION_RUNNING;
            }
            //运行执行
            if (context.status == ACTION_RUNNING)
            {
                runningState = OnRunning(wData);
                if (NodeState.IsFinished(runningState))
                {
                    context.status = ACTION_FINISHED;
                }
            }
            //完成退出
            if (context.status == ACTION_FINISHED)
            {
                if (context.needExit)
                {
                    OnExit(wData, runningState);
                }
                context.status = ACTION_READY;
                context.needExit = false;
            }
            return runningState;
        }

        //子类复写
        protected virtual void OnEnter(NodeData wData)
        {
        }
        protected virtual int OnRunning(NodeData wData)
        {
            return NodeState.FINISHED;
        }
        protected virtual void OnExit(NodeData wData, int runningStatus)
        {

        }
    } 
}
