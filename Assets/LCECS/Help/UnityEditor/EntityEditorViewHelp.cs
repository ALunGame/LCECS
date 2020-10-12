#if UNITY_EDITOR
using UnityEngine;

namespace LCECS.Help
{
    /// <summary>
    /// 编辑器下实体预览辅助
    /// </summary>
    public class EntityEditorViewHelp : MonoBehaviour
    {
        public int EntityId = 0;
        public bool DrawEditor = true;
        public Rect ShowRect = new Rect(0, 0, 350, 800);
    }
}

#endif