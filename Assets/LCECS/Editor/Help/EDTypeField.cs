using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LCECS.Help
{
    /// <summary>
    /// 输入框
    /// </summary>
    public class EDTypeField
    {
        public static void CreateLableField(string name, string value,float width,float height)
        {
            EditorGUILayout.LabelField(name,value,GUILayout.Width(width), GUILayout.Height(height));
        }
        
        public static void CreateTypeField(string name, ref object value,Type valueType,float width,float height)
        {
            HandleTypeField(name,ref value,valueType,width,height);
        }

        private static void HandleTypeField(string name, ref object value,Type valueType,float width,float height)
        {
            if (valueType==typeof(int))
            {
                value=EditorGUILayout.IntField(name, (int) value, GUILayout.Width(width), GUILayout.Height(height));
            }
            else if (valueType==typeof(float))
            {
                value=EditorGUILayout.FloatField(name, (float) value, GUILayout.Width(width), GUILayout.Height(height));
            }
            else if (valueType==typeof(double))
            {
                value=EditorGUILayout.DoubleField(name, (double) value, GUILayout.Width(width), GUILayout.Height(height));
            }
            else if (valueType == typeof(bool))
            {
                value=EditorGUILayout.Toggle(name, (bool) value, GUILayout.Width(width), GUILayout.Height(height));
            }
            else if (valueType.BaseType==typeof(Enum))
            {
                value=DrawEnumPopPanel(valueType,value, width, height);
            }
            else if (valueType==typeof(Vector2))
            {
                value=EditorGUILayout.Vector2Field(name,(Vector2)value,GUILayout.Width(width), GUILayout.Height(height));
            }
            else if (valueType == typeof(Vector2Int))
            {
                value = EditorGUILayout.Vector2IntField(name, (Vector2Int)value, GUILayout.Width(width), GUILayout.Height(height));
            }
            else if (valueType==typeof(Vector3))
            {
                value=EditorGUILayout.Vector3Field(name,(Vector3)value,GUILayout.Width(width), GUILayout.Height(height));
            }
            else if (valueType == typeof(Vector3Int))
            {
                value = EditorGUILayout.Vector3IntField(name, (Vector3Int)value, GUILayout.Width(width), GUILayout.Height(height));
            }
            else if (valueType==typeof(string))
            {
                value=EditorGUILayout.TextField(name,value.ToString(),GUILayout.Width(width), GUILayout.Height(height));
            }
            else if (valueType==typeof(Color))
            {
                value=EditorGUILayout.ColorField(name,(Color)value,GUILayout.Width(width), GUILayout.Height(height));
            }
            else
            {
                CreateLableField(name, value.ToString(), width, height);
            }
        }

        private static object DrawEnumPopPanel(Type enumType,object value,float width,float height)
        {
            List<string> enumStrs = new List<string>();

            int selIndex = -1;
            foreach (int index in Enum.GetValues(enumType))
            {
                string strName = Enum.GetName(enumType, index);
                if (strName== value.ToString())
                {
                    selIndex = index;
                }
                enumStrs.Add(strName);
            }
            if (selIndex==-1)
            {
                selIndex = 1;
            }

            selIndex = EditorGUILayout.Popup("枚举:",selIndex, enumStrs.ToArray(),GUILayout.Width(width), GUILayout.Height(height));
            value = Enum.GetName(enumType, selIndex);
            return value;
        }
    }
}