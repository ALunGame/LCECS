using LCECS.Core.ECS;
using LCECS.Core.Tree;
using LCECS.Data;
using LCHelp;
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
        private SystemSortJson systemSortJson = null;

        [Header("开启决策线程")]
        public bool OpenDecThread = false;
        private bool DecThreadRun = false;

        private void Awake()
        {
            Init();
        }

        private void Update()
        {
            //更新树时间
            NodeTime.UpdateTime(Time.deltaTime, 1);

            if (OpenDecThread==false)
            {
                ECSLayerLocate.Decision.ExecuteEntityDecision();
                ECSLayerLocate.Decision.ExecuteWorldDecision();
            }

            //执行行为树
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
            }, () => {
                ECSLocate.ECSLog.LogWarning("决策更新结束");
            });
        }

        #region 初始化

        private void Init()
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

        //设置服务
        private void InitServer()
        {
            ECSLocate.InitServer();
            ECSLayerLocate.InitLayerServer();
        }

        //初始化配置
        private void InitConf()
        {
            //系统排序
            TextAsset jsonData = ECSLocate.Factory.GetProduct<TextAsset>(FactoryType.Asset, null, ECSDefinitionPath.SystemSortPath);
            systemSortJson = LitJson.JsonMapper.ToObject<SystemSortJson>(jsonData.text);
        }

        //注册系统
        private void RegSystems()
        {
            List<Type> systemTypes = LCReflect.GetClassByType<BaseSystem>();
            List<Type> updateSystems = new List<Type>();
            List<Type> fixedUpdateSystems = new List<Type>();

            //分组
            for (int i = 0; i < systemTypes.Count; i++)
            {
                Type type = systemTypes[i];
                SystemAttribute attr = LCReflect.GetTypeAttr<SystemAttribute>(type);
                if (attr == null)
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
                BaseSystem system = LCReflect.CreateInstanceByType<BaseSystem>(type.FullName);
                system.Init();

                ECSLocate.ECS.RegUpdateSystem(system);
            }
            for (int i = 0; i < fixedUpdateSystems.Count; i++)
            {
                Type type = fixedUpdateSystems[i];
                BaseSystem system = LCReflect.CreateInstanceByType<BaseSystem>(type.FullName);
                system.Init();

                ECSLocate.ECS.RegFixedUpdateSystem(system);
            }
        }

        //系统排序
        private int SystemSortFunc(Type type01, Type type02)
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

        #endregion
    }
}
