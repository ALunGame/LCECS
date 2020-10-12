using LCECS;
using LCECS.Help;
using LCHelp;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LCSkill
{
    public class SkillEditorWindow : EditorWindow
    {
        private const string JsonPath = "/" + ECSDefinitionPath.SkillJsonPath;
        private static SkillList Json = new SkillList();

        private static Animator SelAnim = null;
        private static AnimationClip SelClip = null;
        private static GameObject SelGo = null;
        private static List<AnimationClip> RunningClip = new List<AnimationClip>();

        [MenuItem("LCECS/技能编辑")]
        private static void OpenWindow()
        {
            SelGo = null;
            SelAnim = null;
            SelClip = null;
            RunningClip.Clear();

            SkillEditorWindow window = GetWindow<SkillEditorWindow>("技能编辑器");
            window.minSize = new Vector2(1250, 800);

            if (EDTool.CheckFileInPath(Application.dataPath + JsonPath))
            {
                string dataJson = EDTool.ReadText(Application.dataPath + JsonPath);
                Json = LitJson.JsonMapper.ToObject<SkillList>(dataJson);
            }
        }

        public static void OpenWindow(Animator anim)
        {
            SelGo = null;
            SelAnim = null;
            SelClip = null;
            RunningClip.Clear();

            SkillEditorWindow window = GetWindow<SkillEditorWindow>("技能编辑器");
            window.minSize = new Vector2(1250, 800);
            SelGo = anim.gameObject;
            SelAnim = anim;
            GetAnimClips();
            if (EDTool.CheckFileInPath(Application.dataPath + JsonPath))
            {
                string dataJson = EDTool.ReadText(Application.dataPath + JsonPath);
                Json = LitJson.JsonMapper.ToObject<SkillList>(dataJson);
            }
        }

        public static void GetAnimClips()
        {
            if (SelAnim==null)
            {
                return;
            }
            for (int i = 0; i < SelAnim.runtimeAnimatorController.animationClips.Length; i++)
            {
                var animationClip = SelAnim.runtimeAnimatorController.animationClips[i];
                RunningClip.Add(animationClip);
            }
        }

        private void OnGUI()
        {
            EDLayout.CreateHorizontal("box", position.width, position.height, (float weight, float height) =>
            {
                DrawSkillList(200, height);
                DrawSkill(770, height);
                DrawAnimList(230, height);
            });
        }

        private void Update()
        {
            TimeLineHelp.Update();
            PlayAnim();
        }

        private void OnDestroy()
        {
            SelGo = null;
            SelAnim = null;
            SelClip = null;
            RunningClip.Clear();
            SaveEntityJsonList();
            TimeLineHelp.Clear();
        }

        private SkillJson SelSkill;
        private float AnimPlayTime { get; set; }
        private bool IsPlaying = false;
        private float TimePlaying = 0;
        private float TimePlayScale = 1;

        #region 技能列表

        private Vector2 LeftPos = Vector2.zero;
        private void DrawSkillList(float width,float height)
        {
            EDLayout.CreateScrollView(ref LeftPos, "", width, height, () =>
            {
                for (int i = 0; i < Json.List.Count; i++)
                {
                    SkillJson skill = Json.List[i];
                    EDButton.CreateBtn(skill.Id, 180, 25, () =>
                    {
                        SelSkillChange(skill);
                    });
                }
                //添加技能
                EDButton.CreateBtn("新建技能", 180, 30, () =>
                {
                    EDPopPanel.PopWindow("输入技能Id>>>>>>", (x) =>
                    {
                        if (CheckContainSkill(x))
                        {
                            Debug.LogError("技能Id重复>>>>>>>>>>>>" + x);
                            return;
                        }
                        int skillId = int.Parse(x);
                        SkillJson json = new SkillJson
                        {
                            Id = skillId,
                        };
                        Json.List.Add(json);
                    });
                });
            });
        }

        #endregion

        #region 动画信息
        private Vector2 animPos = Vector2.zero;
        private void DrawAnimList(float width, float height)
        {
            if (SelSkill == null)
                return;
            EDLayout.CreateVertical("", width, height, (float wid, float hei) =>
            {
                if (SelSkill.AnimClips.Count!=0)
                {
                    EDLayout.CreateScrollView(ref animPos, "box", wid, hei * 0.7f, (float wid01, float hei01) =>
                    {
                        //动画列表
                        for (int i = 0; i < SelSkill.AnimClips.Count; i++)
                        {
                            int selIndex = i;
                            if (CheckAnimatorHasClip(SelAnim,SelSkill.AnimClips[i].AnimName))
                            {
                                EDLayout.CreateHorizontal("box", 210, 30, (float wid02, float hei02) =>
                                {
                                    EDTypeField.CreateLableField(string.Format("动画名:{0}", SelSkill.AnimClips[i].AnimName), "", wid02 * 0.7f, hei02);
                                    EDButton.CreateBtn("删除动画", wid02 * 0.3f, hei02, () => {
                                        SelSkill.AnimClips.RemoveAt(selIndex);
                                        UpdateAnimClipsTime();
                                    });
                                });
                            }
                            else
                            {
                                SelSkill.AnimClips.RemoveAt(selIndex);
                                UpdateAnimClipsTime();
                            }
                        }
                    });
                }

                //添加动画
                if (RunningClip.Count == 0)
                {
                    return;
                }
                List<string> showClips = new List<string>();
                for (int i = 0; i < RunningClip.Count; i++)
                {
                    showClips.Add(RunningClip[i].name);
                }
                EDButton.CreateBtn("添加动画", 180, 25, () => {
                    EDPopMenu.CreatePopMenu(showClips, (string str) =>
                    {
                        SkillAnimJson animJson = new SkillAnimJson();
                        animJson.AnimName = str;
                        SelSkill.AnimClips.Add(animJson);
                        UpdateAnimClipsTime();
                    });
                });
            });
        }

        //更新动画播放时间
        public void UpdateAnimClipsTime()
        {
            for (int i = 0; i < SelSkill.AnimClips.Count; i++)
            {
                List<string> lastAnims = new List<string>();
                for (int j = 0; j < i; j++)
                {
                    lastAnims.Add(SelSkill.AnimClips[j].AnimName);
                    SelSkill.AnimClips[i].Time = GetAnimatorClipTime(SelAnim, lastAnims);
                }
            }
        }

        public float GetAnimatorClipTime(Animator animator, List<string> names)
        {
            //动画片段时间长度
            float length = 0;
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            foreach (AnimationClip clip in clips)
            {
                if (names.Contains(clip.name))
                {
                    length += clip.length;
                }
            }
            return length;
        }

        public bool CheckAnimatorHasClip(Animator animator, string name)
        {
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            foreach (AnimationClip clip in clips)
            {
                if (name==clip.name)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region 技能信息
        private bool ShowInfo = false;
        private bool ShowData = false;
        private bool ShowAudio = false;
        private bool ShowEffect = false;
        private Vector2 midPos = Vector2.zero;
        private Vector2 dataPos = Vector2.zero;
        private Vector2 audioPos = Vector2.zero;
        private Vector2 effectPos = Vector2.zero;

        private void DrawSkill(float width, float height)
        {
            if (SelSkill==null)
                return;

            EDLayout.CreateVertical("box", width, height, (float wid, float hei) =>
            {
                //动画
                if (SelAnim != null)
                {
                    AnimPlayTime = EditorGUILayout.Slider("播放动画", AnimPlayTime, 0, GetTotalAnimTime());

                    object playScale = TimePlayScale;
                    EDTypeField.CreateTypeField("播放动画时间倍数:", ref playScale, typeof(string), width, 25);
                    TimePlayScale = float.Parse(playScale.ToString());

                    if (IsPlaying)
                    {
                        GUI.color = Color.red;
                        EDButton.CreateBtn("暂停", width, 50, () =>
                        {
                            IsPlaying = false;
                        });
                    }
                    else
                    {
                        GUI.color = Color.green;
                        EDButton.CreateBtn("播放", width, 50, () =>
                        {
                            IsPlaying = true;
                        });
                    }
                }
                GUI.color = Color.white;
                EDTypeField.CreateLableField("技能Id:" + SelSkill.Id, "", width, 25);
                object value = SelSkill.DecStr;
                EDTypeField.CreateTypeField("技能描述:", ref value, typeof(string), width, 25);
                SelSkill.DecStr = value.ToString();

                object triggerValue = SelSkill.TriggerTime;
                EDTypeField.CreateTypeField("技能触发时间:", ref triggerValue, typeof(double), width, 25);
                SelSkill.TriggerTime = double.Parse(triggerValue.ToString());

                EDButton.CreateBtn("删除技能", 200, 50, () =>
                {
                    Json.List.Remove(SelSkill);
                    SelSkill = null;
                });

                ShowInfo = EditorGUILayout.Foldout(ShowInfo, "技能信息");
                if (ShowInfo)
                    DrawSkillInfo(width, height * 0.3f);
                GUILayout.Space(10);

                ShowData = EditorGUILayout.Foldout(ShowData, "技能数据");
                if (ShowData)
                    DrawSkillData(width, height * 0.3f);
                GUILayout.Space(10);

                ShowAudio = EditorGUILayout.Foldout(ShowAudio, "技能音效");
                if (ShowAudio)
                    DrawSkillAudio(width, height * 0.3f);
                GUILayout.Space(10);

                ShowEffect = EditorGUILayout.Foldout(ShowEffect, "技能特效");
                if (ShowEffect)
                    DrawSkillEffect(width, height * 0.3f);
                GUILayout.Space(10);
            });
        }

        private void DrawSkillInfo(float width, float height)
        {
            object conValue = SelSkill.ContinueTime;
            EDTypeField.CreateTypeField("技能持续时间:", ref conValue, typeof(double), width, 25);
            SelSkill.ContinueTime = double.Parse(conValue.ToString());

            object value = LCConvert.StrChangeToObject(SelSkill.Area, typeof(Vector2).FullName);
            EDTypeField.CreateTypeField("技能范围:", ref value, typeof(Vector2), width, 25);
            SelSkill.Area = value.ToString();
        }

        private void DrawSkillData(float width, float height)
        {
            EDLayout.CreateScrollView(ref dataPos, "", width, height, (float wid, float hei) =>
            {
                for (int i = 0; i < SelSkill.Data.Count; i++)
                {
                    EDLayout.CreateVertical("GroupBox", wid*0.9f, 55, (float wid01, float hei01) =>
                    {
                        int selIndex = i;
                        SkillDataJson dataJson = SelSkill.Data[i];

                        //时间信息
                        EDLayout.CreateHorizontal("box", wid01, 25, (float wid02, float hei02) =>
                        {
                            //作用时间
                            object timeValue = dataJson.Time;
                            EDTypeField.CreateTypeField("作用时间:", ref timeValue, typeof(double), 220, 25);
                            dataJson.Time = double.Parse(timeValue.ToString());

                            //持续时间
                            object continueTimeValue = dataJson.ContinueTime;
                            EDTypeField.CreateTypeField("持续时间:", ref continueTimeValue, typeof(double), 220, 25);
                            dataJson.ContinueTime = double.Parse(continueTimeValue.ToString());

                            //间隔时间
                            object gapTimeValue = dataJson.GapTime;
                            EDTypeField.CreateTypeField("间隔时间:", ref gapTimeValue, typeof(double), 220, 25);
                            dataJson.GapTime = double.Parse(gapTimeValue.ToString());
                        });

                        //技能枚举
                        EDLayout.CreateHorizontal("box", wid01, 25, (float wid02, float hei02) =>
                        {
                            //技能类型
                            dataJson.Type=(SkillType)EditorGUILayout.EnumPopup("技能类型：", dataJson.Type, GUILayout.Width(220), GUILayout.Height(25));

                            //技能作用类型
                            dataJson.UseType = (SkillUseType)EditorGUILayout.EnumPopup("作用类型：", dataJson.UseType, GUILayout.Width(220), GUILayout.Height(25));

                            //数据类型
                            dataJson.DataType = (SkillDataType)EditorGUILayout.EnumPopup("数据类型：", dataJson.DataType, GUILayout.Width(220), GUILayout.Height(25));
                        });

                        //数据
                        object data = dataJson.Data;
                        EDTypeField.CreateTypeField("数据：", ref data, typeof(String), 220, 20);
                        dataJson.Data = data.ToString();

                        EDButton.CreateBtn("删除数据", wid01, 20, () => {
                            SelSkill.Data.RemoveAt(selIndex);
                        });
                    });

                }

                EDButton.CreateBtn("添加数据", wid*0.9f, 25, () => {
                    SelSkill.Data.Add(new SkillDataJson());
                });
            });
        }

        private void DrawSkillAudio(float width, float height)
        {
            EDLayout.CreateScrollView(ref audioPos, "", width, height, (float wid, float hei) =>
            {
                //音效列表
                for (int i = 0; i < SelSkill.Audios.Count; i++)
                {
                    EDLayout.CreateVertical("GroupBox", wid * 0.9f, 55, (float wid01, float hei01) => {
                        int selIndex = i;
                        object conValue = SelSkill.Audios[i].Time;
                        EDTypeField.CreateTypeField("音效播放时间:", ref conValue, typeof(double), wid01, 25);
                        SelSkill.Audios[i].Time = double.Parse(conValue.ToString());

                        //音效Id
                        object idValue = SelSkill.Audios[i].AudioId;
                        EDTypeField.CreateTypeField("音效Id:", ref idValue, typeof(int), wid01, 25);
                        SelSkill.Audios[i].AudioId = int.Parse(idValue.ToString());

                        EDButton.CreateBtn("删除音效", wid01, 25, () => {
                            SelSkill.Audios.RemoveAt(selIndex);
                        });
                    });
                }

                EDButton.CreateBtn("添加音效", wid * 0.9f, 25, () => {
                    SelSkill.Audios.Add(new SkillAudioJson());
                });
            });
        }

        private void DrawSkillEffect(float width, float height)
        {
            EDLayout.CreateScrollView(ref effectPos, "", width, height, (float wid, float hei) =>
            {
                EDLayout.CreateVertical("GroupBox", wid * 0.9f, 55, (float wid01, float hei01) => {
                    //特效列表
                    for (int i = 0; i < SelSkill.Effects.Count; i++)
                    {
                        int selIndex = i;
                        object conValue = SelSkill.Effects[i].Time;
                        EDTypeField.CreateTypeField("特效播放时间:", ref conValue, typeof(double), wid01, 25);
                        SelSkill.Effects[i].Time = double.Parse(conValue.ToString());

                        //特效Id
                        object idValue = SelSkill.Effects[i].EffectId;
                        EDTypeField.CreateTypeField("特效Id:", ref idValue, typeof(int), wid01, 25);
                        SelSkill.Effects[i].EffectId=int.Parse(idValue.ToString());

                        //特效位置
                        object value = LCConvert.StrChangeToObject(SelSkill.Effects[i].Pos, typeof(Vector3).FullName);
                        EDTypeField.CreateTypeField("特效位置:", ref value, typeof(Vector3), width, 25);
                        SelSkill.Effects[i].Pos = value.ToString();

                        EDButton.CreateBtn("删除特效", wid01, 25, () => {
                            SelSkill.Effects.RemoveAt(selIndex);
                        });
                    }
                });

                EDButton.CreateBtn("添加特效", wid * 0.9f, 25, () => {
                    SelSkill.Effects.Add(new SkillEffectJson());
                });
            });
        }

        #endregion

        #region 动画播放相关

        private void PlayAnim()
        {
            if (SelGo == null)
                return;
            AnimationClip clip = GetAnimClipByTime(AnimPlayTime, out float playTime);
            if (clip==null)
                return;
            if (IsPlaying)
            {
                TimePlaying += Time.deltaTime * TimePlayScale;
                TimePlaying = TimePlaying > GetTotalAnimTime() ? 0 : TimePlaying;
                playTime    = TimePlaying;
                AnimPlayTime = TimePlaying;
                PlaySkillTimeLine();
            }
            clip.SampleAnimation(SelGo, playTime);
        }

        private float GetTotalAnimTime()
        {
            if (SelGo == null)
                return 0;
            if (SelSkill==null || SelSkill.AnimClips.Count==0)
            {
                return 0;
            }
            float totalTime = 0;

            for (int i = 0; i < SelSkill.AnimClips.Count; i++)
            {
                AnimationClip clip = GetAnimClip(SelSkill.AnimClips[i].AnimName);
                if (clip)
                {
                    totalTime += clip.length;
                }
            }

            return totalTime;
        }

        private AnimationClip GetAnimClip(string name)
        {
            if (RunningClip.Count==0)
            {
                return null;
            }
            for (int i = 0; i < RunningClip.Count; i++)
            {
                if (RunningClip[i].name==name)
                {
                    return RunningClip[i];
                }
            }
            return null;
        }

        private AnimationClip GetAnimClipByTime(float animTime,out float playTime)
        {
            playTime = animTime;
            if (SelSkill == null || SelSkill.AnimClips.Count == 0)
            {
                return null;
            }
            for (int i = 0; i < SelSkill.AnimClips.Count; i++)
            {
                AnimationClip clip = GetAnimClip(SelSkill.AnimClips[i].AnimName);
                if (clip)
                {
                    float time = clip.length;
                    if (animTime<=time)
                    {
                        return clip;
                    }
                    else
                    {
                        animTime -= clip.length;
                        playTime = animTime;
                    }
                }
            }
            return null;
        }

        #endregion

        #region 技能时间线

        private void PlaySkillTimeLine()
        {
            if (SelSkill==null)
            {
                return;
            }
            TimeLineHelp.Clear();
            TimeLine line = TimeLineHelp.CreateTimeLine();
            //Effect
            for (int i = 0; i < SelSkill.Effects.Count; i++)
            {
                SkillEffectJson effect = SelSkill.Effects[i];
                line.AddTrack((float)effect.Time, 0).OnStart(()=> {
                    Debug.Log("Effect》》》》》》》");
                });
            }

            //Audio
            for (int i = 0; i < SelSkill.Audios.Count; i++)
            {
                SkillAudioJson audio = SelSkill.Audios[i];
                line.AddTrack((float)audio.Time, 0).OnStart(() => {
                    Debug.Log("Audio》》》》》》》");
                });
            }
        }

        #endregion

        //选择实体改变
        private void SelSkillChange(SkillJson skill)
        {
            if (skill == null)
            {
                SelSkill = null;
                return;
            }
            SelSkill = skill;
        }

        //保存实体Json数据
        private void SaveEntityJsonList()
        {
            if (Json.List.Count == 0)
            {
                return;
            }
            string jsonData = LitJson.JsonMapper.ToJson(Json);
            EDTool.WriteText(jsonData, Application.dataPath + JsonPath);
            AssetDatabase.Refresh();
        }

        private bool CheckContainSkill(string id)
        {
            int skillId = int.Parse(id);
            for (int i = 0; i < Json.List.Count; i++)
            {
                if (Json.List[i].Id==skillId)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
