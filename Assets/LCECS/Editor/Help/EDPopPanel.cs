using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System;
using UnityEditor;
using UnityEngine;

namespace LCECS.Help
{
    /// <summary>
    /// 编辑器弹窗
    /// </summary>
    public class EDPopPanel: EditorWindow
    {
        public string InputStr = "";
        public Action<string> CallBack;

        private void OnGUI()
        {
            EDLayout.CreateVertical("box", position.width, position.height, () =>
            {
                InputStr=EditorGUILayout.TextField("请输入：", InputStr);

                EditorGUILayout.Space();

                EDButton.CreateBtn("确定", position.width * 0.9f, position.height * 0.5f, () =>
                {
                    if (CallBack!=null && InputStr!="")
                    {
                        CallBack(InputStr);
                        Close();
                    }
                });
            });
        }

        public static void PopWindow(string strContent, Action<string> callBack)
        {
            Rect rect = GUIHelper.GetEditorWindowRect().AlignCenter(250, 80);

            EDPopPanel window = GetWindowWithRect<EDPopPanel>(rect,true,strContent);
            window.CallBack = callBack;
        }
    }
}
