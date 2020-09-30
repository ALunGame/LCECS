using LCECS.Help;
using System;
using UnityEngine;

namespace LCECS.Core.ECS
{
    /// <summary>
    /// 组件特性 类可用
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ComAttribute : Attribute
    {
        /// <summary>
        /// 编辑器显示名
        /// </summary>
        public string ViewName { get; set; } = "";

        /// <summary>
        /// 组件分组名
        /// </summary>
        public string GroupName { get; set; } = "";

        /// <summary>
        /// 全局唯一的组件
        /// </summary>
        public bool IsGlobal { get; set; } = false;
    }
    
    /// <summary>
    /// 组件字段特性 字段可用
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ComValueAttribute : Attribute
    {
        /// <summary>
        /// 可以在编辑器模式下编辑
        /// </summary>
        public bool ViewEditor { get; set; } = false;
        /// <summary>
        /// 可以在编辑器模式下显示
        /// </summary>
        public bool ShowView { get; set; } = true;
    }
    
    public class BaseCom
    {
        private int entityId = 0;

        public bool IsActive { get; private set; } = false;

        //初始化（首次添加调用）
        public void Init(int entityId,GameObject go)
        {
            this.entityId = entityId;
            IsActive = true;
            OnEnable();
            OnInit(go);
        }

        //实体本身启用
        public void EntityEnable()
        {
            IsActive = true;
            OnEnable();
        }

        //启用
        public void Enable()
        {
            ECSLocate.ECS.CheckEntityInSystem(entityId);
            IsActive = true;
            OnEnable();
        }

        //实体本身禁用
        public void EntityDisable()
        {
            IsActive = false;
            OnDisable();
        }

        //禁用
        public void Disable()
        {
            ECSLocate.ECS.CheckEntityInSystem(entityId);
            IsActive = false;
            OnDisable();
        }

        //初始化（首次添加调用）
        protected virtual void OnInit(GameObject go)
        {

        }

        protected virtual void OnEnable()
        {

        }

        protected virtual void OnDisable()
        {

        }
    }
}
