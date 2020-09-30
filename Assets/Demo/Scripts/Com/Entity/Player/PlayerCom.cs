using LCECS.Core.ECS;
using UnityEngine;

namespace Demo.Com
{
    /// <summary>
    /// 玩家组件
    /// </summary>
    [Com(ViewName = "玩家组件", GroupName="Player")]
    public class PlayerCom:BaseCom
    {
        public Transform        Trans;
        public SpriteRenderer   SpriteRender;
        public Transform        WaveTrans;

        [ComValue]
        public float Energy=0;

        [ComValue(ViewEditor=true)]
        public float AddEnergy=0;

        [ComValue(ViewEditor = true)]
        public float MaxEnergy=0;

        protected override void OnInit(GameObject go)
        {
            SpriteRender = go.GetComponent<SpriteRenderer>();
            Trans = go.transform;
            WaveTrans = go.transform.Find("Wave");
        } 
    }
}