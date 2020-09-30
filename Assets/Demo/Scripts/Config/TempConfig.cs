using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using LCECS;
using LCTileMap;
using UnityEngine;

namespace Demo.Config
{
    /// <summary>
    /// 临时的类
    /// </summary>
    public class TempConfig
    {
        public static int MapSizeX = 30;
        public static int MapSizeY = 10;
        public static Dictionary<Vector2, MapData> MapDict = new Dictionary<Vector2, MapData>();

        public static void Init()
        {
            for (int i = 0; i < 100; i++)
            {
                int mapId = i;
                MapData data = ECSLocate.Factory.GetProduct<MapData>(FactoryType.Asset, null, "Assets/Resources/Map/Map"+mapId+".asset");
                if (data!=null)
                {
                    MapDict.Add(data.MapPos, data);
                }
            }
        }

        public static MapData GetMapData(Vector2 mapPos)
        {
            MapDict.TryGetValue(mapPos, out MapData mapData);
            return mapData;
        }

        //清除字符串括号
        public static string RemoveStrBracket(string str)
        {
            str.Replace("（", "(").Replace("）",")");
            str = Regex.Replace(str.Replace("（", "(").Replace("）", ")"), @"\([^\(]*\)", "");
            return str;
        }
        
    }
}