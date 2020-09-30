using UnityEditor;
using UnityEditor.SceneManagement;

namespace EditorTool
{
    /// <summary>
    /// Unity开发辅助
    /// </summary>
    public class UnityDevelopHelp
    {
        [MenuItem("Edit/快捷键/打开主场景 %i")]
        private static void OpenPlayerScnen()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
        }

        [MenuItem("Edit/快捷键/打开地图场景 %m")]
        private static void OpenMapScnen()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/MapScene.unity");
        }
    }
}
