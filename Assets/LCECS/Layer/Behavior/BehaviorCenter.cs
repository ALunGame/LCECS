using LCECS;
using LCECS.Core.Tree.Base;
using LCECS.Data;
using LCECS.Layer.Behavior;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 行为中心：
/// 1，请求Id，与具体实现组合
/// 2，行为实现中只做一个事情，，将实体组件的值改变（PS：改变的结果表现是System搞得）
/// </summary>
public class BehaviorCenter {

    private Dictionary<int, BaseEntityBehavior> EntityBevDict = new Dictionary<int, BaseEntityBehavior>();
    private Dictionary<int, BaseWorldBehavior> WorldBevDict = new Dictionary<int, BaseWorldBehavior>();

    public void Init()
    {
        TextAsset jsonData = ECSLocate.Factory.GetProduct<TextAsset>(FactoryType.Asset, null, ECSDefinitionPath.BevTreePath);
        BevTrees bevTrees = LitJson.JsonMapper.ToObject<BevTrees>(jsonData.text);
        SetBevTrees(bevTrees);
    }

    //设置行为树
    private void SetBevTrees(BevTrees trees)
    {
        if (trees == null)
            return;

        CreateBev(trees);
    }

    //创建行为树
    private void CreateBev(BevTrees BevTrees)
    {
        //实体
        foreach (string key in BevTrees.EntityTrees.Keys)
        {
            EntityReqId reqId = (EntityReqId)Enum.Parse(typeof(EntityReqId), key);

            //创建树
            NodeDataJson nodeJson = BevTrees.EntityTrees[key];
            Node rootNode = Node.CreateNodeInstance(nodeJson);
            Node.CreateNodeRelation(rootNode, nodeJson.ChildNodes);

            BaseEntityBehavior request = new BaseEntityBehavior(rootNode);
            EntityBevDict.Add((int)reqId, request);
        }

        //世界
        foreach (string key in BevTrees.WorldTrees.Keys)
        {
            WorldReqId reqId = (WorldReqId)Enum.Parse(typeof(WorldReqId), key);

            //创建树
            NodeDataJson nodeJson = BevTrees.WorldTrees[key];
            Node rootNode = Node.CreateNodeInstance(nodeJson);
            Node.CreateNodeRelation(rootNode, nodeJson.ChildNodes);

            BaseWorldBehavior request = new BaseWorldBehavior(rootNode);
            WorldBevDict.Add((int)reqId, request);
        }
    }

    /// <summary>
    /// 获得实体行为
    /// </summary>
    private BaseEntityBehavior GetEntityBev(int bevId)
    {
        if (EntityBevDict.ContainsKey(bevId))
        {
            return EntityBevDict[bevId];
        }
        return null;
    }

    /// <summary>
    /// 获得世界行为
    /// </summary>
    private BaseWorldBehavior GetWorldBev(int bevId)
    {
        if (WorldBevDict.ContainsKey(bevId))
        {
            return WorldBevDict[bevId];
        }
        return null;
    }

    /// <summary>
    /// 获得实体行为树
    /// </summary>
    public Node GetEntityBevNode(int bevId)
    {
        BaseEntityBehavior behavior = GetEntityBev(bevId);
        if (behavior!=null)
        {
            return behavior.Tree;
        }
        return null;
    }

    /// <summary>
    /// 获得世界行为树
    /// </summary>
    public Node GetWorldBevNode(int bevId)
    {
        BaseWorldBehavior behavior = GetWorldBev(bevId);
        if (behavior != null)
        {
            return behavior.Tree;
        }
        return null;
    }

    /// <summary>
    /// 放入实体行为
    /// </summary>
    public void PushEntityBev(EntityWorkData workData)
    {
        //删除
        BaseEntityBehavior lastBehavior = GetEntityBev(workData.ClearReqId);
        if (lastBehavior != null)
        {
            lastBehavior.RemoveWorkData(workData);
        }

        //添加
        BaseEntityBehavior currBehavior = GetEntityBev(workData.CurrReqId);
        if (currBehavior != null)
        {
            currBehavior.AddWorkData(workData);
        }
    }

    /// <summary>
    /// 放入世界行为
    /// </summary>
    public void PushWorldBev(WorldWorkData workData)
    {
        //删除
        BaseWorldBehavior lastBehavior = GetWorldBev(workData.ClearReqId);
        if (lastBehavior != null)
        {
            lastBehavior.RemoveWorkData(workData);
        }

        //添加
        BaseWorldBehavior currBehavior = GetWorldBev(workData.CurrReqId);
        if (currBehavior != null)
        {
            currBehavior.AddWorkData(workData);
        }
    }

    /// <summary>
    /// 执行实体行为
    /// </summary>
    public void ExecuteEntityBev()
    {
        foreach (BaseEntityBehavior item in EntityBevDict.Values)
        {
            item.Execute();
        }
    }

    /// <summary>
    /// 执行世界行为
    /// </summary>
    public void ExecuteWorldBev()
    {
        foreach (BaseWorldBehavior item in WorldBevDict.Values)
        {
            item.Execute();
        }
    }
}
