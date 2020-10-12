using Demo.Config;
using Demo.Info;
using LCECS;
using LCECS.Core.ECS;
using LCECS.Data;
using LCHelp;
using UnityEngine;

namespace Demo
{
    public class GameStart:MonoBehaviour
    {
        public bool DrawGizmos=false;
        public GameObject PlayerStart;
        private Entity PlayerEntity;

        private void Awake()
        {
            TempConfig.Init();
        }

        private void Start()
        {
            //创建玩家实体
            GameObject entityGo = null;
            ECSLocate.Player.CreatePlayerEntity("Player", ref entityGo);
            entityGo.transform.position = PlayerStart.transform.position;
            PlayerEntity = ECSLocate.Player.GetPlayerEntity();

            //创建全局系统
            ECSLocate.ECS.CreateEntity("MapSystem",    new GameObject("MapSystem"));
            ECSLocate.ECS.CreateEntity("CameraSystem", new GameObject("CameraSystem"));
            ECSLocate.ECS.CreateEntity("EffectSystem", new GameObject("EffectSystem"));
        }

		private void Update()
        {
            //时间线
            TimeLineHelp.Update();
            ComputeVelocity();
            OnClickNormalAttack();
        }

        protected void ComputeVelocity()
        {
            
            #region 点击加速
            // Vector2 move=Vector2.zero;
            // if (Input.GetKeyDown(KeyCode.A))
            // {
            //     move.x = 1;
            // }
            // if (Input.GetButtonDown("Jump"))
            // {
            //     move.y = 1;
            // }
            
            // ParamData paramData = ECSLocate.Player.GetReqParam(EntityReqId.PlayerMove);
            // paramData.SetVect2(move);
            // ECSLocate.Player.PushPlayerReq(EntityReqId.PlayerMove);

            #endregion
            
            #region 按键速度

            Vector2 move = Vector2.zero;
            bool dash=false;
            
            move.x = Input.GetAxisRaw("Horizontal");
            
            if (Input.GetButtonDown("Jump"))
            {
                move.y = 1;
            }

            if (Input.GetMouseButtonDown(0))
            {
                dash=true;
            }
            
            ParamData paramData = ECSLocate.Player.GetReqParam(EntityReqId.PlayerMove);
            paramData.SetVect2(move);
            paramData.SetBool(dash);
            ECSLocate.Player.PushPlayerReq(EntityReqId.PlayerMove);

            #endregion
        }

        //普通攻击
        private void OnClickNormalAttack()
        {
            if (Input.GetMouseButtonDown(0))
            {
                ParamData paramData = ECSLocate.Player.GetReqParam(EntityReqId.PlayerNormalAttack);
                paramData.SetBool(true);
                ECSLocate.Player.PushPlayerReq(EntityReqId.PlayerNormalAttack);
            }
        }

        private void OnDrawGizmos()
        {
            if (DrawGizmos==false)
            {
                return;
            }
            if (!Application.isPlaying)
            {
                return;
            }
            PlayerInfoData playerInfo = ECSLayerLocate.Info.GetWorldInfo<PlayerInfoData>(WorldInfoKey.PlayerInfo);

            //地图检测
            DrawMapCheckBox(playerInfo.Pos);
            
            //背景检测
            DrawBgCheckBox(playerInfo.Pos);
        }

        private void DrawMapCheckBox(Vector3 playerPos)
        {
            Vector3 pos  = new Vector3(playerPos.x-((float)TempConfig.MapSizeX/2),playerPos.y-((float)TempConfig.MapSizeY/2)); 
            Vector3 size = new Vector3(TempConfig.MapSizeX, TempConfig.MapSizeY);
            Rect mapCheck= new Rect(pos,size);
            EDGizmos.DrawRect(mapCheck,Color.green);
        }

        private void DrawBgCheckBox(Vector3 playerPos)
        {
            Vector3 pos  = new Vector3(playerPos.x-45,-10); 
            Vector3 size = new Vector3(90,20);
            Rect bgCheck = new Rect(pos,size);
            EDGizmos.DrawRect(bgCheck,Color.blue);
        }

    }
}
