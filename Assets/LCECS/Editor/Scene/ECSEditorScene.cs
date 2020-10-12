using System.Collections.Generic;
using System.Reflection;
using LCECS.Core.ECS;
using LCECS.Help;
using LCHelp;
using UnityEditor;
using UnityEngine;

namespace LCECS.Scene
{
    /// <summary>
    /// 组件键值预览
    /// </summary>
    public class ComKeyValueView
    {
        public string KeyName;
        
        public FieldInfo Info;

        public BaseCom Com;

        public ComKeyValueView(string name, FieldInfo info,BaseCom com)
        {
            KeyName = name;
            Info = info;
            Com = com;
        }
    }
    
    /// <summary>
    /// 组件预览
    /// </summary>
    public class EntityComView
    {
        public string ComName;
        public List<ComKeyValueView> ValueList = new List<ComKeyValueView>();
        public List<ComKeyValueView> EditorValueList = new List<ComKeyValueView>();
        
        public EntityComView(string comName, FieldInfo[] fieldInfos, BaseCom baseCom)
        {
            ComName = comName;
            ComAttribute comAttribute = LCReflect.GetTypeAttr<ComAttribute>(baseCom.GetType());
            if (comAttribute!=null)
            {
                ComName = comAttribute.ViewName;
            }
            for (int i = 0; i < fieldInfos.Length; i++)
            {
                FieldInfo info = fieldInfos[i];
                ComKeyValueView keyValue = new ComKeyValueView(info.Name, info,baseCom);
                
                ComValueAttribute comValueAttribute = LCReflect.GetFieldAttr<ComValueAttribute>(info);
                if (comValueAttribute!=null)
                {
                    if (comValueAttribute.ViewEditor)
                    {
                        EditorValueList.Add(keyValue);
                    }
                    else
                    {
                        if (comValueAttribute.ShowView)
                        {
                            ValueList.Add(keyValue);
                        }
                    }
                }
                
            }
        }
    }
    
    /// <summary>
    /// ECS 场景视图
    /// </summary>
    public class ECSEditorScene
    {
        [InitializeOnLoadMethod]  //unity初始化时调用
        private static void Init()
        {
            SceneView.onSceneGUIDelegate += OnSceneGUI;
        }
        
        public static EntityEditorViewHelp SelEntityView = null;
        private static Vector2 ScrollPos=Vector2.zero;
        private static Vector2 SystemScrollPos=Vector2.zero;

        private static bool ShowComs=false;
        private static bool ShowSystems=false;

        private static void OnSceneGUI(SceneView sceneView)
        {
            GameObject go = Selection.activeGameObject;
            if (go!=null)
            {
                EntityEditorViewHelp entityView = go.GetComponent<EntityEditorViewHelp>();
                if (entityView!=null)
                {
                    SelEntityView = entityView;
                }
            }

            if (SelEntityView!=null&& SelEntityView.DrawEditor)
            {
                DrawEntity(SelEntityView);
            }
        }
        
        private static void DrawEntity(EntityEditorViewHelp targetCom)
        {
            Entity entity = ECSLocate.ECS.GetEntity(targetCom.EntityId);
            if (entity == null)
                return;
            
            List<EntityComView> comViews = HandleEntityComs(entity);
            Rect showRect = targetCom.ShowRect;
            
            //渲染
            Handles.BeginGUI();
            GUILayout.BeginArea(showRect);
            
            //基础按钮和信息
            EDTypeField.CreateLableField("实体名：",targetCom.name,showRect.width,20);
            EDTypeField.CreateLableField("实体状态：", entity.IsEnable.ToString(), showRect.width, 20);
            EDButton.CreateBtn("激活/禁用 实体", 100, 50, () => {
                if (entity.IsEnable)
                    entity.Disable();
                else
                    entity.Enable();
            });
            EDButton.CreateBtn("显示组件",100,50,()=>{
                ShowComs=ShowComs?false:true;
            });
            EDButton.CreateBtn("显示系统",100,50,()=>{
                ShowSystems=ShowSystems?false:true;
            });

            //组件列表
            DrawEntityComs(ShowComs,comViews, showRect.width, showRect.height*0.7f);

            //系统列表
            DrawEntitySystem(ShowSystems,entity.Systems, showRect.width, showRect.height * 0.7f);
            
            GUILayout.EndArea();
            Handles.EndGUI();
        }


        /// <summary>
        /// 渲染实体组件列表
        /// </summary>
        private static void DrawEntityComs(bool isShow,List<EntityComView> comViews,float width,float height)
        {
            if (isShow==false)
            {
                return;
            }
            EDLayout.CreateScrollView(ref ScrollPos, "GroupBox", width, height, () =>
            {
                for (int i = 0; i < comViews.Count; i++)
                {
                    EntityComView comView = comViews[i];
                    EditorGUILayout.Space();
                    GUI.color= Color.green;
                    //组件名
                    EditorGUILayout.LabelField("组件:",comView.ComName,GUILayout.Width(width), GUILayout.Height(20));
            
                    //可编辑键值
                    for (int j = 0; j < comView.EditorValueList.Count; j++)
                    {
                        GUI.color= Color.red;
                        ComKeyValueView info = comView.EditorValueList[j];
                        object valueObj = info.Info.GetValue(info.Com);
                        EDTypeField.CreateTypeField(info.KeyName+"= ",ref valueObj,info.Info.FieldType, width, 20);
                        LCReflect.SetTypeFieldValue(info.Com,info.KeyName,valueObj);
                    }
                    
                    //只读值
                    for (int j = 0; j < comView.ValueList.Count; j++)
                    {
                        GUI.color= Color.white;
                        ComKeyValueView info = comView.ValueList[j];
                        object valueObj = info.Info.GetValue(info.Com);
                        EDTypeField.CreateLableField(info.KeyName+"= ",valueObj.ToString(), width, 20);
                    }
                }
            });
        }

        /// <summary>
        /// 渲染实体观察系统列表
        /// </summary>
        private static void DrawEntitySystem(bool isShow,List<string> systems, float width, float height)
        {
            if (isShow==false)
            {
                return;
            }

            EDLayout.CreateScrollView(ref SystemScrollPos,"GroupBox", width, height, () =>
            {
                for (int i = 0; i < systems.Count; i++)
                {
                    string systemName = systems[i];
                    EditorGUILayout.Space();
                    GUI.color= Color.green;
                    //组件名
                    EditorGUILayout.LabelField("观察系统:",systemName,GUILayout.Width(width), GUILayout.Height(20));
                }
            });
        }

        /// <summary>
        /// 处理实体组件
        /// </summary>
        private static List<EntityComView> HandleEntityComs(Entity entity)
        {
            List<EntityComView> comViews=new List<EntityComView>();
            //组件名
            HashSet<string> entityComs = entity.GetAllComStr();
            foreach (string comName in entityComs)
            {
                BaseCom com = entity.GetCom(comName);
                FieldInfo[] fields = LCReflect.GetTypeFieldInfos(com.GetType());

                EntityComView comView = new EntityComView(comName,fields,com);

                comViews.Add(comView);
            }

            return comViews;
        }
    }
}
