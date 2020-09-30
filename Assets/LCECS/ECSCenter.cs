using LCECS.Core.ECS;
using LCECS.Core.Tree;
using LCECS.Data;
using LCECS.Help;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace LCECS
{
    /// <summary>
    /// 框架中心
    /// </summary>
    public class ECSCenter:MonoBehaviour
    {
        private DecTrees decTrees = null;
        
        private BevTrees bevTrees = null;
        
        private EntityJsonList entityJsons = null;
        
        private ReqWeightJson reqWeightJson = null;

        private SystemSortJson systemSortJson = null;

        [Header("开启决策线程")]
        public bool OpenDecThread = false;
        private bool DecThreadRun = false;


        private void Awake()
        {
            InitServer();
            InitConf();
            RegSystems();

            //开启决策线程
            if (OpenDecThread)
            {
                DecThreadRun = true;
                ThreadExcuteDec();
            }
        }

        private void Update()
        {
            //更新树时间
            NodeTime.UpdateTime(Time.deltaTime, 1);

            //时间线
            TimeLineHelp.Update();

            if (OpenDecThread==false)
            {
                ECSLayerLocate.Decision.ExecuteEntityDecision();
                ECSLayerLocate.Decision.ExecuteWorldDecision();
            }

            //更新行为（就是更新组件中的数据）
            ECSLayerLocate.Behavior.ExecuteEntityBev();
            ECSLayerLocate.Behavior.ExecuteWorldBev();

            //系统处理
            ECSLocate.ECS.ExcuteUpdateSystem();
        }

        private void FixedUpdate()
        {
            //系统处理
            ECSLocate.ECS.ExcuteFixedUpdateSystem();
        }

        private void OnDestroy()
        {
            DecThreadRun = false;
        }

        //设置服务
        private void InitServer()
        {
            ECSLocate.InitServer();
            ECSLayerLocate.InitLayerServer();
        }

        //初始化配置
        private void InitConf()
        {
            //实体数据
            TextAsset jsonData = ECSLocate.Factory.GetProduct<TextAsset>(FactoryType.Asset, null, ECSDefinitionPath.EntityJsonPath);
            entityJsons = LitJson.JsonMapper.ToObject<EntityJsonList>(jsonData.text);
            ECSLocate.Config.SetEntityConf(entityJsons);
            
            //请求权重
            jsonData=ECSLocate.Factory.GetProduct<TextAsset>(FactoryType.Asset, null, ECSDefinitionPath.LogicReqWeightPath);
            reqWeightJson= LitJson.JsonMapper.ToObject<ReqWeightJson>(jsonData.text);
            ECSLocate.Config.SetReqWeightConf(reqWeightJson);
            
            //决策树
            jsonData=ECSLocate.Factory.GetProduct<TextAsset>(FactoryType.Asset, null, ECSDefinitionPath.DecTreePath);
            decTrees= LitJson.JsonMapper.ToObject<DecTrees>(jsonData.text);
            ECSLocate.Config.SetDecTrees(decTrees);
            
            //行为树
            jsonData=ECSLocate.Factory.GetProduct<TextAsset>(FactoryType.Asset, null, ECSDefinitionPath.BevTreePath);
            bevTrees= LitJson.JsonMapper.ToObject<BevTrees>(jsonData.text);
            ECSLocate.Config.SetBevTrees(bevTrees);

            //系统排序
            jsonData = ECSLocate.Factory.GetProduct<TextAsset>(FactoryType.Asset, null, ECSDefinitionPath.SystemSortPath);
            systemSortJson= LitJson.JsonMapper.ToObject<SystemSortJson>(jsonData.text);
        }

        //注册系统
        private void RegSystems()
        {
            List<Type> systemTypes = ReflectHelp.GetClassByType<BaseSystem>();
            List<Type> updateSystems = new List<Type>();
            List<Type> fixedUpdateSystems = new List<Type>();

            //分组
            for (int i = 0; i < systemTypes.Count; i++)
            {
                Type type = systemTypes[i];
                SystemAttribute attr = ReflectHelp.GetTypeAttr<SystemAttribute>(type);
                if(attr==null)
                {
                    //ECSLocate.ECSLog.Log("该系统没有设置系统特性>>>>>>", type.Name);
                    updateSystems.Add(type);
                }
                else
                {
                    if (attr.InFixedUpdate)
                        fixedUpdateSystems.Add(type);
                    else
                        updateSystems.Add(type);
                }
            }

            //排序
            updateSystems.Sort(SystemSortFunc);
            fixedUpdateSystems.Sort(SystemSortFunc);

            //注册
            for (int i = 0; i < updateSystems.Count; i++)
            {
                Type type = updateSystems[i];
                BaseSystem system = ReflectHelp.CreateInstanceByType<BaseSystem>(type);
                system.Init();

                ECSLocate.ECS.RegUpdateSystem(system);
            }
            for (int i = 0; i < fixedUpdateSystems.Count; i++)
            {
                Type type = fixedUpdateSystems[i];
                BaseSystem system = ReflectHelp.CreateInstanceByType<BaseSystem>(type);
                system.Init();

                ECSLocate.ECS.RegFixedUpdateSystem(system);
            }
        }

        //系统排序
        private int SystemSortFunc(Type type01,Type type02)
        {
            int sysSort01 = GetSystemSort(type01.FullName);
            int sysSort02 = GetSystemSort(type02.FullName);

            if (sysSort01 == sysSort02)
                return 0;
            else if (sysSort01 < sysSort02)
                return -1;
            else
                return 1;
        }

        //获得系统排序
        private int GetSystemSort(string sysName)
        {
            List<SortJson> sorts = systemSortJson.UpdateList;
            for (int i = 0; i < sorts.Count; i++)
            {
                if (sorts[i].TypeName == sysName)
                {
                    return sorts[i].Sort;
                }
            }

            sorts = systemSortJson.FixedUpdateList;
            for (int i = 0; i < sorts.Count; i++)
            {
                if (sorts[i].TypeName == sysName)
                {
                    return sorts[i].Sort;
                }
            }

            return 9999;
        }

        //线程执行决策层（只是读取数据操作）
        private void ThreadExcuteDec()
        {
            TaskHelp.AddTask(() =>
            {
                while (DecThreadRun)
                {
                    Thread.Sleep(100);
                    //更新决策
                    ECSLayerLocate.Decision.ExecuteEntityDecision();
                    ECSLayerLocate.Decision.ExecuteWorldDecision();
                }
            }, ()=> {
                ECSLocate.ECSLog.LogWarning("决策更新结束");
            });
        }

    }
}
