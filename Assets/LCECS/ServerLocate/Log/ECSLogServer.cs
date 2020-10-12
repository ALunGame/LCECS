using LCHelp;

namespace LCECS.Server.Log
{
    public class ECSLogServer : IECSLogServer
    {
        public void Log(string str, params object[] data)
        {
            LCLog.Log(str, data);
        }

        public void LogWarning(string str, params object[] data)
        {
            LCLog.LogWarning(str, data); 
        }

        public void LogR(string str, params object[] data)
        {
            LCLog.LogR(str, data);
        }

        public void LogError(string str, params object[] data)
        {
            LCLog.LogError(str, data);
        }
    }
}
