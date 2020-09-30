using LCECS.Core.ECS;
using UnityEngine;

namespace Demo.Com
{
    /// <summary>
    /// 速度组件
    /// </summary>
    [Com(ViewName = "速度组件")]
    public class SpeedCom : BaseCom
    {
        [ComValue]
        public float Speed=0;
        [ComValue(ViewEditor = true)]
        public float MaxSpeed;

        protected override void OnInit(GameObject go)
        {
        }
    }
}
