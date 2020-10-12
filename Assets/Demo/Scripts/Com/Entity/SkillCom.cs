using LCECS.Core.ECS;
using LCHelp;
using UnityEngine;

namespace Demo.Com
{
    [Com(ViewName = "技能组件",GroupName ="Entity")]
    public class SkillCom: BaseCom
    {
        public int EntityId;
        public int SkillId;
        public int LastSkillId;
        public Vector3 SpawnPos;
        public TimeLine Line;
        public Animator Animator;

        protected override void OnInit(GameObject go)
        {
            Animator = go.GetComponent<Animator>();
        }
    }
}
