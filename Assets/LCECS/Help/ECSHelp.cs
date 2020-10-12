using LCECS.Core.ECS;
using LCHelp;
using System;

namespace LCECS.Help
{
    /// <summary>
    /// ECS辅助函数
    /// </summary>
    public class ECSHelp
    {
        /// <summary>
        /// 检测组件是否是全局单一
        /// </summary>
        /// <param name="comType"></param>
        /// <returns></returns>
        public static bool CheckComIsGlobal(Type comType)
        {
            if (comType==null)
            {
                return false;
            }
            ComAttribute comAttribute = LCReflect.GetTypeAttr<ComAttribute>(comType);
            if (comAttribute==null)
            {
                return false;
            }
            return comAttribute.IsGlobal;
        }
    } 
}
