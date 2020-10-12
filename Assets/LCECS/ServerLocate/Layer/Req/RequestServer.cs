using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LCECS.Data;
using LCECS.Layer.Request;

namespace LCECS.Server.Layer
{
    public class RequestServer : IRequestServer
    {
        private RequestCenter requestCenter = new RequestCenter();

        public void Init()
        {
            requestCenter.Init();
        }

        public void PushEntityRequest(int entityId, EntityReqId reqId)
        {
            requestCenter.PushEntityRequest(entityId, (int)reqId);
        }

        public void PushWorldRequest(int worldId, WorldReqId reqId)
        {
            requestCenter.PushWorldRequest(worldId, (int)reqId);
        }
    }
}
