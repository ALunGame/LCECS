using LCECS.Core.Tree.Base;
using LCECS.Data;
using LCECS.Layer.Behavior;

namespace LCECS.Server.Layer
{
    public interface IBehaviorServer
    {
        void Init();

        Node GetEntityBevNode(EntityReqId bevId);
        Node GetWorldBevNode(WorldReqId bevId);

        void PushEntityBev(EntityWorkData workData);
        void PushWorldBev(WorldWorkData workData);

        void ExecuteEntityBev();
        void ExecuteWorldBev();
    }
}
