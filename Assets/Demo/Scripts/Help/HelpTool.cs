﻿using UnityEngine;

namespace Demo.Help
{
    /// <summary>
    /// 辅助工具
    /// </summary>
    public class HelpTool
    {
        public static void GizmosDrawRect(Rect rect,Color color)
        {
            Vector3[] line =new Vector3[5];

            line[0] = new Vector3(rect.x,rect.y,0);

            line[1] = new Vector3(rect.x+rect.width, rect.y, 0);

            line[2] = new Vector3(rect.x + rect.width, rect.y + rect.height, 0);

            line[3] = new Vector3(rect.x, rect.y + rect.height, 0);

            line[4] = new Vector3(rect.x, rect.y, 0);

            GizmosDrawLine(line, color);
        }
        
        private static void GizmosDrawLine(Vector3[] line, Color color)
        {
            if(line == null && line.Length <= 0)
                return;
            Gizmos.color = color;
            for (int i = 0; i < line.Length - 1; i++)
            {

                Gizmos.DrawLine(line[i], line[i + 1]);
            }
        }
    }
}