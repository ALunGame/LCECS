using LCECS.Core.ECS;
using LCECS.Data;
using LCHelp;
using System;
using System.Collections.Generic;
using UnityEngine;

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

        private EntityJsonList entityJsons = null;

        public void Init()
        {
            TextAsset jsonData = ECSLocate.Factory.GetProduct<TextAsset>(FactoryType.Asset, null, ECSDefinitionPath.EntityJsonPath);
            EntityJsonList entityJsons = LitJson.JsonMapper.ToObject<EntityJsonList>(jsonData.text);
            SetEntityConf(entityJsons);
            RegAllSensor();
        }

        private void SetEntityConf(EntityJsonList json)
        {
            entityJsons = json;
        }

        public EntityJson GetEntityConf(string entityName)
        {
            if (entityJsons == null)
                return null;
            for (int i = 0; i < entityJsons.List.Count; i++)
            {
                EntityJson json = entityJsons.List[i];
                if (json.EntityName == entityName)
                {
                    return json;
                }
            }
            return null;
        }

        private void RegAllSensor()
        {
            List<Type> worldSensorTypes = LCReflect.GetInterfaceByType<IWorldSensor>();
            List<Type> entitySensorTypes = LCReflect.GetInterfaceByType<IEntitySensor>();
            if (worldSensorTypes == null && entitySensorTypes == null)
                return;

            //世界信息
            foreach (Type type in worldSensorTypes)
            {
                WorldSensorAttribute attr = LCReflect.GetTypeAttr<WorldSensorAttribute>(type);
                if (attr == null)
                {
                    ECSLocate.ECSLog.LogR("有世界信息没有加入特性 >>>>>>", type.Name);
                    return;
                }

                IWorldSensor sensor = LCReflect.CreateInstanceByType<IWorldSensor>(type.FullName);
                WorldSensorDict.Add((int)attr.InfoKey, sensor);
            }

            //实体信息
            foreach (Type type in entitySensorTypes)
            {
                EntitySensorAttribute attr = LCReflect.GetTypeAttr<EntitySensorAttribute>(type);
                if (attr == null)
                {
                    ECSLocate.ECSLog.LogR("有实体信息没有加入特性 >>>>>>", type.Name);
                    return;
                }

                IEntitySensor sensor = LCReflect.CreateInstanceByType<IEntitySensor>(type.FullName);
                EntitySensorDict.Add((int)attr.InfoKey, sensor);
            }
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
