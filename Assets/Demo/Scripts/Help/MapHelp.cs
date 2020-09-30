using LCTileMap;
using System.Collections.Generic;
using UnityEngine;

namespace Demo.Help
{
    /// <summary>
    /// 地图辅助
    /// </summary>
    public class MapHelp
    {
        /// <summary>
        /// 获得俩点间有多少地图格
        /// </summary>
        public static int GetTwoPointHasGridCnt(Vector2Int point1,Vector2Int point2)
        {
            int xOff = point1.x = point2.x;
            xOff = xOff < 0 ? -xOff : xOff;
            int yOff = point1.x = point2.x;
            yOff = yOff < 0 ? -yOff : yOff;
            return yOff> xOff? yOff: xOff;
        }

        //点和范围，，获得一个区域
        public static Rect GetArea(Vector2Int point, Vector2Int area)
        {
            //左下点
            Vector2 leftDownPoint = new Vector2(point.x - (float)area.x/2, point.y - (float)area.y/2);
            Rect rect = new Rect(leftDownPoint, area);
            return rect;
        }

        /// <summary>
        /// 检测一个点是否在区域里
        /// </summary>
        public static bool CheckPointInArea(Vector2Int point, Vector2Int area, Vector2Int chekPoint)
        {
            Rect areaRect = GetArea(point, area);
            return areaRect.Contains(chekPoint);
        }
    }
}
