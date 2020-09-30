using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LCSkill.Window
{
    public class AnimatorSkillEditorWindow : EditorWindow
    {
        private static Animator Anim;
        public static void OpenWindow(Animator anim)
        {
            Anim = anim;
            AnimatorSkillEditorWindow window = GetWindow<AnimatorSkillEditorWindow>();
            window.titleContent = new GUIContent("动画技能编辑器");
        }

        private List<AnimationClip> AnimClips = new List<AnimationClip>();
        private void OnEnable()
        {
            for (int i = 0; i < Anim.runtimeAnimatorController.animationClips.Length; i++)
            {
                AnimationClip animationClip = Anim.runtimeAnimatorController.animationClips[i];
                AnimClips.Add(animationClip);
            }
        }
    }
}
