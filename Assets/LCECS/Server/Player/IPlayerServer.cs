using LCECS.Core.ECS;
using LCECS.Data;
using UnityEngine;

namespace LCECS.Server.Player
{
    /// <summary>
    /// 玩家服务
    /// </summary>
    public interface IPlayerServer
    {
		GameObject GetPalyerGo();

        void CreatePlayerEntity(string entityName,ref GameObject gameObject);

        Entity GetPlayerEntity();

        ParamData GetReqParam(EntityReqId reqId);

        void PushPlayerReq(EntityReqId reqId);
    }
}
