using LCECS.Help;
using UnityEditor;
using UnityEngine;

namespace LCSkill.Window
{
    [CustomEditor(typeof(Animator))]
    public class AnimatorInspector : Editor
    {
        private Animator TargetAnim;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            TargetAnim = (Animator)target;
            EDButton.CreateBtn("动画技能编辑器", 220, 45, (() => { SkillEditorWindow.OpenWindow(TargetAnim); }));
        }
    } 
}
