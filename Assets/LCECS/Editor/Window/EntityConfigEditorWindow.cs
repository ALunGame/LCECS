using LCECS.Core.ECS;
using LCECS.Data;
using LCECS.Help;
using LCHelp;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace LCECS.Window
{
    public class EntityConfigEditorWindow:EditorWindow
    {

        private EntityJson SelEntity;
        private bool ShowEntityList = true;
        private Editor EntityEditor;
        private Type SelCom;
        
        private Vector2 LeftPos = Vector2.zero;
        private const string EntityJsonPath   =  "/"+ECSDefinitionPath.EntityJsonPath;
        private static EntityJsonList MEntityJsonList=new EntityJsonList();
        
       
        [MenuItem("LCECS/实体编辑")]
        public static void OpenWindow()
        {
            EntityConfigEditorWindow window = GetWindow<EntityConfigEditorWindow>("实体编辑器");
            window.minSize = new Vector2(800, 600);
            
            if (EDTool.CheckFileInPath(Application.dataPath+EntityJsonPath))
            { 
                string dataJson = EDTool.ReadText(Application.dataPath+EntityJsonPath);
                MEntityJsonList  = LitJson.JsonMapper.ToObject<EntityJsonList>(dataJson);
            }
        }

        private void OnGUI()
        {
            EDLayout.CreateVertical("box",position.width, position.height, () =>
            {
                //界面刷新
                EDLayout.CreateHorizontal("box", position.width, position.height-20, (() =>
                {
                    DrawEntityJsonList();
                }));
            });
        }

        private void OnDestroy()
        {
            SaveEntityJsonList();
        }

        #region 实体组件配置界面

        //渲染实体列表
        private void DrawEntityJsonList()
        {
            //实体列表
            EDLayout.CreateScrollView(ref LeftPos, "box", position.width*0.2f, position.height, (float width,float height) =>
            {
                for (int i = 0; i < MEntityJsonList.List.Count; i++)
                {
                    EntityJson entity = MEntityJsonList.List[i];
                    EDButton.CreateBtn(entity.EntityName, width, 25, () =>
                    {
                        SelEntityChange(entity);
                    });
                }

                //添加实体
                EDButton.CreateBtn("新建实体", width, 30, () =>
                {
                    EDPopPanel.PopWindow("输入实体名>>>>>>", (x) =>
                    {
                        if (CheckContainEntity(x))
                        {
                            Debug.LogError("实体名重复>>>>>>>>>>>>"+x);
                            return;
                        }
                        EntityJson json=new EntityJson();
                        json.EntityName = x;

                        //默认添加Go组件
                        EntityComJson goComJson = new EntityComJson()
                        {
                            ComName = "LCECS.Core.ECS.GameObjectCom",
                        };
                        json.Coms.Add(goComJson);

                        MEntityJsonList.List.Add(json);
                        SelEntityChange(json);
                    });
                });
            });

            //编辑实体
            EDLayout.CreateVertical("box", position.width * 0.4f, position.height, (float width, float height) =>
            {
                if (SelEntity!=null)
                {
                    ShowSelEntityWindow(width);
                }
            });

            //实体预览
            EDLayout.CreateVertical("box", position.width * 0.3f, position.height, (float width, float height) =>
            {
                if (EntityEditor!=null)
                {
                    EntityEditor.DrawPreview(GUILayoutUtility.GetRect(300, 200));
                }

                //实体编辑
                if (SelCom!=null)
                {
                    ShowSelComWindow(width, position.height*0.5f);
                }
            });
            
        }

        private Vector2 ComListPos = Vector2.zero; 
        //实体编辑界面
        private void ShowSelEntityWindow(float width)
        {
            //基础配置只读
            EDTypeField.CreateLableField("实体名：",SelEntity.EntityName,width,20);

            object tipobj = SelEntity.TipStr;
            EDTypeField.CreateTypeField("描述信息：",ref tipobj,typeof(string),width,20);
            SelEntity.TipStr = tipobj.ToString();
            
            //决策分组
            SelEntity.Group = (EntityDecGroup)EditorGUILayout.EnumPopup("实体决策分组", SelEntity.Group);
            EditorGUILayout.Space();

            //预制体路径
            EditorGUILayout.LabelField("选择预制体：");
            GameObject prefab = null;
            prefab = EditorGUILayout.ObjectField(prefab, typeof(GameObject), false) as GameObject;
            if (prefab != null)
                SelEntity.PrefabPath = AssetDatabase.GetAssetPath(prefab);
            EditorGUILayout.LabelField("预制体路径：");
            EditorGUILayout.LabelField(SelEntity.PrefabPath,"","box");
            EditorGUILayout.Space();

            //组件列表
            EditorGUILayout.LabelField("组件列表：");
            EDLayout.CreateScrollView(ref ComListPos, "box", width, 150,()=> {
                for (int i = 0; i < SelEntity.Coms.Count; i++)
                {
                    //按钮名
                    string btnName = SelEntity.Coms[i].ComName;
                    Type comType = LCReflect.GetType(SelEntity.Coms[i].ComName);
                    if (comType != null)
                    {
                        ComAttribute comAttribute = LCReflect.GetTypeAttr<ComAttribute>(comType);
                        if (comAttribute != null)
                        {
                            btnName = comAttribute.ViewName;
                        }
                    }
                    else
                    {
                        SelEntity.Coms.RemoveAt(i);
                        continue;
                    }

                    EDButton.CreateBtn(btnName, width * 0.8f, 20, () =>
                    {
                        int comIndex = i;
                        EDPopMenu.CreatePopMenu(new List<string>() { "编辑组件","删除组件" }, (int index) =>
                        {
                            if (index==0)
                            {
                                SelCom = LCReflect.GetType(SelEntity.Coms[comIndex].ComName);
                            }
                            else if (index==1)
                            {
                                SelEntity.Coms.RemoveAt(comIndex);
                            }
                        });
                    });
                    EditorGUILayout.Space();
                }
            });

            //添加组件
            EDButton.CreateBtn("添加组件", 200, 50, () =>
            {
                List<Type> comTyps = LCReflect.GetClassByType<BaseCom>();
                List<string> showList = new List<string>();
                List<Type> showTyps = new List<Type>();
                for (int j = 0; j < comTyps.Count; j++)
                {
                    if (CheckContainCom(comTyps[j].FullName))
                    {
                        continue;
                    }
                    showTyps.Add(comTyps[j]);
                    ComAttribute comAttribute = LCReflect.GetTypeAttr<ComAttribute>(comTyps[j]);
                    if (comAttribute!=null)
                    {
                        if (comAttribute.GroupName=="")
                        {
                            comAttribute.GroupName="Default";
                        }
                        showList.Add(comAttribute.GroupName+"/"+comAttribute.ViewName);
                    }
                    else
                    {
                        showList.Add(comTyps[j].FullName);
                    }
                }

                EDPopMenu.CreatePopMenu(showList, (int index) =>
                {
                    Type comType = showTyps[index];
                    if (CheckContainCom(comType.FullName))
                    {
                        Debug.LogError("重复的组件>>>>>>>>"+comType.FullName);
                        return;
                    }

                    EntityComJson comJson = new EntityComJson()
                    {
                        ComName= comType.FullName,
                    };
                    SelEntity.Coms.Add(comJson);
                });
            });
            
            //保存配置
            EDButton.CreateBtn("保存配置", 200, 50, SaveEntityJsonList);
            
            //删除实体
            EDButton.CreateBtn("删除实体", 200, 30, () =>
            {
                EDDialog.CreateDialog("删除","确认删除该实体吗?",(() =>
                {
                    MEntityJsonList.List.Remove(SelEntity);
                    SelEntityChange(null);
                }));
                
            });
        }

        private Vector2 ComValuePos = Vector2.zero;
        //组件编辑界面
        private void ShowSelComWindow(float width,float height)
        {
            List<EntityComValueJson> values = UpdateSelComEditorValues();
            EDLayout.CreateScrollView(ref ComValuePos, "Box", width, height, () =>
            {
                for (int i = 0; i < values.Count; i++)
                {
                    EntityComValueJson comView = values[i];
                    Type resType = null;
                    resType = LCReflect.GetType(comView.Type);
                    if (resType == null)
                        resType = LCReflect.GetType(comView.Type);
                    object value = LCConvert.StrChangeToObject(comView.Value, resType.FullName);
                    EDTypeField.CreateTypeField(comView.Name + "= ", ref value, resType, width, 40);
                    SetSelComJson(comView.Name, value.ToString());
                }
            });
        }

        //选择实体改变
        private void SelEntityChange(EntityJson entity)
        {
            if (entity==null)
            {
                SelEntity = null;
                SelCom = null;
                EntityEditor = null;
                return;
            }
            SelEntity = entity;
            SelCom = null;
            GameObject selGo = AssetDatabase.LoadAssetAtPath<GameObject>(SelEntity.PrefabPath);
            EntityEditor = Editor.CreateEditor(selGo);
        }

        #endregion
        
        #region 数据处理
        
        //检测是否包含相同组件
        private bool CheckContainCom(string name)
        {
            if (SelEntity==null)
            {
                return false;
            }
            for (int i = 0; i < SelEntity.Coms.Count; i++)
            {
                string confName = SelEntity.Coms[i].ComName;
                if (confName==name)
                {
                    return true;
                }
            }
            return false;
        }
        
        //检测是否包含相同实体
        private bool CheckContainEntity(string name)
        {
            for (int i = 0; i < MEntityJsonList.List.Count; i++)
            {
                string confName = MEntityJsonList.List[i].EntityName;
                if (confName==name)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region 组件编辑数据处理

        private List<EntityComValueJson> GetSelComEditorValues()
        {
            FieldInfo[] fields = LCReflect.GetTypeFieldInfos(SelCom);

            List<EntityComValueJson> comValues = new List<EntityComValueJson>();

            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo info = fields[i];
                ComValueAttribute comValueAttribute = LCReflect.GetFieldAttr<ComValueAttribute>(info);
                if (comValueAttribute != null)
                {
                    if (comValueAttribute.ViewEditor)
                    {
                        comValues.Add(new EntityComValueJson()
                        {
                            Name = info.Name,
                            Type = info.FieldType.ToString(),
                            Value = "",
                        });
                    }
                }
            }
            return comValues;
        }

        private List<EntityComValueJson> UpdateSelComEditorValues()
        {
            List<EntityComValueJson> values = GetSelComEditorValues();

            EntityComJson comJson = null;
            for (int i = 0; i < SelEntity.Coms.Count; i++)
            {
                if (SelEntity.Coms[i].ComName==SelCom.FullName)
                {
                    comJson = SelEntity.Coms[i];
                    break;
                }
            }

            if (comJson==null)
            {
                return values;
            }

            //更新
            for (int i = 0; i < values.Count; i++)
            {
                for (int j = 0; j < comJson.Values.Count; j++)
                {
                    if (comJson.Values[j].Name == values[i].Name)
                    {
                        values[i].Value = comJson.Values[j].Value;
                        break;
                    }
                }
            }

            comJson.Values = values;
            return values;
        }

        private void SetSelComJson(string name,string value)
        {
            EntityComJson comJson = null;
            for (int i = 0; i < SelEntity.Coms.Count; i++)
            {
                if (SelEntity.Coms[i].ComName == SelCom.FullName)
                {
                    comJson = SelEntity.Coms[i];
                    break;
                }
            }

            for (int j = 0; j < comJson.Values.Count; j++)
            {
                if (comJson.Values[j].Name == name)
                {
                    comJson.Values[j].Value = value;
                    break;
                }
            }
        }

        #endregion

        //保存实体Json数据
        private void SaveEntityJsonList()
        {
            if (MEntityJsonList.List.Count==0)
            {
                return;   
            }
            string jsonData = LitJson.JsonMapper.ToJson(MEntityJsonList);
            EDTool.WriteText(jsonData,Application.dataPath+EntityJsonPath);
            AssetDatabase.Refresh();
        }
    }
}
