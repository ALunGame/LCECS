using LCECS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LCECS.Server.Config
{
    public interface IConfigServer
    {
        /// <summary>
        /// 设置决策树
        /// </summary>
        void SetDecTrees(DecTrees trees);

        /// <summary>
        /// 设置行为树
        /// </summary>
        void SetBevTrees(BevTrees trees);

        /// <summary>
        /// 设置实体配置
        /// </summary>
        void SetEntityConf(EntityJsonList entityConf);

        /// <summary>
        /// 设置请求权重配置
        /// </summary>
        void SetReqWeightConf(ReqWeightJson reqWeightConf);

        /// <summary>
        /// 获得请求权重
        /// </summary>
        int GetRequestWeight(int reqId,bool isEntity);

        /// <summary>
        /// 获得实体数据
        /// </summary>
        EntityJson GetEntityData(string entityName);
    }
}
