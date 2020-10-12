using LCECS.Help;
using LCHelp;
using UnityEditor;

namespace LCECS.Inspector
{
    [CustomEditor(typeof(EntityEditorViewHelp))]
    public class EntityConfigInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EDButton.CreateBtn("监视实体决策执行", 150, 50, () =>
            {
                EntityEditorViewHelp help = target as EntityEditorViewHelp;
                NodeRunSelEntityHelp.SelEntityId = help.EntityId;
                NodeRunSelEntityHelp.RefreshRunState = false;
            });

            EDButton.CreateBtn("刷新决策执行", 150, 50, () =>
            {
                NodeRunSelEntityHelp.RunningNodes.Clear();
            });
        }
    }
}
