﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace LCECS.Help
{
    /// <summary>
    /// 编辑器反射辅助类
    /// </summary>
    public class EDReflectHelp
    {
        public static List<Type> GetAllClassByClass<T>(string assemblyPath="")
        {
            string asPath = assemblyPath;
            if (asPath=="")
            {
                asPath = Path.GetFullPath(@".\Library\ScriptAssemblies\Assembly-CSharp.dll");
            }
            List<Type> resTypes = new List<Type>();

            //获得程序集
            Assembly ass = Assembly.LoadFile(@asPath);
            Type[] typs = ass.GetTypes();

            //Debug.LogError("目标类>>>>>>>" + typeof(T).FullName);
            foreach (Type item in typs)
            {
                
                //获得子类
                if (item.BaseType != null && item.BaseType.FullName == typeof(T).FullName)
                {
                    resTypes.Add(item);
                }
            }
            return resTypes;
        }

        public static List<Type> GetAllClassByClass(string classFullName,string assemblyPath = "")
        {
            string asPath = assemblyPath;
            if (asPath == "")
            {
                asPath = Path.GetFullPath(@".\Library\ScriptAssemblies\Assembly-CSharp.dll");
            }
            List<Type> resTypes = new List<Type>();

            //获得程序集
            Assembly ass = Assembly.LoadFile(@asPath);
            Type[] typs = ass.GetTypes();

            //Debug.LogError("目标类>>>>>>>" + classFullName);
            foreach (Type item in typs)
            {
                //获得子类
                if (item.BaseType != null && item.BaseType.FullName == classFullName)
                {
                    resTypes.Add(item);
                }
            }
            return resTypes;
        }

        public static List<Type> GetAllInterfaceByInterface<T>(string assemblyPath = "")
        {
            string asPath = assemblyPath;
            if (asPath == "")
            {
                asPath = Path.GetFullPath(@".\Library\ScriptAssemblies\Assembly-CSharp.dll");
            }
            List<Type> resTypes = new List<Type>();

            //获得程序集
            Assembly ass = Assembly.LoadFile(@asPath);
            Type[] typs = ass.GetTypes();

            //Debug.LogError("目标类>>>>>>>" + typeof(T).FullName);
            foreach (Type item in typs)
            {
                //获得接口
                if (item.GetInterface(typeof(T).FullName) != null && item.IsAbstract)
                {
                    resTypes.Add(item);
                }
            }
            return resTypes;
        }

        public static List<Type> GetAllInterfaceByInterface(string interfaceFullName,string assemblyPath = "")
        {
            string asPath = assemblyPath;
            if (asPath == "")
            {
                asPath = Path.GetFullPath(@".\Library\ScriptAssemblies\Assembly-CSharp.dll");
            }
            List<Type> resTypes = new List<Type>();

            //获得程序集
            Assembly ass = Assembly.LoadFile(@asPath);
            Type[] typs = ass.GetTypes();

            //Debug.LogError("目标类>>>>>>>" + typeof(T).FullName);
            foreach (Type item in typs)
            {
                //获得接口
                if (item.GetInterface(interfaceFullName) != null && item.IsAbstract)
                {
                    resTypes.Add(item);
                }
            }
            return resTypes;
        }

        public static Type GetTypeByFullName(string fullName, string assemblyPath = "")
        {
            if (fullName=="")
            {
                return null;
            }
            if (fullName.Contains("UnityEngine"))
            {
                return Type.GetType(fullName + ",UnityEngine");
            }
            string asPath = assemblyPath;
            if (asPath == "")
            {
                asPath = Path.GetFullPath(@".\Library\ScriptAssemblies\Assembly-CSharp.dll");
            }
            List<Type> resTypes = new List<Type>();

            //获得程序集
            Assembly ass = Assembly.LoadFile(@asPath);
            Type typ = ass.GetType(fullName);
            return typ;
        }
    }
}
