using LCECS.Core.Tree;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using LCECS.Core.Tree.Nodes;
using LCECS.Core.Tree.Nodes.Control;
using UnityEngine;

namespace LCECS.Data
{

    /// <summary>
    /// 实体决策树
    /// </summary>
    public class DecTrees
    {
        public int NodeId = 1;
        public Dictionary<string,NodeDataJson> EntityTrees=new Dictionary<string, NodeDataJson>();
        public Dictionary<string,NodeDataJson> WorldTrees=new Dictionary<string, NodeDataJson>();
    }
    
    
    /// <summary>
    /// 实体行为树
    /// </summary>
    public class BevTrees
    {
        public int NodeId = 1;
        public Dictionary<string,NodeDataJson> EntityTrees=new Dictionary<string, NodeDataJson>();
        public Dictionary<string,NodeDataJson> WorldTrees=new Dictionary<string, NodeDataJson>();
    }

    /// <summary>
    /// 节点数据
    /// </summary>
    public class NodeDataJson
    {
        public int NodeId;
        public NodeType Type = NodeType.Root;
        public NodePremiseJson Premise;
        public string TypeFullName = "";
        public int ChildMaxCnt = 0;
        
        public string Name = "";
        public double PosX;
        public double PosY;
        
        public List<NodeKeyValue> KeyValues=new List<NodeKeyValue>();
        public List<NodeDataJson> ChildNodes = new List<NodeDataJson>();

        public NodeDataJson()
        {
            
        }
        
        public NodeDataJson(int nodeId,NodeType nodeType,string typeFullName,float posX,float posY,string name="")
        {
            NodeId = nodeId;
            Type = nodeType;
            TypeFullName = typeFullName;
            ChildMaxCnt = GetNodeTypeChildCnt(nodeType,typeFullName);
            PosX = posX;
            PosY = posY;
            Name = name;
        }

        //获得节点可以添加的子节点数
        private int GetNodeTypeChildCnt(NodeType nodeType,string typeFullName)
        {
            switch (nodeType)
            {
                case NodeType.Root:
                    return 1;
                case NodeType.Action:
                    return 1;
            }

            if (typeFullName==typeof(NodeControlLoop).FullName)
            {
                return 1;
            }
            if (typeFullName==typeof(NodeControlLoop).FullName)
            {
                return 1;
            }

            return 10;
        }
    }
    
    /// <summary>
    /// 节点键值
    /// </summary>
    public class NodeKeyValue
    {
        public string KeyName="";

        public string TypeFullName = "";
        
        public string Value="";

        public NodeKeyValue()
        {
            
        }
        public NodeKeyValue(string key,string typeFullName,string value)
        {
            KeyName = key;
            TypeFullName = typeFullName;
            Value = value;
        }
    }

    /// <summary>
    /// 节点前提
    /// </summary>
    public class NodePremiseJson
    {
        public string Name;
        public PremiseType Type;
        public string TypeFullName;
        public bool TrueValue;
        public NodePremiseJson OtherPremise;
    }
}
