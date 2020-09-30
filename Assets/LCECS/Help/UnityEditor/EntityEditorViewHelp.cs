using Demo.Com;
using Demo.Help;
using LCECS.Core.ECS;
using LCECS.Data;
using UnityEngine;

namespace LCECS.Help
{
#if UNITY_EDITOR
    /// <summary>
    /// 编辑器下实体预览辅助
    /// </summary>
    public class EntityEditorViewHelp : MonoBehaviour
    {
        public int EntityId = 0;
        public bool DrawEditor = true;
        public Rect ShowRect=new Rect(0,0,350,800);

        private Entity Entity;
        private EntityJson Json;

        private void Awake()
        {
            
        }

        private void OnDrawGizmos()
        {
            if (Entity == null || Json==null)
            {
                Entity = ECSLocate.ECS.GetEntity(EntityId);
                Json = ECSLocate.Config.GetEntityData(Entity.GetEntityName());
            }
            if (Entity == null || Json == null)
                return;
            //临时代码（应该抽离出来，，，框架中应该不含有Demo代码）
            //TODO
            if (Json.Group== EntityDecGroup.Enemy)
            {
                DrawEnemy();
            }
        }

        //绘制敌人
        private void DrawEnemy()
        {
            EnemyCom enemyCom = Entity.GetCom<EnemyCom>();
            SeekPathCom seekPathCom = Entity.GetCom<SeekPathCom>();

            Vector2Int worldPos = seekPathCom.CurrPos + seekPathCom.MapPos;
            Rect guardRect = MapHelp.GetArea(worldPos, enemyCom.GuardArea);
            HelpTool.GizmosDrawRect(guardRect, Color.red);
        }
    } 
#endif
}
