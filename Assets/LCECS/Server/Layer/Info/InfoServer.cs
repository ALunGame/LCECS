using LCECS.Core.ECS;
using LCECS.Data;
using LCECS.Layer.Info;

namespace LCECS.Server.Layer
{
    public class InfoServer : IInfoServer
    {
        private InfosCenter infosCenter = new InfosCenter();

        public void RegEntitySensor(EntityInfoKey key, IEntitySensor sensor)
        {
            infosCenter.RegEntitySensor((int)key, sensor);
        }

        public void RegWorldSensor(WorldInfoKey key, IWorldSensor sensor)
        {
            infosCenter.RegWorldSensor((int)key, sensor);
        }

        public T GetWorldInfo<T>(WorldInfoKey key,params object[] data) where T:InfoData
        {
            return infosCenter.GetWorldInfo<T>((int)key,data);
        }

        public T GetEntityInfo<T>(EntityInfoKey key,Entity entity,params object[] data)where T:InfoData
        {
            return infosCenter.GetEntityInfo<T>((int)key, entity,data);
        }

        public void AddEntityWorkData(int entityId, EntityWorkData data)
        {
            infosCenter.AddEntityWorkData(entityId, data);
        }

        public void AddWorldWorkData(int worldId, WorldWorkData data)
        {
            infosCenter.AddWorldWorkData(worldId, data);
        }

        public EntityWorkData GetEntityWorkData(int entityId)
        {
            return infosCenter.GetEntityWorkData(entityId);
        }

        public WorldWorkData GetWorldWorkData(int worldId)
        {
            return infosCenter.GetWorldWorkData(worldId);
        }

        public void RemoveEntityWorkData(int entityId)
        {
            infosCenter.RemoveEntityWorkData(entityId);
        }

        public void RemoveWorldWorkData(int worldId)
        {
            infosCenter.RemoveWorldWorkData(worldId);
        }
    }
}
