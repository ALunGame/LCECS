using Demo.Help;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Demo.Config
{
    [CreateAssetMenu(fileName = "ConfigDict", menuName = "创建配置映射")]
    [Serializable]
    public class ConfigDict: ScriptableObject
    {
        public List<ConfigPathInfo> Config = new List<ConfigPathInfo>();
    }

    [Serializable]
    public class ConfigPathInfo
    {
        public ConfigType Type;
        public ScriptableObject Asset;
    }
}
