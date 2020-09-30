using LCECS.Core.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LCECS.Layer.Info
{

    /// <summary>
    /// 世界感知器 特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class WorldSensorAttribute : Attribute
    {
        /// <summary>
        /// 世界信息键
        /// </summary>
        public WorldInfoKey InfoKey { get; set; } = 0;

        /// <summary>
        /// 世界信息需要感知的组件
        /// </summary>
        public Type[] SensorComs;

        public WorldSensorAttribute(WorldInfoKey infoKey)
        {
            this.InfoKey = infoKey;
        }
    }

    /// <summary>
    /// 实体感知器 特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EntitySensorAttribute : Attribute
    {
        /// <summary>
        /// 实体信息键
        /// </summary>
        public EntityInfoKey InfoKey { get; set; } = 0;

        public EntitySensorAttribute(EntityInfoKey infoKey)
        {
            this.InfoKey = infoKey;
        }
    }

    /// <summary>
    /// 世界感知器（收集世界信息）
    /// </summary>
    public interface IWorldSensor
    {
        T GetInfo<T>(params object[] data) where T: InfoData;
    }

    /// <summary>
    /// 自身感知器（收集自身信息）
    /// </summary>
    public interface IEntitySensor
    {
        T GetInfo<T>(Entity entity,params object[] data)where T: InfoData;
    }
}
