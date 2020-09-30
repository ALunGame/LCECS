using System.Collections.Generic;
using UnityEngine;

namespace LCSkill
{
    public class SkillList
    {
        public List<SkillJson> List = new List<SkillJson>();
    }

    public class SkillJson
    {
        public int Id                        = 0;
        public string DecStr                 = "";
        public double TriggerTime            = 0;
                                             
        public double ContinueTime           = 0;
        public string Area                   = "";

        public List<SkillDataJson> Data      = new List<SkillDataJson>();
        public List<SkillEffectJson> Effects = new List<SkillEffectJson>();
        public List<SkillAudioJson> Audios   = new List<SkillAudioJson>();
        public List<SkillAnimJson> AnimClips = new List<SkillAnimJson>();
        public List<int> ExSkills            = new List<int>();
    }

    /// <summary>
    /// 技能类型
    /// </summary>
    public enum SkillType
    {
        Once,                   //作用一次
        Continue,               //持续作用
        Gap,                    //间断作用
        Buff,                   //作用一次,时间结束还原
    }

    /// <summary>
    /// 技能作用类型
    /// </summary>
    public enum SkillUseType
    {
        Self,                   //自己
        Other,                  //他人
        Friends,                //友方（多个）
        Enemys,                 //敌方（多个）
    }

    /// <summary>
    /// 技能数据类型
    /// </summary>
    public enum SkillDataType
    {
        HP,                     //生命
        Energy,                 //精力
        Speed,                  //速度
    }

    public class SkillDataJson
    {
        //作用时间
        public double Time              = 0;
        //持续时间
        public double ContinueTime      = 0;
        //间隔时间
        public double GapTime           = 0;
        //各种枚举
        public SkillType Type           = SkillType.Once;
        public SkillUseType UseType     = SkillUseType.Other;
        public SkillDataType DataType   = SkillDataType.HP;
        //数据
        public string Data              = "";
    }

    public class SkillEffectJson
    {
        public double Time  = 0;
        public string Pos = "";
        public int EffectId = 0;
    }

    public class SkillAudioJson
    {
        public double Time = 0;
        public int AudioId = 0;
    }

    public class SkillAnimJson
    {
        public double Time = 0;
        public string AnimName = "";
    }
}
