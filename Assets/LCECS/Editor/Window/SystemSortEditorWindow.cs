using UnityEngine;
using UnityEditor;
using LCECS.Help;
using LCECS.Data;
using LCECS;
using System.Collections.Generic;
using System;
using LCECS.Core.ECS;

public class SystemSortEditorWindow : EditorWindow
{
    [MenuItem("LCECS/系统排序")]
    static void OpenWindow()
    {
        if (EDTool.CheckFileInPath(Application.dataPath + JsonPath))
        {
            string dataJson = EDTool.ReadText(Application.dataPath + JsonPath);
            MSystemSortJson = LitJson.JsonMapper.ToObject<SystemSortJson>(dataJson);
        }
        var window = GetWindow<SystemSortEditorWindow>();
        window.titleContent = new GUIContent("系统排序");
        window.Show();
    }
    private static SystemSortJson MSystemSortJson = new SystemSortJson();
    private const string JsonPath = "/" + ECSDefinitionPath.SystemSortPath;
    private Vector2 ScPos;

    private void OnEnable()
    {
        UpdateSystemJson();
        GUI.changed = true;
    }

    private void OnGUI()
    {
        EDLayout.CreateScrollView(ref ScPos, "box", position.width, position.height, (width, height) => {

            EDTypeField.CreateLableField("", "------------ UpdateSystem ------------", width, 30);
            ShowSystemList(true, width);

            EDTypeField.CreateLableField("", "------------ FixedSystem ------------", width, 30);
            ShowSystemList(false, width);
        });
    }

    private void OnDestroy()
    {
        SaveJson();
    }

    private void ShowSystemList(bool isUpdate,float width)
    {
        List<SortJson> sorts = isUpdate == true ? MSystemSortJson.UpdateList : MSystemSortJson.FixedUpdateList;
        sorts.Sort((left, right) =>
        {
            if (left.Sort == right.Sort)
                return 0;
            else if (left.Sort < right.Sort)
                return -1;
            else
                return 1;
        });
        for (int i = 0; i < sorts.Count; i++)
        {
            string sysName = sorts[i].TypeName;
            string showName= sysName+ string.Format("（{0}）", sorts[i].Sort);
            EDButton.CreateBtn(showName, width, 30, () =>
            {
                EDPopMenu.CreatePopMenu(new List<string>() { "上升", "下降" }, (int sel) =>
                {
                    if (sel == 0)
                    {
                        UPSystem(sysName, isUpdate);
                    }
                    else if (sel == 1)
                    {
                        DOWNSystem(sysName, isUpdate);
                    }
                });
            });
        }
    }

    private void UpdateSystemJson()
    {
        List<SortJson> upJson = new List<SortJson>();
        List<string> upList = GetSystemList(true);
        for (int i = 0; i < upList.Count; i++)
        {
            int sort = GetSystemSort(upList[i],true);
            upJson.Add(new SortJson() {
                TypeName= upList[i],
                Sort= sort,
            });
        }

        List<SortJson> fixJson = new List<SortJson>();
        List<string> fixList = GetSystemList(false);
        for (int i = 0; i < fixList.Count; i++)
        {
            int sort = GetSystemSort(fixList[i], false);
            fixJson.Add(new SortJson()
            {
                TypeName = fixList[i],
                Sort = sort,
            });
        }

        MSystemSortJson.UpdateList = upJson;
        MSystemSortJson.FixedUpdateList = fixJson;
    }

    private List<string> GetSystemList(bool isUpdate)
    {
        List<string> list = new List<string>();
        List<Type> systemTypes = EDReflectHelp.GetAllClassByClass<BaseSystem>();
        for (int i = 0; i < systemTypes.Count; i++)
        {
            Type sysTy = systemTypes[i];
            SystemAttribute attribute = ReflectHelp.GetTypeAttr<SystemAttribute>(sysTy);
            if(isUpdate)
            {
                if (attribute == null || attribute.InFixedUpdate == false)
                {
                    list.Add(sysTy.FullName);
                }
            }
            else
            {
                if (attribute != null && attribute.InFixedUpdate == true)
                {
                    list.Add(sysTy.FullName);
                }
            }
        }
        return list;
    }

    private int GetSystemSort(string systemName, bool isUpdate)
    {
        List<SortJson> sorts = isUpdate == true ? MSystemSortJson.UpdateList : MSystemSortJson.FixedUpdateList;
        for (int i = 0; i < sorts.Count; i++)
        {
            if (sorts[i].TypeName== systemName)
            {
                return sorts[i].Sort;
            }
        }

        SortJson sortJson = new SortJson()
        {
            TypeName = systemName,
        };
        sorts.Add(sortJson);
        return sortJson.Sort;
    }

    private void UPSystem(string systemName, bool isUpdate)
    {
        List<SortJson> sorts = isUpdate == true ? MSystemSortJson.UpdateList : MSystemSortJson.FixedUpdateList;
        for (int i = 0; i < sorts.Count; i++)
        {
            if (sorts[i].TypeName == systemName)
            {
                sorts[i].Sort--;
                break;
            }
        }
        GUI.changed = true;
    }

    private void DOWNSystem(string systemName, bool isUpdate)
    {
        List<SortJson> sorts = isUpdate == true ? MSystemSortJson.UpdateList : MSystemSortJson.FixedUpdateList;
        for (int i = 0; i < sorts.Count; i++)
        {
            if (sorts[i].TypeName == systemName)
            {
                sorts[i].Sort++;
                break;
            }
        }
        GUI.changed = true;
    }

    private void SaveJson()
    {
        string jsonData = LitJson.JsonMapper.ToJson(MSystemSortJson);
        EDTool.WriteText(jsonData, Application.dataPath + JsonPath);
        AssetDatabase.Refresh();
    }
}