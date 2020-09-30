using System.Collections.Generic;
using Demo.Com;
using LCECS;
using LCECS.Core.ECS;
using LCECS.Layer.Info;
using LCTileMap;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace Demo.Info
{
    /// <summary>
    /// 地图信息
    /// </summary>
    [WorldSensor(WorldInfoKey.MapInfo)]
    public class MapSensor:IWorldSensor
    {
        private MapCom mapCom;
        private MapSensorData info = new MapSensorData();

        public T GetInfo<T>(params object[] data) where T : InfoData
        {
            if (mapCom==null)
            {
                mapCom = ECSLocate.ECS.GetGlobalSingleCom<MapCom>();
            }
            UpdateInfo();
            return info.As<T>();
        }

        private void UpdateInfo()
        {
            info.CurrMap = mapCom.CurrMap;
            info.PlayerMapPos = mapCom.PlayerMapPos;
        }
    }

    public class MapSensorData:InfoData
    {
        public MapData CurrMap;
        public Vector2Int PlayerMapPos;
    }
}