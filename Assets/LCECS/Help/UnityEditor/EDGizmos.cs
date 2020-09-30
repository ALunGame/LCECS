using UnityEngine;

namespace LCECS.Help
{
    /// <summary>
    /// 辅助工具
    /// </summary>
    public static class EDGizmos
    {
        public static void DrawRect(Rect rect,Color color)
        {
            Vector3[] line =new Vector3[5];

            line[0] = new Vector3(rect.x,rect.y,0);

            line[1] = new Vector3(rect.x + rect.width,rect.y,0);

            line[2] = new Vector3(rect.x + rect.width,rect.y + rect.height,0);

            line[3] = new Vector3(rect.x,rect.y + rect.height,0);

            line[4] = new Vector3(rect.x,rect.y,0);

            DrawLines(line, color);
        }

        public static void DrawLine(Vector3 start,Vector3 end,Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawLine(start,end);
        }
        
        private static void DrawLines(Vector3[] line, Color color)
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