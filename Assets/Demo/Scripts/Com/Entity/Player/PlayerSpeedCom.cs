using LCECS;
using LCECS.Core.ECS;
using UnityEngine;

namespace Demo.Com
{
    /// <summary>
    /// 玩家速度组件
    /// </summary>
    [Com(ViewName = "玩家速度组件",GroupName="Player")]
    public class PlayerSpeedCom:BaseCom
    {
        [ComValue]
        public float Speed=0;
        [ComValue(ViewEditor = true)]
        public float MaxSpeed;

        [ComValue]
        public float UpSpeed=0;
        [ComValue(ViewEditor = true)]
        public float MaxUpSpeed;

        [ComValue]
        public bool DoDash=false;
        [ComValue]
        public bool IsDash=false;
        [ComValue(ViewEditor = true)]
        public float MaxDashSpeed;
        [ComValue(ViewEditor = true)]
        public float DashDragTime;

        [ComValue]
        public int CurrJumpIndex=0;
        [ComValue(ViewEditor = true)]
        public int MaxJumpIndex;


        [ComValue(ViewEditor = true)]
        public float FallMultiplier = 1.5f;
        [ComValue(ViewEditor = true)]
        public float LowJumpMultiplier = 1f;
        
        protected override void OnInit(GameObject go)
        {
        }
    }
}