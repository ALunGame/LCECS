using LCECS.Core.ECS;
using LCECS.Data;
using System.Collections.Generic;

namespace LCECS.Layer.Info
{
    public class InfoData
    {
        public T As<T>() where T : InfoData
        {
            return (T)this;
        }
    }

    /// <summary>
    /// 信息池
    /// </summary>
    public class InfosCenter
    {
        private Dictionary<int, IWorldSensor> WorldSensorDict   = new Dictionary<int, IWorldSensor>();
        private Dictionary<int, IEntitySensor> EntitySensorDict     = new Dictionary<int, IEntitySensor>();

        private Dictionary<int, EntityWorkData> EntityWorkDataDict = new Dictionary<int, EntityWorkData>();
        private Dictionary<int, WorldWorkData> WorldWorkDataDict = new Dictionary<int, WorldWorkData>();

        public void RegWorldSensor(int key, IWorldSensor sensor)
        {
            if (WorldSensorDict.ContainsKey(key))
                return;
            WorldSensorDict.Add(key, sensor);
        }

        public void RegEntitySensor(int key, IEntitySensor sensor)
        {
            if (EntitySensorDict.ContainsKey(key))
                return;
            EntitySensorDict.Add(key, sensor);
        }

        public T GetWorldInfo<T>(int key,params object[] data) where T:InfoData
        {
            if (!WorldSensorDict.ContainsKey(key))
            {
                ECSLocate.ECSLog.LogError("没有找到对应的世界信息》》》》" + key);
                return null;
            }
            IWorldSensor sensor = WorldSensorDict[key];
            return sensor.GetInfo<T>(data);
        }

        public T GetEntityInfo<T>(int key,Entity entity,params object[] data)where T:InfoData
        {
            if (!EntitySensorDict.ContainsKey(key))
            {
                ECSLocate.ECSLog.LogError("没有找到对应的自身信息》》》》" + key);
                return null;
            }
            IEntitySensor sensor = EntitySensorDict[key];
            return sensor.GetInfo<T>(entity,data);
        }
        
        public void AddEntityWorkData(int entityId, EntityWorkData data)
        {
            if(EntityWorkDataDict.ContainsKey(entityId))
            {
                return;
            }
            EntityWorkDataDict.Add(entityId, data);
        }

        public void AddWorldWorkData(int worldId, WorldWorkData data)
        {
            if (WorldWorkDataDict.ContainsKey(worldId))
            {
                return;
            }
            WorldWorkDataDict.Add(worldId, data);
        }

        public EntityWorkData GetEntityWorkData(int entityId)
        {
            if (EntityWorkDataDict.ContainsKey(entityId))
            {
                return EntityWorkDataDict[entityId];
            }
            return null;
        }

        public WorldWorkData GetWorldWorkData(int worldId)
        {
            if (WorldWorkDataDict.ContainsKey(worldId))
            {
                return WorldWorkDataDict[worldId];
            }
            return null;
        }

        public void RemoveEntityWorkData(int entityId)
        {
            if (EntityWorkDataDict.ContainsKey(entityId))
            {
                EntityWorkDataDict.Remove(entityId);
            }
        }

        public void RemoveWorldWorkData(int worldId)
        {
            if (WorldWorkDataDict.ContainsKey(worldId))
            {
                WorldWorkDataDict.Remove(worldId);
            }
        }

    }
}
