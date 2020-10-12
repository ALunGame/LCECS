using LCECS.Server.Layer;

namespace LCECS
{
    /// <summary>
    /// ECS层级定位器
    /// </summary>
    public class ECSLayerLocate
    {
        public static IInfoServer Info { get; set; }
        public static IDecisionServer Decision { get; set; }
        public static IRequestServer Request { get; set; }
        public static IBehaviorServer Behavior { get; set; }

        public static void InitLayerServer()
        {
            Info        = new InfoServer();
            Info.Init();
            Decision    = new DecisionServer();
            Decision.Init();
            Request     = new RequestServer();
            Request.Init();
            Behavior    = new BehaviorServer();
            Behavior.Init();
        }
    }
}
