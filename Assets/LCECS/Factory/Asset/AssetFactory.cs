using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LCECS.Server.Factory
{
    /// <summary>
    /// 加载资源
    /// </summary>
    public class AssetFactory : IFactory<Object>
    {
        public Object CreateProduct(Action<object[]> func, params object[] data)
        {
			if (data==null)
				return null;

			string path = data[0].ToString();
            if (path.Contains("Resources"))
            {
                int startIndex = path.IndexOf("Resources/");
                int endIndex = path.IndexOf(".");
                path = path.Substring(startIndex, endIndex-startIndex);
                path = path.Replace("Resources/", "");

                ECSLocate.ECSLog.Log("加载资源路径>>>>>>", path, startIndex, endIndex);
                return Resources.Load<Object>(path);
            }

            ECSLocate.ECSLog.Log("不是Resources资源，，，在这里添加其他的加载方案>>>>>>", path);
            return null;
        }
    }
}
