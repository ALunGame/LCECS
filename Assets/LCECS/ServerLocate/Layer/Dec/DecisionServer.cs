using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LCECS.Data;
using LCECS.Layer.Decision;

namespace LCECS.Server.Layer
{
    public class DecisionServer : IDecisionServer
    {
        private DecisionCenter decisionCenter = new DecisionCenter();

        public void Init()
        {
            decisionCenter.Init();
        }

        public void RegEntityDecision(EntityDecGroup decId, BaseEntityDecision decision)
        {
            decisionCenter.RegEntityDecision((int)decId, decision);
        }

        public void RegWorldDecision(WorldDecGroup decId, BaseWorldDecision decision)
        {
            decisionCenter.RegWorldDecision((int)decId, decision);
        }

        public void AddDecisionEntity(EntityDecGroup decId, EntityWorkData workData)
        {
            decisionCenter.AddDecisionEntity((int)decId, workData);
        }

        public void AddDecisionWorld(WorldDecGroup decId, WorldWorkData workData)
        {
            decisionCenter.AddDecisionWorld((int)decId, workData);
        }

        public void RemoveDecisionEntity(EntityDecGroup decId, int entityId)
        {
            decisionCenter.RemoveDecisionEntity((int)decId, entityId);
        }

        public void RemoveDecisionWorld(WorldDecGroup decId, int worldId)
        {
            decisionCenter.RemoveDecisionWorld((int)decId, worldId);
        }

        public void ExecuteWorldDecision()
        {
            decisionCenter.ExecuteWorldDecision();
        }

        public void ExecuteEntityDecision()
        {
            decisionCenter.ExecuteEntityDecision();
        }
    }
}
