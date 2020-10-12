using LCECS.Core.ECS;
using LCECS.Data;
using LCECS.Layer.Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LCECS.Server.Layer
{
    public interface IInfoServer
    {
        void Init();

        EntityJson GetEntityConf(string entityName);

        T GetWorldInfo<T>(WorldInfoKey key, params object[] data) where T : InfoData;
        T GetEntityInfo<T>(EntityInfoKey key, Entity entity, params object[] data) where T : InfoData;

        void AddEntityWorkData(int entityId, EntityWorkData data);

        void AddWorldWorkData(int worldId, WorldWorkData data);

        EntityWorkData GetEntityWorkData(int entityId);

        WorldWorkData GetWorldWorkData(int worldId);

        void RemoveEntityWorkData(int entityId);

        void RemoveWorldWorkData(int worldId);
    }
}
