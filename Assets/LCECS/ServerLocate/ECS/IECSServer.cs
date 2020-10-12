using LCECS.Core.ECS;
using System;
using UnityEngine;

namespace LCECS.Server.ECS
{
    /// <summary>
    /// ECS服务类
    /// </summary>
    public interface IECSServer
    {
        //创建实体（通知所有的系统检测下）  ----由工厂实例化节点
        Entity CreateEntity(string entityName,ref GameObject go);

		//创建实体（通知所有的系统检测下）  ----不需要实例化节点
		Entity CreateEntity(string entityName,GameObject go);

		//获得实体
		Entity GetEntity(int entityId);

        //获得全局单一组件
        T GetGlobalSingleCom<T>() where T:BaseCom;

        //设置全局单一组件的值
        void SetGlobalSingleComData<T>(Action<T> changeData) where T : BaseCom;

        //注册全局单一组件改变的回调
        void RegGlobalSingleComChangeCallBack(Type comType,Action callBack);

        //注册在Update中更新系统
        void RegUpdateSystem(BaseSystem system);

        //注册在FixedUpdate中更新系统
        void RegFixedUpdateSystem(BaseSystem system);

        //检测系统是否检测该实体
        void CheckEntityInSystem(int entityId);

        //执行UpdateSystem
        void ExcuteUpdateSystem();

        //执行UpdateSystem
        void ExcuteFixedUpdateSystem();

    }
}
