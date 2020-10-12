using LCECS.Core.Tree.Base;
using LCECS.Data;
using LCECS.Layer.Behavior;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LCECS.Server.Layer
{
    public class BehaviorServer : IBehaviorServer
    {
        private BehaviorCenter behaviorCenter = new BehaviorCenter();

        public void Init()
        {
            behaviorCenter.Init();
        }

        public Node GetEntityBevNode(EntityReqId bevId)
        {
            return behaviorCenter.GetEntityBevNode((int)bevId);
        }

        public Node GetWorldBevNode(WorldReqId bevId)
        {
            return behaviorCenter.GetWorldBevNode((int)bevId);
        }

        public void PushEntityBev(EntityWorkData workData)
        {
            behaviorCenter.PushEntityBev(workData);
        }

        public void PushWorldBev(WorldWorkData workData)
        {
            behaviorCenter.PushWorldBev(workData);
        }

        public void ExecuteEntityBev()
        {
            behaviorCenter.ExecuteEntityBev();
        }

        public void ExecuteWorldBev()
        {
            behaviorCenter.ExecuteWorldBev();
        }
    }
}
