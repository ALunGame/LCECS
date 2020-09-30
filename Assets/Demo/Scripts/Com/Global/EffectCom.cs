using Demo.Config;
using LCECS.Core.ECS;
using System.Collections.Generic;
using UnityEngine;

namespace Demo.Com
{
    /// <summary>
    /// 特效组件（全局唯一）
    /// </summary>
    [Com(ViewName = "特效组件", IsGlobal = true)]
    public class EffectCom:BaseCom
    {
        public int CurrShowEffectId = 0;
        public int EntityId = 0;
        public Vector3 ShowPos = Vector3.zero;
        public Dictionary<int,EffectData> CurrShowEffects   = new Dictionary<int,EffectData>();
        public Dictionary<int,EffectData> CacheEffects      = new Dictionary<int,EffectData>();
    }

    public class EffectData
    {
        public EffectInfo Info;
        public List<EffectGo> EffectGos = new List<EffectGo>();
    }

    public class EffectGo
    {
        public float Time;
        public GameObject Go;
    }
}
