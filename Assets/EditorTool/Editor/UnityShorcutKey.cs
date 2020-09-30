using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class UnityShorcutKey
{

    [MenuItem("Edit/快捷键/清空日志 &#c")]
    private static void Invert()
    {
        var type = Assembly
                .GetAssembly(typeof(SceneView))
#if UNITY_2017_1_OR_NEWER
                    .GetType("UnityEditor.LogEntries")
#else
				.GetType( "UnityEditorInternal.LogEntries" )
#endif
                ;

        var attr = BindingFlags.Static | BindingFlags.Public;
        var method = type.GetMethod("Clear", attr);
        method.Invoke(null, null);
    }

    [MenuItem("Edit/快捷键/切换Debug模式 &#c")]
    private static void ToggleDebugMode()
    {
        var window = Resources.FindObjectsOfTypeAll<EditorWindow>();
        var inspectorWindow = ArrayUtility.Find(window, c => c.GetType().Name == "InspectorWindow");

        if (inspectorWindow == null) return;

        var inspectorType = inspectorWindow.GetType();
        var tracker = ActiveEditorTracker.sharedTracker;
        var isNormal = tracker.inspectorMode == InspectorMode.Normal;
        var methodName = isNormal ? "SetDebug" : "SetNormal";

        var attr = BindingFlags.NonPublic | BindingFlags.Instance;
        var methodInfo = inspectorType.GetMethod(methodName, attr);
        methodInfo.Invoke(inspectorWindow, null);
        tracker.ForceRebuild();
    }

    [MenuItem("Edit/快捷键/切换Debug模式 &#c", true)]
    private static bool CanToggle()
    {
        var window = Resources.FindObjectsOfTypeAll<EditorWindow>();
        var inspectorWindow = ArrayUtility.Find(window, c => c.GetType().Name == "InspectorWindow");

        return inspectorWindow != null;
    }

    #region 节点上移下移

    [MenuItem("Edit/快捷键/节点上移 &UP")]
    private static void Up()
    {
        var t = Selection.activeTransform;
        t.SetSiblingIndex(t.GetSiblingIndex() - 1);
    }

    [MenuItem("Edit/快捷键/节点上移 &UP", true)]
    private static bool CanUp()
    {
        return Selection.activeTransform != null;
    }

    [MenuItem("Edit/快捷键/节点下移 &DOWN")]
    private static void Down()
    {
        var t = Selection.activeTransform;
        t.SetSiblingIndex(t.GetSiblingIndex() + 1);
    }

    [MenuItem("Edit/快捷键/节点下移 &DOWN", true)]
    private static bool CanDown()
    {
        return Selection.activeTransform != null;
    }

    #endregion

    #region Unity启动停止

    [MenuItem("Edit/快捷键/运行Unity _F5")]
    private static void Run()
    {
        EditorApplication.isPlaying = true;
    }

    [MenuItem("Edit/快捷键/运行Unity _F5", true)]
    private static bool CanRun()
    {
        return !EditorApplication.isPlaying;
    }

    [MenuItem("Edit/快捷键/关闭Unity #_F5")]
    private static void Stop()
    {
        EditorApplication.isPlaying = false;
    }

    [MenuItem("Edit/快捷键/关闭Unity #_F5", true)]
    private static bool CanStop()
    {
        return EditorApplication.isPlaying;
    }

    #endregion

    #region 节点序号

    private static Regex m_regex = new Regex(@"(.*)(\([0-9]*\))");
    [MenuItem("Edit/快捷键/清除节点序号 &r")]
    private static void Remove()
    {
        var list = Selection.gameObjects
                .Where(c => m_regex.IsMatch(c.name))
                .ToArray()
            ;

        if (list.Length == 0) return;

        foreach (var n in list)
        {
            Undo.RecordObject(n, "Remove Duplicated Name");
            n.name = m_regex.Replace(n.name, @"$1");
            n.name = n.name.Replace(" ", "");
        }
    }

    [MenuItem("Edit/快捷键/清除节点序号 &r", true)]
    private static bool CanRemove()
    {
        var gameObjects = Selection.gameObjects;
        return gameObjects != null && 0 < gameObjects.Length;
    }

    [MenuItem("Edit/快捷键/创建不带节点序号的节点 &d")]
    private static void Duplicate()
    {
        var list = new List<int>();

        foreach (var n in Selection.gameObjects)
        {
            var clone = Object.Instantiate(n, n.transform.parent);
            clone.name = n.name;
            list.Add(clone.GetInstanceID());
            Undo.RegisterCreatedObjectUndo(clone, "Duplicate Without Serial Number");
        }

        Selection.instanceIDs = list.ToArray();
        list.Clear();
    }

    [MenuItem("Edit/快捷键/创建不带节点序号的节点 &d", true)]
    private static bool CanDuplicate()
    {
        var gameObjects = Selection.gameObjects;
        return gameObjects != null && 0 < gameObjects.Length;
    }

    #endregion
}
