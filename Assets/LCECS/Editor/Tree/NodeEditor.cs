
using UnityEngine;
using LCECS.Help;
using System.Collections.Generic;
using System;
using System.Reflection;
using LCECS.Core.Tree.Base;
using LCECS.Data;

namespace LCECS.Tree
{

    /// <summary>
    /// 节点编辑
    /// 1，绘制节点
    /// 2，响应操作事件
    /// 3，编辑节点数据
    /// </summary>
    public class NodeEditor
    {
        private int Id;

        public int MId
        {
            get { return Id; }
        }
        
        private string Name;
        
        private Action<int, NodeEditor> CallBack;
        
        public Rect mRect;
        
        public NodeDataJson Json;
        
        public NodeEditor ParEditor;

        private Vector2 valuePos;

        private bool IsRunning;

        public NodeEditor(Rect rect,NodeDataJson json,NodeEditor parEditor,Action<int,NodeEditor> processCallBack)
        {
            Id = json.NodeId;
            Name  = json.Name;
            mRect = rect;
            Json  = json;
            ParEditor = parEditor;
            
            GetEditorNodeKeyValue();
            CallBack = processCallBack;

            IsRunning = false;
        }

        public void Draw()
        {
            IsRunning = NodeRunSelEntityHelp.CheckIsRunningNode(MId);
            if (IsRunning)
            {
                GUI.color = Color.green;
            }
            else
            {
                GUI.color = Color.white;
            }
            GUI.Window(Id, mRect, DrawNodeWindow, Name);
            if (ParEditor!=null)
            {
                EDLine.CreateBezierLine(mRect.center,ParEditor.mRect.center,2.5f,Color.gray);
            }

            Json.PosX = mRect.position.x;
            Json.PosY = mRect.position.y;
        }

        public bool ProcessEvents(Event e)
        {
            bool isClick = mRect.Contains(e.mousePosition);
            if (isClick==false)
            {
                return false;
            }

            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 1)
                    {
                        DrawPopMenu();
                        e.Use();
                        return true;
                    }
                    else if (e.button == 0)
                    {
                        if (NodeEditorWindow.curConnectNode!=null)
                        {
                            CallBack(0, this);
                        }
                        else
                        {
                            CallBack(-1, this);
                        }
                    }
                    break;
                case EventType.MouseDrag:
                    if (e.button == 0)
                    {
                        Drag(e.delta);
                        e.Use();
                        return true;
                    }
                    break;
            }

