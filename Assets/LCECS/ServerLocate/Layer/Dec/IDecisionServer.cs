using LCECS.Data;
using LCECS.Layer.Decision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LCECS.Server.Layer
{
    public interface IDecisionServer
    {
        void Init();

        void AddDecisionEntity(EntityDecGroup decId, EntityWorkData workData);
        void AddDecisionWorld(WorldDecGroup decId, WorldWorkData workData);

        void RemoveDecisionEntity(EntityDecGroup decId, int entityId);
        void RemoveDecisionWorld(WorldDecGroup decId, int worldId);

        void ExecuteEntityDecision();
        void ExecuteWorldDecision();
    }
}
