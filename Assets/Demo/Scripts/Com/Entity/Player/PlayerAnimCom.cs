﻿using LCECS.Core.ECS;
using UnityEngine;

namespace Demo.Com
{
    [Com(ViewName = "玩家动画组件",GroupName="Player")]
    public class PlayerAnimCom:BaseCom
    {
        public Animator Animtor;
        
        public Animator WaveAnimTor;
        protected override void OnInit(GameObject go)
        {
            Animtor = go.GetComponent<Animator>();
            
            WaveAnimTor=go.transform.Find("Wave").GetComponent<Animator>();
        }
    }
}
