using LCECS.Server.ECS;
using LCECS.Server.Factory;
using LCECS.Server.Log;
using LCECS.Server.Player;

namespace LCECS
{
    /// <summary>
    /// ECS服务定位器
    /// </summary>
    public static class ECSLocate
    {
        public static IECSLogServer ECSLog { get; set; }
        public static IECSServer ECS { get; set; }
        public static IFactoryServer Factory { get; set; }
        public static IPlayerServer Player { get; set; }

        public static void InitServer()
        {
            ECSLog      = new ECSLogServer();
            ECS         = new ECSServer();
            Factory     = new FactoryServer();
            Player      = new PlayerServer();
        }
    }
}
