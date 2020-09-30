using LCECS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LCECS.Layer.Request
{
    /// <summary>
    /// 世界请求 特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class WorldRequestAttribute : Attribute
    {
        private WorldReqId reqId = 0;
        /// <summary>
        /// 世界信息键
        /// </summary>
        public WorldReqId ReqId
        {
            get { return reqId; }
            set { reqId = value; }
        }

        public WorldRequestAttribute(WorldReqId reqId)
        {
            this.reqId = reqId;
        }
    }

    /// <summary>
    /// 实体请求 特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EntityRequestAttribute : Attribute
    {
        private EntityReqId reqId = 0;
        /// <summary>
        /// 世界信息键
        /// </summary>
        public EntityReqId ReqId
        {
            get { return reqId; }
            set { reqId = value; }
        }

        public EntityRequestAttribute(EntityReqId reqId)
        {
            this.reqId = reqId;
        }
    }

    public interface IEntityRequest
    {
        /// <summary>
        /// 自身置换规则
        /// </summary>
        /// <param name="swId">请求置换Id</param>
        /// <param name="resId">自身置换后的Id</param>
        /// <returns>置换规则  不需要自身处理直接 Return ECSDefinition.RESwithRuleSelf</returns>
        int SwitchRequest(int swId,ref int resId);
    }

    public interface IWorldRequest
    {
        /// <summary>
        /// 自身置换规则
        /// </summary>
        /// <param name="swId">请求置换Id</param>
        /// <param name="resId">自身置换后的Id</param>
        /// <returns>置换规则  不需要自身处理直接 Return ECSDefinition.RESwithRuleSelf</returns>
        int SwitchRequest(int swId, ref int resId);
    }
}
