using LCECS.Core.Tree.Base;

namespace LCECS.Core.Tree.Nodes.Action
{
    /// <summary>
    /// 实体请求行为            （向行为发请求层）
    /// </summary>
    [Node(ViewName = "实体请求行为", IsBevNode = false)]
    public class NodeActionEntityReqBev : NodeAction
    {
        [NodeValue(ViewEditor = true)]
        public EntityReqId ReqId = EntityReqId.None;

        protected override void OnEnter(NodeData wData)
        {
            //ECSLocate.ECSLog.LogR("发送请求>>>>>>", wData.Id,ReqId.ToString());
            ECSLayerLocate.Request.PushEntityRequest(wData.Id, ReqId);
        }

        protected override int OnRunning(NodeData wData)
        {
            return NodeState.FINISHED;
        }
    }
}
