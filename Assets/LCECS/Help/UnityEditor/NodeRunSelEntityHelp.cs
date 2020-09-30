#if UNITY_EDITOR
using System.Collections.Generic;

namespace LCECS.Help
{
    /// <summary>
    /// 用于辅助监视实体的决策树运行状态
    /// </summary>
    public class NodeRunSelEntityHelp
    {
        //本次执行需不需要刷新
        public static bool RefreshRunState;
        public static int SelEntityId = 0;
        //正在执行的节点
        public static List<int> RunningNodes = new List<int>();

        public static void RefreshRunningNode()
        {
            RunningNodes.Clear();
        }

        public static void SetNeedRefresh(int entityId)
        {
            if (SelEntityId == 0)
            {
                RefreshRunState = false;
            }

            if (RefreshRunState == true)
            {
                return;
            }
            if (entityId == SelEntityId)
            {
                RefreshRunState = true;
                RunningNodes.Clear();
                //ShowNodes.Clear();
                return;
            }
        }

        public static void SetRunningNode(int entityId, int nodeId, string nodeType)
        {
            if (!RefreshRunState)
                return;
            if (entityId != SelEntityId)
                return;
            if (!RunningNodes.Contains(nodeId))
                RunningNodes.Add(nodeId);
            //ECSLocate.ECSLog.LogR("节点执行中>>>>>>>>>>>>", entityId,nodeId, nodeType);
        }

        public static void RemoveRunningNode(int nodeId)
        {
            if (RunningNodes.Contains(nodeId))
                RunningNodes.Remove(nodeId);
        }

        public static bool CheckIsRunningNode(int nodeId)
        {
            return RunningNodes.Contains(nodeId);
        }
    }
} 
#endif
