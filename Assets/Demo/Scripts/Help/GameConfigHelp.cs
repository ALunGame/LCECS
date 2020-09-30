using Demo.Config;
using LCECS;
using LCSkill;
using UnityEngine;

namespace Demo.Help
{
    public enum ConfigType
    {
        Effect,
    }

    public class GameConfigHelp
    {
        private static ConfigDict configPath = null;
        private static SkillList skillList;

        public static SkillJson GetSkillInfo(int skillId)
        {
            if (skillList==null)
            {
                string path = "Resources/SkillJson.txt";
                TextAsset jsonData = ECSLocate.Factory.GetProduct<TextAsset>(FactoryType.Asset, null, path);
                skillList = LitJson.JsonMapper.ToObject<SkillList>(jsonData.text);
            }
            for (int i = 0; i < skillList.List.Count; i++)
            {
                if (skillList.List[i].Id == skillId)
                {
                    return skillList.List[i];
                }
            }
            return null;
        }

        private static void CheckConfigDict()
        {
            if (configPath==null)
            {
                configPath = Resources.Load<ConfigDict>("Config/ConfigDict");
            }
        }

        public static T GetConfig<T>(ConfigType type)where T: ScriptableObject
        {
            CheckConfigDict();
            for (int i = 0; i < configPath.Config.Count; i++)
            {
                if (configPath.Config[i].Type== type)
                {
                    return (T)configPath.Config[i].Asset;
                }
            }
            return default;
        }
    }
}
