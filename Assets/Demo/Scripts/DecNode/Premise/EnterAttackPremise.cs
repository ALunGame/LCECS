using Demo.Com;
using Demo.Help;
using Demo.Info;
using LCECS;
using LCECS.Core.Tree.Base;
using LCECS.Data;

namespace DecNode.Premise
{
    [NodePremise("进入攻击区域")]
    public class EnterAttackPremise : NodePremise
    {
        public override bool OnMakeTrue(NodeData wData)
        {
            EntityWorkData workData = wData as EntityWorkData;
            EnemyCom enemyCom       = workData.MEntity.GetCom<EnemyCom>();
            SeekPathCom seekPathCom = workData.MEntity.GetCom<SeekPathCom>();

            MapSensorData mapSensor = ECSLayerLocate.Info.GetWorldInfo<MapSensorData>(WorldInfoKey.MapInfo);

            bool value = MapHelp.CheckPointInArea(seekPathCom.CurrPos, enemyCom.AttackArea, mapSensor.PlayerMapPos);
            return value;
        }
    }
}
