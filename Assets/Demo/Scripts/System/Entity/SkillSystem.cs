using Demo.Com;
using Demo.Config;
using Demo.Help;
using LCECS;
using LCECS.Core.ECS;
using LCECS.Help;
using LCSkill;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Demo.System
{
    //技能系统
    public class SkillSystem : BaseSystem
    {
        private EffectConfig effectConfig;

        protected override List<Type> RegListenComs()
        {
            effectConfig = GameConfigHelp.GetConfig<EffectConfig>(ConfigType.Effect);
            return new List<Type>() {typeof(SkillCom)};
        }

        protected override void HandleComs(List<BaseCom> comList)
        {
            HandleSkill(GetCom<SkillCom>(comList[0]));
        }

        private void HandleSkill(SkillCom skillCom)
        {
            //没有技能释放请求
            if (skillCom.SkillId<=0)
            {
                return;
            }

            //生成一个时间线
            if (skillCom.Line == null)
                skillCom.Line = TimeLineHelp.CreateTimeLine();
            else
                skillCom.Line = TimeLineHelp.AddTimeLine(skillCom.Line);

            //重置
            skillCom.Line.ReSet();

            SkillJson json = GameConfigHelp.GetSkillInfo(skillCom.SkillId);
            //播放动画
            for (int i = 0; i < json.AnimClips.Count; i++)
            {
                SkillAnimJson animJson = json.AnimClips[i];
                skillCom.Line.AddTrack((float)animJson.Time, 0).OnStart(()=> {
                    PlayAnim(skillCom, animJson.AnimName);
                });
            }
            //播放特效
            for (int i = 0; i < json.Effects.Count; i++)
            {
                SkillEffectJson effectJson = json.Effects[i];
                skillCom.Line.AddTrack((float)effectJson.Time, 0).OnStart(() => {
                    PlayEffect(skillCom, effectJson);
                });
            }
            //播放音效
            for (int i = 0; i < json.Audios.Count; i++)
            {
                SkillAudioJson audioJson = json.Audios[i];
                skillCom.Line.AddTrack((float)audioJson.Time, 0).OnStart(() => {
                    PlayAudio(skillCom, audioJson.AudioId);
                });
            }
            //处理数据
            HandleSkillData(skillCom, json.Data);

            //开始播放技能
            skillCom.Line.Start();

            //重置
            skillCom.SkillId = 0;
        }

        private void PlayAnim(SkillCom skillCom, string animName)
        {
            if (animName == "")
            {
                ECSLocate.ECSLog.LogR("动画 为空>>>>>>> ", skillCom.SkillId, animName);
                return;
            }
            AnimatorClipInfo[] clipInfos = skillCom.Animator.GetCurrentAnimatorClipInfo(0);

            List<AnimatorClipInfo> showClips = new List<AnimatorClipInfo>();
            for (int i = 0; i < clipInfos.Length; i++)
            {
                if (clipInfos[i].clip.name== animName)
                {
                    showClips.Add(clipInfos[i]);
                }
            }


        }

        //特效
        private void PlayEffect(SkillCom skillCom, SkillEffectJson effect)
        {
            if (effect.EffectId<= 0)
            {
                ECSLocate.ECSLog.LogR("特效Id 为空>>>>>>> ", skillCom.SkillId, effect.EffectId);
                return;
            }
            ECSLocate.ECSLog.LogR("播放特效>>>>>>>>>", effect.EffectId);
            ECSLocate.ECS.SetGlobalSingleComData((EffectCom com) =>
            {
                com.CurrShowEffectId = effect.EffectId;
                com.EntityId = skillCom.EntityId;
                com.ShowPos = (Vector3)ReflectHelp.StrChangeToObject(effect.Pos,typeof(Vector3).FullName);
            });
        }

        //音效
        private void PlayAudio(SkillCom skillCom, int audioId)
        {
            if (audioId <= 0)
            {
                ECSLocate.ECSLog.LogR("音效Id 为空>>>>>>> ", skillCom.SkillId, audioId);
                return;
            }

            ECSLocate.ECSLog.Log("播放音效Id ", skillCom.SkillId, audioId);

        }

        //技能数值
        private void HandleSkillData(SkillCom skillCom, List<SkillDataJson> datas)
        {
            for (int i = 0; i < datas.Count; i++)
            {
                SkillDataJson dataJson = datas[i];
                switch (dataJson.Type)
                {
                    case SkillType.Once:
                        HandleOnceSkillData(skillCom, dataJson);
                        break;
                    case SkillType.Continue:
                        HandleContinueSkillData(skillCom, dataJson);
                        break;
                    case SkillType.Gap:
                        HandleGapSkillData(skillCom, dataJson);
                        break;
                    case SkillType.Buff:
                        HandleBuffSkillData(skillCom, dataJson);
                        break;
                    default:
                        break;
                }
            }
        }

        //作用一次技能
        private void HandleOnceSkillData(SkillCom skillCom, SkillDataJson dataJson)
        {
            skillCom.Line.AddTrack((float)dataJson.Time, 0).OnStart(() =>
            {
                ECSLocate.ECSLog.LogR("作用一次技能>>>>>>> ", skillCom.SkillId, dataJson.Data);
            });
        }

        //持续作用技能
        private void HandleContinueSkillData(SkillCom skillCom, SkillDataJson dataJson)
        {
            skillCom.Line.AddTrack((float)dataJson.Time, (float)dataJson.ContinueTime).OnUpdate(() =>
            {
                ECSLocate.ECSLog.LogR("持续作用技能>>>>>>> ", skillCom.SkillId, dataJson.Data);
            });
        }

        //间断作用技能
        private void HandleGapSkillData(SkillCom skillCom, SkillDataJson dataJson)
        {
            int gapCnt = (int)(dataJson.ContinueTime / dataJson.GapTime);
            for (int i = 0; i < gapCnt; i++)
            {
                float delayTime=(float)(dataJson.Time + i * dataJson.GapTime);
                skillCom.Line.AddTrack(delayTime, 0).OnStart(() =>
                {
                    ECSLocate.ECSLog.LogR("间断作用技能>>>>>>> ", skillCom.SkillId, dataJson.Data);
                });
            }
        }

        //作用一次,时间结束还原技能
        private void HandleBuffSkillData(SkillCom skillCom, SkillDataJson dataJson)
        {
            skillCom.Line.AddTrack((float)dataJson.Time, 0).OnStart(() =>
            {
                ECSLocate.ECSLog.LogR("作用一次,时间结束还原技能>>>>>>> 附加 ", skillCom.SkillId, dataJson.Data);
            }).OnCompleted(()=> {
                ECSLocate.ECSLog.LogR("作用一次,时间结束还原技能>>>>>>> 还原 ", skillCom.SkillId, dataJson.Data);
            });
        }
    }
}
