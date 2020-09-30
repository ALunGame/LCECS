using LCECS.Server.Config;
using LCECS.Server.ECS;
using LCECS.Server.Factory;
using LCECS.Server.Layer;
using LCECS.Server.Log;
using LCECS.Server.Player;

namespace LCECS
{
    /// <summary>
    /// ECS服务定位器
    /// </summary>
    public static class ECSLocate
    {
        private static IECSLogServer eCSLog;
        public static IECSLogServer ECSLog
        {
            get
            {
                return eCSLog;
            }

            set
            {
                eCSLog = value;
            }
        }

        private static IECSServer eCS;
        public static IECSServer ECS
        {
            get
            {
                return eCS;
            }

            set
            {
                eCS = value;
            }
        }

        private static IConfigServer config;
        public static IConfigServer Config
        {
            get
            {
                return config;
            }

            set
            {
                config = value;
            }
        }

        private static IFactoryServer factory;
        public static IFactoryServer Factory
        {
            get
            {
                return factory;
            }

            set
            {
                factory = value;
            }
        }

        private static IPlayerServer player;
        public static IPlayerServer Player
        {
            get
            {
                return player;
            }

            set
            {
                player = value;
            }
        }

        public static void InitServer()
        {
            ECSLog      = new ECSLogServer();
            ECS         = new ECSServer();
            Config      = new ConfigServer();
            Factory     = new FactoryServer();
            Player      = new PlayerServer();
        }
    }
}
