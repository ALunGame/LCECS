using LCECS.Data;
using LCECS.Layer.Request;

namespace LCECS.Server.Layer
{
    public interface IRequestServer
    {
        void Init();
        void PushEntityRequest(int entityId, EntityReqId reqId);
        void PushWorldRequest(int worldId, WorldReqId reqId);
    }
}
