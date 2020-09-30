using Demo.Com;
using LCECS;
using LCECS.Core.ECS;
using LCECS.Layer.Info;
using UnityEngine;

namespace Demo.Info
{
    [WorldSensor(WorldInfoKey.PlayerInfo)]
    public class PlayerInfoSensor:IWorldSensor
    {
        private PlayerInfoData info=new PlayerInfoData();
        
        private void UpdatePlayerInfo()
        {
            Entity playerEntity = ECSLocate.Player.GetPlayerEntity();
            if (playerEntity==null)
                return;
            GameObjectCom goCom = playerEntity.GetCom<GameObjectCom>();
            info.Pos = goCom.Tran.position;
        }
        
        public T GetInfo<T>(params object[] data) where T : InfoData
        {
            UpdatePlayerInfo();
            return info.As<T>();
        }
    }

    public class PlayerInfoData:InfoData
    {
        public Vector2 Pos = Vector2.zero;
    }
}