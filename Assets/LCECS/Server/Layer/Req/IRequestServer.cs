using LCECS.Data;
using LCECS.Layer.Request;

namespace LCECS.Server.Layer
{
    public interface IRequestServer
    {
        void RegWorldRequest(WorldReqId key, IWorldRequest request);
        void RegEntityRequest(EntityReqId key, IEntityRequest request);

        void PushEntityRequest(int entityId, EntityReqId reqId);
        void PushWorldRequest(int worldId, WorldReqId reqId);
    }
}
