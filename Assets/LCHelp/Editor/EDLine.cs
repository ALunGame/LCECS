using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace LCHelp
{
    /// <summary>
    /// 编辑器画线
    /// </summary>
    public class EDLine
    {
        /// <summary>
        /// 画贝赛尔曲线
        /// </summary>
        public static void CreateBezierLine(Vector3 startPosition, Vector3 endPosition,float width,Color color = default(Color), Texture2D texture=null)
        {
            Handles.DrawBezier(startPosition,endPosition,startPosition - Vector3.left * 50f,endPosition + Vector3.left * 50f,color,texture,width);
        }
    }
}
