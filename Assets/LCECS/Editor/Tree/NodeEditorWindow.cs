using LCECS.Core.Tree;
using LCECS.Core.Tree.Base;
using LCECS.Core.Tree.Nodes;
using LCECS.Core.Tree.Nodes.Action;
using LCECS.Core.Tree.Nodes.Control;
using LCECS.Data;
using LCECS.Help;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LCECS.Tree
{
    /// <summary>
    /// 节点编辑器
    /// 1，通过数据来显示对应的节点
    /// 2，绘制节点
    /// 3，保存数据
    /// 4，派发操作事件
    /// </summary>
    public class NodeEditorWindow : EditorWindow
    {
        private const string DecPath = "/"+ECSDefinitionPath.DecTreePath;
        private const string BevPath = "/"+ECSDefinitionPath.BevTreePath;
        private static DecTrees MDecTrees;
        private static BevTrees MBevTrees;
        private static bool ShowDec = true;

        private static float TimeRefreshRunningNode = 0.1f;
        private static float Timer;

        [MenuItem("LCECS/决策及行为树编辑")]
        private static void OpenWindow()
        {
            if (EDTool.CheckFileInPath(Application.dataPath + DecPath))
            {
                string dataJson = EDTool.ReadText(Application.dataPath + DecPath);
                MDecTrees = LitJson.JsonMapper.ToObject<DecTrees>(dataJson);
            }
            else
            {
                MDecTrees = new DecTrees();
            }

            if (EDTool.CheckFileInPath(Application.dataPath + BevPath))
            {
                string dataJson = EDTool.ReadText(Application.dataPath + BevPath);
                MBevTrees = LitJson.JsonMapper.ToObject<BevTrees>(dataJson);
            }
            else
            {
                MBevTrees = new BevTrees();
            }
            InitJson();
            
            NodeEditorWindow window = GetWindow<NodeEditorWindow>();
            window.titleContent = new GUIContent("决策及行为树编辑");

            if (EditorApplication.isPlaying == true)
            {
                Timer = Time.realtimeSinceStartup;
            }
        }

        private static void InitJson()
        {
            //决策
            DecTrees TempDecTrees =new DecTrees();
            TempDecTrees.NodeId = MDecTrees.NodeId;
            foreach (int group in Enum.GetValues(typeof(EntityDecGroup)))
            {
                string strName = Enum.GetName(typeof(EntityDecGroup),group);
                NodeDataJson oldJson = null;
                if (MDecTrees.EntityTrees.ContainsKey(strName))
                    oldJson = MDecTrees.EntityTrees[strName];
                if (oldJson==null)
                    oldJson= CreateNodeJson(NodeType.Root, typeof(NodeRoot).FullName, "根节点");
                TempDecTrees.EntityTrees.Add(strName, oldJson);
            }
            foreach (int group in Enum.GetValues(typeof(WorldDecGroup)))
            {
                string strName = Enum.GetName(typeof(WorldDecGroup),group);
                NodeDataJson oldJson = null;
                if (MDecTrees.WorldTrees.ContainsKey(strName))
                    oldJson = MDecTrees.WorldTrees[strName];
                if (oldJson == null)
                    oldJson = CreateNodeJson(NodeType.Root, typeof(NodeRoot).FullName, "根节点");
                TempDecTrees.WorldTrees.Add(strName, oldJson);
            }
            MDecTrees = TempDecTrees;

            //行为
            BevTrees TempBevTrees = new BevTrees();
            TempBevTrees.NodeId = MBevTrees.NodeId;
            foreach (int group in Enum.GetValues(typeof(EntityReqId)))
            {
                string strName = Enum.GetName(typeof(EntityReqId),group);
                NodeDataJson oldJson = null;
                if (MBevTrees.EntityTrees.ContainsKey(strName))
                    oldJson = MBevTrees.EntityTrees[strName];
                if (oldJson == null)
                    oldJson = CreateNodeJson(NodeType.Root, typeof(NodeRoot).FullName, "根节点");
                TempBevTrees.EntityTrees.Add(strName, oldJson);
            }
            foreach (int group in Enum.GetValues(typeof(WorldReqId)))
            {
                string strName = Enum.GetName(typeof(WorldReqId),group);
                NodeDataJson oldJson = null;
                if (MBevTrees.WorldTrees.ContainsKey(strName))
                    oldJson = MBevTrees.WorldTrees[strName];
                if (oldJson == null)
                    oldJson = CreateNodeJson(NodeType.Root, typeof(NodeRoot).FullName, "根节点");
                TempBevTrees.WorldTrees.Add(strName, oldJson);
            }
            MBevTrees = TempBevTrees;
        }
        
        //背景格偏移
        private Vector2 offset;
        //背景格范围
        private Rect bgGridRect;
        //鼠标拖拽
        private Vector2 drag;
        //左侧滚动条
        private Vector2 leftPos = Vector2.zero;
        //当前鼠标位置
        private Vector2 curMousePos;
        
        private NodeDataJson selTree;
        
        private List<NodeEditor> showNodeEditor=new List<NodeEditor>();
        private NodeEditor selNode = null;
        
        //连接逻辑
        public static NodeEditor curConnectNode = null;

        private void Update()
        {
            if (Application.isPlaying)
            {
                float dlTime = Time.realtimeSinceStartup - Timer;
                if (dlTime >= TimeRefreshRunningNode)
                {
                    NodeRunSelEntityHelp.RefreshRunningNode();
                    Timer = Time.realtimeSinceStartup + dlTime;
                } 
            }
        }

        private void OnEnable()
        {
        
        }

        private void OnDestroy()
        {
            SaveJson();
            MDecTrees=null;
            MBevTrees=null;
            curConnectNode = null;
            ShowDec = true;
        }

        private void OnGUI()
        {
            EDLayout.CreateHorizontal("box", position.width, position.height, RefreshPanel);

            ProcessNodeEvents(Event.current);

            ProcessEvents(Event.current);

            curMousePos = Event.current.mousePosition;
            // if (GUI.changed)
            // {
            //     
            // }

            bgGridRect = new Rect(new Vector2(position.width * 0.25f, 0), new Vector2(position.width * 0.75f, position.height));
            Repaint();
        }
        
        private void ProcessNodeEvents(Event e)
        {
            for (int i = 0; i < showNodeEditor.Count; i++)
            {
                bool childChange = showNodeEditor[i].ProcessEvents(e);
                if (childChange)
                {
                    GUI.changed = true;
                }
            }
        }

        private void ProcessEvents(Event e)
        {
            drag = Vector2.zero;
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 1)
                    {
                        ShowCreateNodeMenu();
                        Debug.LogError("创建节点>>>>>>>");
                    }
                    else if (e.button == 0)
                    {
                        if (curConnectNode!=null)
                        {
                            curConnectNode = null;
                        }
                    }
                    break;
                case EventType.MouseDrag:
                    if (e.button == 0)
                    {
                        if (bgGridRect.Contains(e.mousePosition))
                        {
                            OnDrag(e.delta);
                        }
                    }
                    break;
            }
        }

        private void NodeEventCallBack(int index, NodeEditor node)
        {
            //选择节点
            if (index==-1)
            {
                selNode = node;
            }
            //连接父节点
            if (index==0)
            {
                if (curConnectNode==null)
                {
                    Debug.Log("连接中》》》》》");
                    curConnectNode = node;
                }
                else
                {
                    Debug.Log("连接》》》》》");
                    if (node.ConnectNode(curConnectNode))
                    {
                        curConnectNode = null;
                        GUI.changed = true;
                    }
                    
                }
                
            }
            //删除节点
            if (index==1)
            {
                //删除显示
                showNodeEditor.Remove(node);
                //删除数据
                if (node.ParEditor!=null)
                {
                    node.ParEditor.Json.ChildNodes.Remove(node.Json);
                }
            }
            //删除连接
            if (index==2)
            {
                //删除数据
                if (node.ParEditor == null)
                {
                    Debug.Log("没有连接》》》》");
                    return;
                }

                node.ParEditor.Json.ChildNodes.Remove(node.Json);
                node.ParEditor = null;
            }
        }

        private void OnDrag(Vector2 delta)
        {
            drag = delta;
            for (int i = 0; i < showNodeEditor.Count; i++)
            {
                showNodeEditor[i].Drag(delta);
            }
            GUI.changed = true;
        }
        
        private void RefreshPanel()
        {
            //左侧列表
            EDLayout.CreateVertical("box",position.width, position.height, () =>
            {
                //编辑界面切换
                EDLayout.CreateHorizontal("box",position.width,40,(() =>
                {
                    EDButton.CreateBtn("决策层",100,20,(() => { ShowDec = true; SaveJson(); showNodeEditor.Clear(); }));
                    
                    EDButton.CreateBtn("行为层",100,20,(() => { ShowDec = false; SaveJson(); showNodeEditor.Clear(); }));

                    DrawSelNodePremise(position.width-200, 40);
                }));

                //界面刷新
                EDLayout.CreateHorizontal("box", position.width, position.height-40, (() =>
                {
                    if (ShowDec)
                    {
                        DrawDecTreeList();
                    }
                    else
                    {
                        DrawBevTreeList();
                    }
                }));
            });
            
            //右侧节点
            RefreshRightPanel();
        }

        #region 左侧界面

        //决策层
        private void DrawDecTreeList()
        {
            //列表
            EDLayout.CreateScrollView(ref leftPos, "box", position.width*0.3f, position.height, () =>
            {
                EDTypeField.CreateLableField("********世界决策********","",position.width*0.3f,20);
                foreach (string group in MDecTrees.WorldTrees.Keys)
                {
                    NodeDataJson tree = MDecTrees.WorldTrees[group];
                    EDButton.CreateBtn(group, position.width * 0.28f, 25, () =>
                    {
                        SelTreeChange(tree);
                    });
                }
                
                EDTypeField.CreateLableField("********实体决策********","",position.width*0.3f,20);
                foreach (string group in MDecTrees.EntityTrees.Keys)
                {
                    NodeDataJson tree = MDecTrees.EntityTrees[group];
                    EDButton.CreateBtn(group, position.width * 0.28f, 25, () =>
                    {
                        SelTreeChange(tree);
                    });
                }
            });
        }

        //行为层
        private void DrawBevTreeList()
        {
            //列表
            EDLayout.CreateScrollView(ref leftPos, "box", position.width*0.3f, position.height, () =>
            {
                EDTypeField.CreateLableField("********世界行为********","",position.width*0.3f,20);
                foreach (string group in MBevTrees.WorldTrees.Keys)
                {
                    NodeDataJson tree = MBevTrees.WorldTrees[group];
                    EDButton.CreateBtn(group, position.width * 0.28f, 25, () =>
                    {
                        SelTreeChange(tree);
                    });
                }
                
                EDTypeField.CreateLableField("********实体行为********","",position.width*0.3f,20);
                foreach (string group in MBevTrees.EntityTrees.Keys)
                {
                    NodeDataJson tree = MBevTrees.EntityTrees[group];
                    EDButton.CreateBtn(group, position.width * 0.28f, 25, () =>
                    {
                        SelTreeChange(tree);
                    });
                }
            });
        }
        
        #endregion

        #region 右侧界面

        //画节点
        private void RefreshRightPanel()
        {
            if (selTree==null)
            {
                return;
            }
            
            EDLayout.DrawGrid(20, 0.2f, Color.gray, bgGridRect, ref offset,drag);
            BeginWindows();
            for (int i = 0; i < showNodeEditor.Count; i++)
            {
                showNodeEditor[i].Draw();
            }
            EndWindows();

            DrawConnectLine();
            //RefreshNodeLines();
        }

        //节点线
        private void RefreshNodeLines()
        {
            if (selTree==null)
            {
                return;
            }
            DrawNodeLine(selTree);
        }

        private void DrawNodeLine(NodeDataJson node)
        {
            NodeEditor parNode = GetNodeEditorById(node.GetHashCode());
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                NodeEditor childNode = GetNodeEditorById(node.ChildNodes[i].GetHashCode());
                EDLine.CreateBezierLine(parNode.mRect.position,childNode.mRect.position,10);

                DrawNodeLine(node.ChildNodes[i]);
            }
        }

        #endregion

        #region 节点前提
        private Vector2 nodePremisePos = Vector2.zero;
        private void DrawSelNodePremise(float width,float height)
        {
            if (selNode == null)
                return;
            if (selNode.Json.Premise == null)
                return;
            EDLayout.CreateScrollView(ref nodePremisePos, "box", width, height, () =>
            {
                NodePremiseJson premiseJson = selNode.Json.Premise;
                CreatePremiseBtn(premiseJson);
            });
        }

        private void CreatePremiseBtn(NodePremiseJson premiseJson)
        {
            if (premiseJson == null)
                return;
            EDButton.CreateBtn("(" + premiseJson.TrueValue.ToString() + ")"+premiseJson.Name +"("+ premiseJson.Type.ToString() + ")", 250, 40, () => {
                EDPopMenu.CreatePopMenu(new List<string> {"删除前提","改变真值" }, (int sel) =>
                {
                    if (sel==0)
                    {
                        RemovePremise(premiseJson);
                    }
                    if (sel == 1)
                    {
                        premiseJson.TrueValue = !premiseJson.TrueValue;
                    }

                });
            });

            if (premiseJson.OtherPremise != null)
            {
                CreatePremiseBtn(premiseJson.OtherPremise);
            }
        }

        private void RemovePremise(NodePremiseJson removeJson)
        {
            if (selNode == null || selNode.Json.Premise == null)
                return;
            NodePremiseJson parent = GetParentPremise(removeJson.Name, selNode.Json.Premise);
            if (parent==null)
            {
                selNode.Json.Premise = null;
            }
            else
            {
                parent.OtherPremise = null;
            }
        }

        private NodePremiseJson GetParentPremise(string typeName, NodePremiseJson json)
        {
            if (json == null)
                return null;
            if (json.OtherPremise == null)
                return null;
            if (json.OtherPremise.TypeFullName == typeName)
                return json;
            return GetParentPremise(typeName, json.OtherPremise);
        }

        private NodePremiseJson GetPremise(string typeName, NodePremiseJson json)
        {
            if (json == null)
                return null;
            if (json.TypeFullName == name)
                return json;
            if (json.OtherPremise==null)
                return null;
            return GetPremise(typeName, json.OtherPremise);
        }

        #endregion

        #region 渲染连接线

        private void DrawConnectLine()
        {
            if (curConnectNode==null)
            {
                return;
            }

            Vector2 mousePosition = Event.current.mousePosition;
            EDLine.CreateBezierLine(curConnectNode.mRect.center,mousePosition,2.5f,Color.gray);
            GUI.changed = true;
        }
        

        #endregion

        #region 创建节点菜单

        private void ShowCreateNodeMenu()
        {
            List<string> showStrs = new List<string>();
            
            //控制节点
            List<Type> controlTypes = EDReflectHelp.GetAllClassByClass<NodeControl>();
            for (int i = 0; i < controlTypes.Count; i++)
            {
                NodeAttribute nodeAttribute = ReflectHelp.GetTypeAttr<NodeAttribute>(controlTypes[i]);
                if (nodeAttribute != null)
                {
                    showStrs.Add("控制节点/"+nodeAttribute.ViewName);
                }
                else
                {
                    showStrs.Add("控制节点/"+controlTypes[i].FullName);
                }
                
            }
            
            //行为节点
            List<Type> actionTypes = EDReflectHelp.GetAllClassByClass<NodeAction>();
            List<Type> actionShowTypes = new List<Type>();
            for (int i = 0; i < actionTypes.Count; i++)
            {
                NodeAttribute nodeAttribute = ReflectHelp.GetTypeAttr<NodeAttribute>(actionTypes[i]);
                if (nodeAttribute==null)
                {
                    showStrs.Add("基础行为节点/" + nodeAttribute.ViewName);
                    actionShowTypes.Add(actionTypes[i]);
                }
                else
                {
                    if (nodeAttribute.IsCommonAction)
                    {
                        showStrs.Add("基础行为/" + nodeAttribute.ViewName);
                        actionShowTypes.Add(actionTypes[i]);
                    }
                    else
                    {
                        if (ShowDec)
                        {
                            if (nodeAttribute.IsBevNode == false)
                            {
                                showStrs.Add("扩展行为/" + nodeAttribute.ViewName);
                                actionShowTypes.Add(actionTypes[i]);
                            }     
                        }
                        else
                        {
                            if (nodeAttribute.IsBevNode)
                            {
                                showStrs.Add("扩展行为/" + nodeAttribute.ViewName);
                                actionShowTypes.Add(actionTypes[i]);
                            }
                        }
                    }
                }
            }

            EDPopMenu.CreatePopMenu(showStrs, (int index) =>
            {
                Debug.Log("创建节点》》》》》》》》》"+index+"  "+controlTypes.Count+"   >>"+ actionShowTypes.Count);
                //创建控制
                if (index<=controlTypes.Count-1)
                {
                    NodeDataJson node = CreateNodeJson(NodeType.Control, controlTypes[index].FullName,showStrs[index],curMousePos);
                    //创建显示
                    NodeEditor nodeEditor=new NodeEditor(new Rect(new Vector2((float)node.PosX,(float)node.PosY), new Vector2(200,100)),node,null,NodeEventCallBack);
                    showNodeEditor.Add(nodeEditor);
                    GUI.changed = true;
                }
                //创建行为
                else
                {
                    Debug.Log("创建行为节点》》》》》》》》》"+(index-controlTypes.Count));
                    NodeDataJson node = CreateNodeJson(NodeType.Action, actionShowTypes[index-controlTypes.Count].FullName, showStrs[index],curMousePos);
                    //创建显示
                    NodeEditor nodeEditor=new NodeEditor(new Rect(new Vector2((float)node.PosX,(float)node.PosY), new Vector2(200,100)),node,null,NodeEventCallBack);
                    showNodeEditor.Add(nodeEditor);
                    GUI.changed = true;
                }
            });
        }
        
        #endregion
        
        private void SelTreeChange(NodeDataJson tree)
        {
            selTree = tree;
            showNodeEditor.Clear();
            //创建显示
            NodeEditor nodeEditor=new NodeEditor(new Rect(new Vector2((float)tree.PosX, (float)tree.PosY), new Vector2(200,100)),tree,null,NodeEventCallBack);
            showNodeEditor.Add(nodeEditor);
            CreateNodeEditor(nodeEditor);
        }

        private void CreateNodeEditor(NodeEditor node)
        {
            for (int i = 0; i < node.Json.ChildNodes.Count; i++)
            { 
                NodeDataJson childNode = node.Json.ChildNodes[i];
                NodeEditor tmpEditor=new NodeEditor(new Rect(new Vector2((float)childNode.PosX,(float)childNode.PosY), new Vector2(200,100)),childNode,node,NodeEventCallBack);
                showNodeEditor.Add(tmpEditor);
                CreateNodeEditor(tmpEditor);
            }
        }

        private NodeEditor GetNodeEditorById(int id)
        {
            for (int i = 0; i < showNodeEditor.Count; i++)
            {
                if (showNodeEditor[i].MId==id)
                {
                    return showNodeEditor[i];
                }
            }
            Debug.LogError("没有找到指定的节点>>>>"+id);
            return null;
        }
        
        private static NodeDataJson CreateNodeJson(NodeType nodeType,string fullName,string name="",Vector2 pos=default)
        {
            if (pos==default)
            {
                pos = new Vector2(300, 400);
            }

            int nodeId = 0;
            if (ShowDec)
            {
                nodeId = MDecTrees.NodeId;
                MDecTrees.NodeId++;
            }
            else
            {
                nodeId = MBevTrees.NodeId;
                MBevTrees.NodeId++;
            }
            NodeDataJson node=new NodeDataJson(nodeId,nodeType,fullName,pos.x,pos.y,name);;
            return node;
        }
        
        private void SaveJson()
        {
            SaveBevTreeJson();
            SaveDecTreeJson();
        }

        private void SaveBevTreeJson()
        {
            string jsonData = LitJson.JsonMapper.ToJson(MBevTrees);
            EDTool.WriteText(jsonData,Application.dataPath+BevPath);
            AssetDatabase.Refresh();
        }
        
        private void SaveDecTreeJson()
        {
            string jsonData = LitJson.JsonMapper.ToJson(MDecTrees);
            EDTool.WriteText(jsonData,Application.dataPath+DecPath);
            AssetDatabase.Refresh();
        }
    }
}