            return false;
        }

        public void Drag(Vector2 delta)
        {
            mRect.position += delta;
        }
        
        private void DrawNodeWindow(int id)
        {
            

            //前提
            if (Json.Premise==null)
                EDTypeField.CreateLableField("无节点前提:", "", mRect.width, 20);
            else
            {
                Type premiseType = EDReflectHelp.GetTypeByFullName(Json.Premise.TypeFullName);
                if (premiseType == null)
                {
                    Json.Premise = null;
                }
                else
                {
                    EDTypeField.CreateLableField("节点前提:" + Json.Premise.Name + "...", "", mRect.width, 20);
                }
            }

            //信息
            EDTypeField.CreateLableField("节点类型:"+Json.Type.ToString(),"",mRect.width,20);
            EDTypeField.CreateLableField("节点ID:" + MId, "", mRect.width, 20);
            DrawEditorNodeKeyValue();
        }
        
        private void DrawEditorNodeKeyValue()
        {
            EDLayout.CreateScrollView(ref valuePos,"",mRect.width,mRect.height,((width, height) =>
            {
                //可编辑键值
                for (int j = 0; j < Json.KeyValues.Count; j++)
                {
                    NodeKeyValue keyValue = Json.KeyValues[j];

                    object value = ReflectHelp.StrChangeToObject(keyValue.Value, keyValue.TypeFullName);
                    Type ty = ReflectHelp.GetType(keyValue.TypeFullName);
                    EDTypeField.CreateTypeField(keyValue.KeyName+"= ",ref value, ty, width-10,20);

                    keyValue.Value = value.ToString();
                }
            }));
        }

        private void DrawPopMenu()
        {
            List<string> showStrs = new List<string>()
            {
                "连接父节点",
                "删除节点",
                "删除连接",
            };

            //前提条件
            List<Type> premiseTypes = EDReflectHelp.GetAllClassByClass<NodePremise>();
            List<Type> premiseShowTypes = new List<Type>();
            for (int i = 0; i < premiseTypes.Count; i++)
            {
                if (GetPremise(premiseTypes[i].FullName, Json.Premise) != null)
                    continue;
                NodePremiseAttribute nodeAttribute = ReflectHelp.GetTypeAttr<NodePremiseAttribute>(premiseTypes[i]);
                if (nodeAttribute == null || nodeAttribute.GroupName=="")
                {
                    showStrs.Add("添加前提/默认前提/" + nodeAttribute.ViewName);
                    premiseShowTypes.Add(premiseTypes[i]);
                }
                else
                {
                    showStrs.Add("添加前提/" + nodeAttribute.GroupName +"/"+ nodeAttribute.ViewName);
                    premiseShowTypes.Add(premiseTypes[i]);
                }
            }

            EDPopMenu.CreatePopMenu(showStrs, (int index) =>
            {
                Debug.Log("CreatePopMenu》》"+ index);
                if (index==2 && ParEditor==null)
                {
                    Debug.Log("没有父节点》》无法删除！！");
                    return;
                }
                
                if (index==0 && ParEditor!=null)
                {
                    Debug.Log("已经有父节点》》无法连接！！");
                    return;
                }
                if (index<=2)
                {
                    CallBack(index, this);
                }
                else
                {
                    index = index - 3;
                    NodePremiseJson nullPremise = GetNullPremise(Json.Premise);

                    string viewName = "";
                    Type premiseType = premiseShowTypes[index];
                    NodePremiseAttribute nodeAttribute = ReflectHelp.GetTypeAttr<NodePremiseAttribute>(premiseType);
                    if (nodeAttribute == null)
                        viewName = premiseType.FullName;
                    else
                        viewName = nodeAttribute.ViewName;
                    NodePremiseJson addJson = new NodePremiseJson();
                    addJson.Name = viewName;
                    addJson.Type = Core.Tree.PremiseType.AND;
                    addJson.TypeFullName = premiseType.FullName;

                    if (nullPremise==null)
                    {
                        Json.Premise = addJson;
                    }
                    else
                    {
                        nullPremise.OtherPremise = addJson;
                    }
                    
                }

            });
        }

        private void GetEditorNodeKeyValue()
        {
            Type nodeType = EDReflectHelp.GetTypeByFullName(Json.TypeFullName);
            
            FieldInfo[] fields = ReflectHelp.GetTypeFieldInfos(nodeType);
            
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo info = fields[i];
                NodeValueAttribute nodeValueAttribute = ReflectHelp.GetFieldAttr<NodeValueAttribute>(info);
                if (nodeValueAttribute!=null)
                {
                    if (nodeValueAttribute.ViewEditor)
                    {
                        object defauleValue = ReflectHelp.GeTypeDefaultFieldValue(Json.TypeFullName, info.Name);
                        UpdateNodeKeyValue(info, defauleValue);
                    }
                }
            }
        }

        private void UpdateNodeKeyValue(FieldInfo info, object defauleValue)
        {
            for (int i = 0; i < Json.KeyValues.Count; i++)
            {
                if (Json.KeyValues[i].KeyName==info.Name)
                {
                    return;
                }
            }
            NodeKeyValue view=new NodeKeyValue(info.Name,info.FieldType.FullName, defauleValue.ToString());
            Json.KeyValues.Add(view);
        }

        private NodePremiseJson GetPremise(string typeName, NodePremiseJson json)
        {
            if (json == null)
                return null;
            if (json.TypeFullName == typeName)
                return json;
            if (json.OtherPremise == null)
                return null;
            return GetPremise(typeName, json.OtherPremise);
        }

        private NodePremiseJson GetNullPremise(NodePremiseJson json)
        {
            if (json == null)
                return null;
            if (json.OtherPremise == null)
                return json;
            return GetNullPremise(json);
        }

        #region 公共方法

        public bool ConnectNode(NodeEditor node)
        {
            if (node.MId==MId)
            {
                Debug.Log("不能连接自身》》》》》");
                return false;
            }

            if (Json.ChildMaxCnt <= 0 || Json.ChildNodes.Count >= Json.ChildMaxCnt)
            {
                Debug.Log("节点数量不够》》》》》");
                return false;
            }
            
            Json.ChildNodes.Add(node.Json);
            node.ParEditor = this;
            return true;
        }

        #endregion
    }
}
