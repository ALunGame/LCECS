using System;
using System.Collections.Generic;
using Demo.Config;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LCTileMap
{
    /// <summary>
    /// 地图
    /// </summary>
    public class MapData:ScriptableObject
    {
        [HideInInspector]
        public string MapName = "";
        public GameObject MapAs = null;

        //障碍物坐标
        public List<Vector2Int> ObstaclePos = new List<Vector2Int>();
        //道路坐标
        public List<Vector2Int> RoadPos = new List<Vector2Int>();
        
        //物体信息
        public List<GoInfo> MonsterList=new List<GoInfo>();
        public List<GoInfo> BossList=new List<GoInfo>();
        public List<GoInfo> NPCList=new List<GoInfo>();
        
        //位置信息
        public Vector2Int MapPos = Vector2Int.zero;
        public Rect MapRect;

        public void Init()
        {
            MapRect = new Rect(MapPos.x, MapPos.y, TempConfig.MapSizeX, TempConfig.MapSizeY);
        }
    }

    /// <summary>
    /// 物体
    /// </summary>
    [Serializable]
    public class GoInfo
    {
        public string GoEntityName = "";
        public int GoIndex = 0;
        public Vector3 Pos        = Vector3.zero;
    }
}