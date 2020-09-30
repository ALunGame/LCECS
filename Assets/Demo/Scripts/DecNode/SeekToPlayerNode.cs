using Demo.Info;
using LCECS;
using LCECS.Core.Tree.Base;
using LCECS.Core.Tree.Nodes.Action;
using LCECS.Data;

namespace Demo.DecNode
{
    /// <summary>
    /// 寻路玩家
    /// </summary>
    [Node(ViewName = "寻路玩家", IsBevNode = false)]
    public class SeekToPlayerNode: NodeAction
    {
        protected override void OnEnter(NodeData wData)
        {
            EntityWorkData workData = wData as EntityWorkData;
            MapSensorData mapSensor = ECSLayerLocate.Info.GetWorldInfo<MapSensorData>(WorldInfoKey.MapInfo);

            //发送请求
            ParamData paramData = workData.GetReqParam(EntityReqId.EnemySeekPlayer);
            paramData.SetVect2Int(mapSensor.PlayerMapPos);
            ECSLayerLocate.Request.PushEntityRequest(workData.MEntity.GetHashCode(), EntityReqId.EnemySeekPlayer);
        }
    }
}
