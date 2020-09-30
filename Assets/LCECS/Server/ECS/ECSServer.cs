using LCECS.Core.ECS;
using LCECS.Data;
using LCECS.Help;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LCECS.Server.ECS
{
	/// <summary>
	/// 1，保存所有的实体
	/// 2，所有的Update系统
	/// 3，所有的FixedUpdate系统
	/// 4，提供创建实体，执行系统，获得实体方法
	/// </summary>
	public class ECSServer : IECSServer
	{
		private int EntityId = 1;

        //全局单个组件
        private Dictionary<Type,BaseCom> globalSingleCom = new Dictionary<Type, BaseCom>();
        private Dictionary<Type,Action> globalSingleComCallBack = new Dictionary<Type, Action>();

        //实体列表
        private Dictionary<int, Entity> entityDict = new Dictionary<int, Entity>();

        //所有Update系统
        private List<BaseSystem> systemUpdateList = new List<BaseSystem>();

        //所有FixedUpdate系统
        private List<BaseSystem> systemFixedUpdateList = new List<BaseSystem>();

        //添加实体
        private void AddEntity(int id, Entity entity)
        {
            if (!entityDict.ContainsKey(id))
                entityDict.Add(id, entity);
        }

        //检测系统是否检测该实体
        private void CheckEntityInSystem(Entity entity)
        {
            for (int i = 0; i < systemUpdateList.Count; i++)
            {
                systemUpdateList[i].CheckEntity(entity);
            }

            for (int i = 0; i < systemFixedUpdateList.Count; i++)
            {
                systemFixedUpdateList[i].CheckEntity(entity);
            }
        }

        //初始化实体（所有的实体生成都会走）
        private void InitEntity(Entity entity,string entityName)
        {
            //实体数据
            EntityJson entityData = ECSLocate.Config.GetEntityData(entityName);

            //初始化实体
            entity.Init(EntityId, entityName, entityData.Group);

            //保存
            AddEntity(EntityId, entity);

            //系统检测
            CheckEntityInSystem(entity);

            //创建实体数据流
            EntityWorkData entityWorkData = new EntityWorkData(EntityId, entity);
            entityWorkData.Id = EntityId;
            ECSLayerLocate.Info.AddEntityWorkData(EntityId, entityWorkData);
            ECSLayerLocate.Decision.AddDecisionEntity(entityData.Group, entityWorkData);

            //添加实体全局单一组件
            AddEntityGlobalSingleCom(entityData, entity);

            //Id自增
            EntityId++;
        }

        //添加实体全局单一组件
        private void AddEntityGlobalSingleCom(EntityJson conf, Entity entity)
        {
            for (int i = 0; i < conf.Coms.Count; i++)
            {
                BaseCom com = entity.GetCom(conf.Coms[i].ComName);
                if (ECSHelp.CheckComIsGlobal(com.GetType()))
                {
                    if (globalSingleCom.ContainsKey(com.GetType()))
                    {
                        ECSLocate.ECSLog.LogError("有多个全局单个组件>>>>>>", conf.EntityName, com.GetType());
                        entity.Disable();
                        return;
                    }
                    globalSingleCom.Add(com.GetType(),com);
                }
            }
        }

        //-------------------------------------------------------- 接口实现 --------------------------------------------------------//

        public Entity CreateEntity(string entityName,ref GameObject go)
        {
			//创建实体
			GameObject entityGo = null;
            Entity entity = ECSLocate.Factory.GetProduct<Entity>(FactoryType.Entity,(object[] data)=>{
				if (data[0]!=null)
				{
					entityGo = data[0] as GameObject;
				}
			}, EntityId, entityName);

			if (entity == null)
				return null;
            go = entityGo;
            InitEntity(entity,entityName);
            return entity;
        }

		public Entity CreateEntity(string entityName,GameObject go)
		{
			//创建实体
			Entity entity = ECSLocate.Factory.GetProduct<Entity>(FactoryType.Entity, null, EntityId, entityName, go);
			if (entity==null)
				return null;
            InitEntity(entity, entityName);
			return entity;
		}

		//获得实体
		public Entity GetEntity(int id)
		{
			return entityDict[id];
		}

        //获得全局单一组件
        public T GetGlobalSingleCom<T>() where T : BaseCom
        {
            Type comType = typeof(T);
            if (!globalSingleCom.ContainsKey(comType))
            {
                ECSLocate.ECSLog.LogError("获得全局单一组件，出错 没有该全局组件>>>>>>", comType);
                return null;
            }
            return (T)globalSingleCom[comType];
        }

        //设置全局单一组件的值
        public void SetGlobalSingleComData<T>(Action<T> changeData) where T : BaseCom
        {
            Type comType = typeof(T);
            if (!globalSingleCom.ContainsKey(comType))
            {
                ECSLocate.ECSLog.LogError("设置全局单一组件的值，出错 没有该全局组件>>>>>>", comType);
                return;
            }
            //回调
            changeData((T)globalSingleCom[comType]);
            //广播事件
            Action callBack = null;
            if (globalSingleComCallBack.ContainsKey(comType))
                callBack = globalSingleComCallBack[comType];
            callBack?.Invoke();
        }

        public void RegGlobalSingleComChangeCallBack(Type comType, Action callBack)
        {
            if (globalSingleComCallBack.ContainsKey(comType))
            {
                globalSingleComCallBack[comType] += callBack;
            }
            else
            {
                globalSingleComCallBack[comType] = callBack;
            }
        }

        public void RegUpdateSystem(BaseSystem system)
        {
            if (systemUpdateList.Contains(system))
                return;
            systemUpdateList.Add(system);
        }

        public void RegFixedUpdateSystem(BaseSystem system)
        {
            if (systemFixedUpdateList.Contains(system))
                return;
            systemFixedUpdateList.Add(system);
        }

        public void CheckEntityInSystem(int entityId)
        {
            Entity entity = GetEntity(entityId);
            if (entity == null)
                return;
            CheckEntityInSystem(entity);
        }

        public void ExcuteUpdateSystem()
        {
            for (int i = 0; i < systemUpdateList.Count; i++)
            {
                systemUpdateList[i].Excute();
            }
        }

        public void ExcuteFixedUpdateSystem()
        {
            for (int i = 0; i < systemFixedUpdateList.Count; i++)
            {
                systemFixedUpdateList[i].Excute();
            }
        }
    }
}
