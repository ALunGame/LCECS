using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace LCHelp
{
    /// <summary>
    /// 反射辅助类
    /// </summary>
    public class LCReflect
    {
        private static List<Type> AllTypeItems = null;
        private static void InitCollectAllTypes()
        {
            if (AllTypeItems==null)
            {
                AllTypeItems = new List<Type>();
                Assembly[] ass = AppDomain.CurrentDomain.GetAssemblies();
                for (int i = 0; i < ass.Length; i++)
                {
                    if (ass[i].ManifestModule.Name == "Assembly-CSharp.dll")
                    {
                        AllTypeItems.AddRange(ass[i].GetTypes());
                        return;
                    }
                }
            }
        }

        //运行模式下GetType
        private static Type GetTypeInPlaying(string typeName)
        {
            InitCollectAllTypes();
            for (int i = 0; i < AllTypeItems.Count; i++)
            {
                if (AllTypeItems[i].FullName== typeName)
                {
                    return AllTypeItems[i];
                }
            }
            LCLog.LogError("GetTypeInPlaying 失败>>>>", typeName);
            return null;
        }

        //非运行模式下GetType
        private static Type GetTypeInStop(string typeName)
        {
            Type resType = null;
            //项目程序集
            string asPath = Path.GetFullPath(@".\Library\ScriptAssemblies\Assembly-CSharp.dll");
            Assembly ass = Assembly.LoadFile(@asPath);
            resType = ass.GetType(typeName);
            if (resType == null)
            {
                LCLog.LogError("GetTypeInStop 失败>>>>", typeName);
            }
            return resType;
        }

        /// <summary>
        /// 该方法性能有问题，，请一定谨慎调用
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static Type GetType(string typeName)
        {
            Type resType = null;
            if (typeName.Contains("UnityEngine"))
            {
                resType = Type.GetType(typeName + ",UnityEngine");
                if (resType == null)
                {
                    LCLog.LogError("GetType UnityEngine 失败>>>>", typeName);
                    return null;
                }
                return resType;
            }
            if (typeName.Contains("LCHelp"))
            {
                resType = Type.GetType(typeName + ",LCHelp");
                if (resType == null)
                {
                    LCLog.LogError("GetType LCHelp 失败>>>>", typeName);
                    return null;
                }
                return resType;
            }
            if (typeName.Contains("LCECS"))
            {
                resType = Type.GetType(typeName + ",LCECS");
                if (resType == null)
                {
                    LCLog.LogError("GetType LCECS 失败>>>>", typeName);
                    return null;
                }
                return resType;
            }

            //基础程序集
            resType = Type.GetType(typeName);
            if (resType != null)
            {
                return resType;
            }

            //项目程序集
            if (resType==null)
            {
                if (Application.isPlaying)
                {
                    return GetTypeInPlaying(typeName);
                }
                else
                {
                    return GetTypeInStop(typeName);
                }
            }
            return resType;
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
            Type type = GetType(fullName);
            if (type==null)
            {
                LCLog.LogError("CreateInstanceByType 没有找到指定类型>>>>", fullName);
                return default(T);
            }

            object obj = null;
            try
            {
                obj=Activator.CreateInstance(type, parameters);
            }
            catch (Exception e)
            {
                LCLog.LogError("CreateInstanceByType 异常>>>>", e);
                obj = default(T);
            }
            return (T)obj;
        }

        private static List<Type> GetClassByFunc<T>(Func<Type, Type, bool> func)
        {
            List<Type> resTypes = new List<Type>();

            if (Application.isPlaying)
            {
                InitCollectAllTypes();
                for (int i = 0; i < AllTypeItems.Count; i++)
                {
                    if (func(AllTypeItems[i], typeof(T)))
                    {
                        resTypes.Add(AllTypeItems[i]);
                    }
                }
            }
            else
            {
                if (AllTypeItems!=null)
                    AllTypeItems.Clear();
                //自身程序集
                Assembly ass = Assembly.GetAssembly(typeof(T));
                Type[] types = ass.GetTypes();
                foreach (var type in types)
                {
                    if (func(type, typeof(T)))
                    {
                        resTypes.Add(type);
                    }
                }
                //项目程序集
                string asPath = Path.GetFullPath(@".\Library\ScriptAssemblies\Assembly-CSharp.dll");
                ass = Assembly.LoadFile(@asPath);
                types = ass.GetTypes();
                foreach (var type in types)
                {
                    if (func(type, typeof(T)))
                    {
                        resTypes.Add(type);
                    }
                }
            }
            return resTypes;
        }

        /// <summary>
        /// 获得继承类型的所有类
        /// </summary>
        public static List<Type> GetClassByType<T>()
        {
            return GetClassByFunc<T>((Type ty, Type type) =>
            {
                return ty.IsSubclassOf(type);
            });
        }

        /// <summary>
        /// 获得实现接口的所有类
        /// </summary>
        public static List<Type> GetInterfaceByType<T>()
        {
            return GetClassByFunc<T>((Type ty, Type type) =>
            {
                if (ty.IsInterface)
                {
                    return false;
                }
                Type[] ins = ty.GetInterfaces();
                foreach (Type tyItem in ins)
                {
                    if (tyItem == type)
                    {
                        return true;
                    }
                }
                return false;
            });
        }

        /// <summary>
        /// 获得类型中的特性
        /// </summary>
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
            Type type = GetType(typeName);
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
                LCLog.LogError("设置类中字段值失败", obj.GetType(),fieldName, e);
            }
        }
    }
}
