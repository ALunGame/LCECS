using System;
using System.Collections.Generic;
using UnityEngine;

namespace Demo.Config
{
    [CreateAssetMenu(fileName = "EffectConfig", menuName = "创建特效配置项")]
    [Serializable]
    public class EffectConfig:ScriptableObject
    {
        public List<EffectInfo> EffectList = new List<EffectInfo>();
    }

    [Serializable]
    public class EffectInfo
    {
        [Header("特效Id")]
        public int Id;
        [Header("描述")]
        public string Des;
        [Header("是否跟随节点")]
        public bool FollowObj = false;
        [Header("持续时间")]
        public float ContinueTime;
        [Header("间隔时间")]
        public float IntervalTime;
        [Header("预制体")]
        public GameObject Prefab;
    }
}
