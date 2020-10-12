using System.Text;
using UnityEngine;

namespace LCHelp
{
    public class LCLog
    {
        public static bool OpenLog = true;

        private static StringBuilder logStr = new StringBuilder("");
        private static void HandleLogStr(string str, params object[] data)
        {
            logStr.Clear();
            logStr.Append(str);
            if (data == null || data.Length <= 0 || data[0] == null)
            {
                return;
            }
            if (data[0] == null)
            {
                logStr.Append("   null");
                return;
            }
            for (int i = 0; i < data.Length; i++)
            {
                logStr.Append("   " + data[i].ToString());
            }
        }

        public static void Log(string str, params object[] data)
        {
#if UNITY_EDITOR
            HandleLogStr(str, data);
            Debug.Log(logStr);
#endif
        }

        public static void LogWarning(string str, params object[] data)
        {
#if UNITY_EDITOR
            HandleLogStr(str, data);
            Debug.LogWarning(logStr); 
#endif
        }

        public static void LogR(string str, params object[] data)
        {
#if UNITY_EDITOR
            HandleLogStr(str, data);
            Debug.LogError(logStr);
#endif
        }

        public static void LogError(string str, params object[] data)
        {
            HandleLogStr(str, data);
            Debug.LogError(logStr);
        }
    }
}
