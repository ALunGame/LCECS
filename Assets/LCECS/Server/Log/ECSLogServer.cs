using UnityEngine;

namespace LCECS.Server.Log
{
    public class ECSLogServer : IECSLogServer
    {
        private bool OpenLog = true;
        public void Log(string str, params object[] data)
        {
#if UNITY_EDITOR
            if (OpenLog==false)
            {
                return;
            }
            if (data == null || data.Length <= 0 || data[0] == null)
            {
                Debug.Log(str + "   null");
                return;
            }
            string parmStr = "";
            for (int i = 0; i < data.Length; i++)
            {
                parmStr += "   "+data[i].ToString();
            }
            Debug.Log(str + parmStr);
#endif
        }

        public void LogWarning(string str, params object[] data)
        {
            if (OpenLog == false)
            {
                return;
            }
            if (data == null || data.Length <= 0 || data[0] == null)
            {
                Debug.LogWarning(str + "    null");
                return;
            }
            string parmStr = "";
            for (int i = 0; i < data.Length; i++)
            {
                parmStr += "   " + data[i].ToString();
            }
            Debug.LogWarning(str + parmStr);
        }

        public void LogR(string str, params object[] data)
        {
#if UNITY_EDITOR
            if (OpenLog == false)
            {
                return;
            }
            if (data == null || data.Length<=0 || data[0] == null)
            {
                Debug.LogError(str + "   null");
                return;
            }
            string parmStr = "";
            for (int i = 0; i < data.Length; i++)
            {
                parmStr += "   " + data[i].ToString();
            }
            Debug.LogError(str + parmStr); 
#endif
        }

        public void LogError(string str, params object[] data)
        {
            if (OpenLog == false)
            {
                return;
            }
            if (data == null || data.Length <= 0 || data[0] == null)
            {
                Debug.LogError(str + "    null");
                return;
            }
            string parmStr = "";
            for (int i = 0; i < data.Length; i++)
            {
                parmStr += "   " + data[i].ToString();
            }
            Debug.LogError(str + parmStr);
        }
    }
}
