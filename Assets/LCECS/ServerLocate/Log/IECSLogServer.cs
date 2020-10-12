
namespace LCECS.Server.Log
{
    public interface IECSLogServer
    {
        //日志（真机不输出）
        void Log(string str, params object[] data);

        //报错日志（真机不输出）
        void LogR(string str, params object[] data);

        //警告日志（真机不输出）
        void LogWarning(string str, params object[] data);

        //报错日志（真机输出）
        void LogError(string str, params object[] data);
    }
}
