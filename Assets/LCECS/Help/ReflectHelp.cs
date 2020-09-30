using System;
using System.Collections.Generic;
using System.Reflection;

namespace LCECS.Help
{
    /// <summary>
    /// 反射辅助类
    /// </summary>
    public class ReflectHelp
    {
        /// <summary>
        /// 创建对应实例
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="type">实例类型</param>
        /// <param name="parameters">构造参数</param>
        /// <returns>返回类型</returns>
        public static T CreateInstanceByType<T>(Type type, params object[] parameters)
        {
            object obj = Activator.CreateInstance(type, parameters);
            return (T)obj;
        }

        /// <summary>
        /// 创建对应实例
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="fullName">实例类型全名</param>
        /// <param name="parameters">构造参数</param>
        /// <returns>返回类型</returns>
        public static T CreateInstanceByType<T>(string fullName,params object[] parameters)
        {
            Type type = Type.GetType(fullName);
            ECSLocate.ECSLog.Log("反射创建对应实例", fullName);

            if (type==null)
            {
                ECSLocate.ECSLog.LogError("反射创建对应实例失败,没有找到对应的类型", fullName);
                return default(T);
            }

            object obj = null;
            try
            {
                obj=Activator.CreateInstance(type, parameters);
            }
            catch (Exception e)
            {
                ECSLocate.ECSLog.LogError("反射创建对应实例失败", fullName, e);
                obj=default(T);
            }
            return (T)obj;
        }

        /// <summary>
        /// 获得继承类型的所有类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<Type> GetClassByType<T>()
        {
            List<Type> resTypes = new List<Type>();

            Assembly ass = Assembly.GetAssembly(typeof(T));
            Type[] types = ass.GetTypes();
            foreach (var type in types)
            {
                if (type.IsSubclassOf(typeof(T)))
                {
                    resTypes.Add(type);
                }
            }
            return resTypes;
        }

        /// <summary>
        /// 获得继承接口的所有类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<Type> GetInterfaceByType<T>()
        {
            List<Type> resTypes = new List<Type>();

            Assembly ass = Assembly.GetAssembly(typeof(T));

            Type[] types = ass.GetTypes();
            foreach (Type item in types)
            {
                if (item.IsInterface) continue;//判断是否是接口
                Type[] ins = item.GetInterfaces();
                foreach (Type ty in ins)
                {
                    if (ty == typeof(T))
                    {
                        resTypes.Add(item);
                    }
                }
            }
            return resTypes;
        }

        /// <summary>
        /// 获得类型中的特性
        /// </summary>
        /// <typeparam name="T">特性</typeparam>
        /// <param name="type">类型</param>
        /// <returns>特性</returns>
        public static T GetTypeAttr<T>(Type type) where T: Attribute
        {
            var something = type.GetCustomAttributes(typeof(T), true);
            foreach (object obj in something)
            {
                T attr = obj as T;
                if (attr != null)
                    return attr;
            }

            return null;
        }

        /// <summary>
        /// 获取类中所有字段
        /// </summary>
        public static FieldInfo[] GetTypeFieldInfos(Type type)
        {
            FieldInfo[] fields = type.GetFields();
            return fields;
        }
        
        /// <summary>
        /// 获取类中字段指定特性
        /// </summary>
        public static T GetFieldAttr<T>(FieldInfo info) where T: Attribute
        {
            var something = info.GetCustomAttributes(typeof(T), true);
            foreach (object obj in something)
            {
                T attr = obj as T;
                if (attr != null)
                    return attr;
            }
            return null;
        }
        
        /// <summary>
        /// 获取类中字段值
        /// </summary>
        public static T GeTypeFieldValue<T>(object obj, string fieldName)
        {
            FieldInfo info = obj.GetType().GetField(fieldName);
            return (T)info.GetValue(obj);
        }

        /// <summary>
        /// 获取类中默认字段值
        /// </summary>
        public static object GeTypeDefaultFieldValue(string typeName,string fieldName)
        {
            Type type = Type.GetType(typeName);
            object res = Activator.CreateInstance(type);
            FieldInfo info = res.GetType().GetField(fieldName);
            return info.GetValue(res);
        }

        /// <summary>
        /// 设置类中字段值
        /// </summary>
        public static void SetTypeFieldValue(object obj, string fieldName, object value)
        {
            if (obj==null)
            {
                return;
            }

            try
            {
                FieldInfo info = obj.GetType().GetField(fieldName);
                info.SetValue(obj, value);
            }
            catch (Exception e)
            {
                ECSLocate.ECSLog.LogError("设置类中字段值失败", obj.GetType(),fieldName, e);
            }
        }

        /// <summary>
        /// 将字符串转为指定类型的值
        /// </summary>
        public static T StrChangeTo<T>(string value)
        {
            T result = default;
            result = (T)Convert.ChangeType(value, typeof(T));
            return result;
        }

        /// <summary>
        /// 将字符串转为指定类型
        /// </summary>
        public static object StrChangeToObject(string value, string typeName)
        {
            object res = null;
            Type ty = GetType(typeName);
            if (value=="")
            {
                res = Activator.CreateInstance(ty);
            }
            else
            {
                //枚举特殊处理
                if (ty.BaseType==typeof(Enum))
                {
                    res = Enum.Parse(ty, value);
                }
                else if (ty.Namespace == "UnityEngine")
                {
                    res = ConvertUnity.ConvertToUnityType(ty, value);
                }
                else
                {
                    res = Convert.ChangeType(value, ty);
                }
            }
            return res;
        }

        /// <summary>
        /// 将字符串转为指定类型
        /// </summary>
        public static object StrChangeToObject(string value, Type type)
        {
            object res = null;
            Type ty = type;
            if (value == "")
            {
                res = Activator.CreateInstance(ty);
            }
            else
            {
                //枚举特殊处理
                if (ty.BaseType == typeof(Enum))
                {
                    res = Enum.Parse(ty, value);
                }
                else if (ty.Namespace== "UnityEngine")
                {
                    res = ConvertUnity.ConvertToUnityType(ty, value);
                }
                else
                {
                    res = Convert.ChangeType(value, ty);
                }
            }
            return res;
        }

        public static Type GetType(string typeName)
        {
            if (typeName.Contains("UnityEngine"))
            {
                return Type.GetType(typeName + ",UnityEngine");
            }
            return Type.GetType(typeName);
        } 
    }
}
