using UnityEngine;
using System.Collections;
using LCECS.Core.Tree.Base;
using LCECS.Core.Tree.Nodes.Action;
using LCECS.Data;
using LCECS;
using Demo.Com;

namespace Demo.BevNode
{
    [Node(ViewName = "玩家普通攻击节点", IsBevNode = true)]
    public class PlayerNormalAttackNode : NodeAction
    {
        protected override void OnEnter(NodeData wData)
        {
            EntityWorkData workData = wData as EntityWorkData;
            //参数
            bool doAttack = workData.GetReqParam(EntityReqId.PlayerNormalAttack).GetBool();
            if (doAttack==false)
            {
                return;
            }

            //组件
            SkillCom skillCom = workData.MEntity.GetCom<SkillCom>();
            skillCom.SkillId  = 1001;
            skillCom.EntityId = workData.Id;
        }
    }

}