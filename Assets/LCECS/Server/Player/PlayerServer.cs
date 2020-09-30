using LCECS.Core.ECS;
using UnityEngine;
using LCECS.Data;

namespace LCECS.Server.Player
{
    public class PlayerServer : IPlayerServer
    {
        private Entity playerEntity;
		private GameObject playerGo;
		private EntityWorkData playerWorkData;

        public void CreatePlayerEntity(string entityName, ref GameObject gameObject)
        {
            Entity entity = ECSLocate.ECS.CreateEntity(entityName, ref gameObject);
            playerEntity  = entity;
			playerGo	  = gameObject;

			playerWorkData = ECSLayerLocate.Info.GetEntityWorkData(entity.GetHashCode());
        }

        public Entity GetPlayerEntity()
        {
	        return playerEntity;
        }

        public GameObject GetPalyerGo()
		{
			return playerGo;
		}

		//请求参数
		public ParamData GetReqParam(EntityReqId reqId)
        {
            return playerWorkData.GetReqParam(reqId);
        }

        //请求
        public void PushPlayerReq(EntityReqId reqId)
        {
            ECSLayerLocate.Request.PushEntityRequest(playerEntity.GetHashCode(), reqId);
        }
    }
}
