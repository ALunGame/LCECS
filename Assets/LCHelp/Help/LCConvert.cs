using System;
using UnityEngine;

namespace LCHelp
{
    /// <summary>
    /// Unity类型转换
    /// </summary>
    public class LCConvertUnity
    {
        //解析Vector类型的值
        private static string[] ParseVectorValue(string value)
        {
            value = value.Replace("(", "").Replace(")", "");
            string[] values = value.Split(',');
            return values;
        }

        public static Vector2 ParseVector2(string value)
        {
            string[] values = ParseVectorValue(value);
            Vector2 vector = Vector2.zero;
            try
            {
                vector= new Vector2(float.Parse(values[0]), float.Parse(values[1]));
            }
            catch (Exception e)
            {
                vector = Vector2.zero;
            }
            return vector;
        }

        public static Vector2Int ParseVector2Int(string value)
        {
            string[] values = ParseVectorValue(value);
            Vector2Int vector = Vector2Int.zero;
            try
            {
                vector = new Vector2Int(int.Parse(values[0]), int.Parse(values[1]));
            }
            catch (Exception e)
            {
                vector = Vector2Int.zero;
            }
            return vector;
        }

        public static Vector3 ParseVector3(string value)
        {
            string[] values = ParseVectorValue(value);
            Vector3 vector = Vector3.zero;
            try
            {
                vector = new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
            }
            catch (Exception e)
            {
                vector = Vector3.zero;
            }
            return vector;
        }

        public static Vector3Int ParseVector3Int(string value)
        {
            string[] values = ParseVectorValue(value);
            Vector3Int vector = Vector3Int.zero;
            try
            {
                vector = new Vector3Int(int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]));
            }
            catch (Exception e)
            {
                vector = Vector3Int.zero;
            }
            return vector;
        }

        public static object ConvertToUnityType(Type type,string value)
        {
            if (type==typeof(Vector2))
                return ParseVector2(value);
            else if (type == typeof(Vector2Int))
                return ParseVector2Int(value);
            else if (type == typeof(Vector3))
                return ParseVector3(value);
            else if (type == typeof(Vector3Int))
                return ParseVector3Int(value);
            return null;
        }
    }


    /// <summary>
    /// 类型转换
    /// </summary>
    public class LCConvert
    {
        public static object StrChangeToObject(string value, string typeName)
        {
            object res  = null;
            Type   type = LCReflect.GetType(typeName);
            if (value == "")
            {
                res = Activator.CreateInstance(type);
            }
            else
            {
                //枚举特殊处理
                if (type.BaseType == typeof(Enum))
                {
                    res = Enum.Parse(type, value);
                }
                else if (type.Namespace == "UnityEngine")
                {
                    res = LCConvertUnity.ConvertToUnityType(type, value);
                }
                else
                {
                    res = Convert.ChangeType(value, type);
                }
            }
            return res;
        }
    }
}
