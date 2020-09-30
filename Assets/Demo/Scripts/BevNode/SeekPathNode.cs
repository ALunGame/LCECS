using Demo.Com;
using Demo.Config;
using Demo.Info;
using LCECS;
using LCECS.Core.Tree;
using LCECS.Core.Tree.Base;
using LCECS.Core.Tree.Nodes.Action;
using LCECS.Data;
using LCTileMap;
using UnityEngine;

namespace Demo.BevNode
{
    /// <summary>
    /// 寻路节点
    /// </summary>
    [Node(ViewName = "寻路节点", IsBevNode = true)]
    public class SeekPathNode: NodeAction
    {
        protected override void OnEnter(NodeData wData)
        {
            EntityWorkData workData = wData as EntityWorkData;
            //参数
            ParamData param = workData.GetReqParam((EntityReqId)workData.CurrReqId);
            Vector2Int targetPos = param.GetVect2Int();
            //对组件赋值
            SeekPathCom seekPathCom = workData.MEntity.GetCom<SeekPathCom>();
            if (!seekPathCom.TargetPos.Equals(targetPos))
            {
                seekPathCom.TargetPos = targetPos;
                seekPathCom.ReqSeek = true;
            }
        }

        protected override int OnRunning(NodeData wData)
        {
            EntityWorkData workData = wData as EntityWorkData;
            SeekPathCom seekPathCom = workData.MEntity.GetCom<SeekPathCom>();
            EnemyCom enemyCom = workData.MEntity.GetCom<EnemyCom>();

            //没有达到目标点
            if (!seekPathCom.CurrPos.Equals(seekPathCom.TargetPos))
            {
                return NodeState.EXECUTING;
            }

            if ((EntityReqId)workData.CurrReqId== EntityReqId.EnemyWander)
            {
                enemyCom.WanderIndex++;
                if (enemyCom.WanderIndex>enemyCom.WanderPath.Count-1)
                {
                    enemyCom.WanderIndex = 0;
                }
                //ECSLocate.ECSLog.LogError("徘徊完成》》》》》》》》》》》》》");
            }
            workData.CurrReqId = 0;
            return NodeState.FINISHED;
        }
    }
}