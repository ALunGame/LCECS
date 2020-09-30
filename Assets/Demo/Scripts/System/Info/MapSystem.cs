using System;
using System.Collections.Generic;
using System.Linq;
using Demo.Com;
using Demo.Config;
using Demo.Info;
using LCECS;
using LCECS.Core.ECS;
using LCTileMap;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Demo.System
{
    public class MapSystem:BaseSystem
    {
        //怪物节点
        private string MonsterName = "Monster";
        private string BossName    = "Boss";
        private string NPCName     = "NPC";

        private MapCom MapCom;
        private GameObjectCom GoCom;

        protected override List<Type> RegListenComs()
        {
            return new List<Type>(){typeof(MapCom), typeof(GameObjectCom) };
        }

        protected override void HandleComs(List<BaseCom> comList)
        {
            if (MapCom==null)
                MapCom = GetCom<MapCom>(comList[0]);
            if (GoCom == null)
                GoCom = GetCom<GameObjectCom>(comList[1]);
            PlayerInfoData playerInfo = ECSLayerLocate.Info.GetWorldInfo<PlayerInfoData>(WorldInfoKey.PlayerInfo);
            RefreshMap(playerInfo.Pos);
            UpdatePlayerMapPos(playerInfo.Pos);
        }

        #region 地图刷新相关
        //刷新地图
        private void RefreshMap(Vector2 playerPos)
        {
            //当前的地图
            MapData currMap = null;
            foreach (MapData item in TempConfig.MapDict.Values)
            {
                if (item.MapRect.Contains(playerPos))
                {
                    currMap = item;
                    break;
                }
            }

            if (currMap == null)
            {
                ECSLocate.ECSLog.LogError("没有检测到可以显示的地图块>>>>>>", playerPos);
                return;
            }

            //相同的
            if (MapCom.CurrMap != null && MapCom.CurrMap.MapPos == currMap.MapPos)
            {
                return;
            }
            ChangeCurrMap(currMap);

            //其他需要显示的
            Vector2[] otherShowMaps = GetOtherMaps(currMap);

            //回收
            foreach (Vector2 mapPos in new List<Vector2>(MapCom.CurrShowMapDict.Keys))
            {
                if (mapPos != currMap.MapPos && !otherShowMaps.Contains(mapPos))
                {
                    ECSLocate.ECSLog.LogError("回收》》》》》》》",mapPos);
                    HideMap(mapPos);
                }
            }

            //显示
            for (int i = 0; i < otherShowMaps.Length; i++)
            {
                if (!MapCom.CurrShowMapDict.ContainsKey(otherShowMaps[i]))
                {
                    ShowMap(otherShowMaps[i]);
                }
            }
        }

        //改变当前地图时
        private void ChangeCurrMap(MapData curMap)
        {
            ECSLocate.ECSLog.LogR("当前地图为：", MapCom.CurrMap);
            ECSLocate.ECSLog.LogR("改变的地图为：", curMap.MapPos.ToString());

            if (MapCom.CurrMap == null)
            {
                MapCom.CurrMap = curMap;
#if UNITY_EDITOR
                MapCom.CurrMapName = curMap.MapName;
#endif
                ShowMap(curMap.MapPos);

                //通知地图块内的所有实体，地图块开启
                List<Entity> firstEntitys = MapCom.EntityDict[MapCom.CurrMap.MapPos];
                for (int i = 0; i < firstEntitys.Count; i++)
                {
                    firstEntitys[i].Enable();
                }
                return;
            }

            //清理旧的
            HideMap(MapCom.CurrMap.MapPos);
            //通知地图块内的所有实体，地图块关闭
            List<Entity> oldEntitys = MapCom.EntityDict[MapCom.CurrMap.MapPos];
            for (int i = 0; i < oldEntitys.Count; i++)
            {
                oldEntitys[i].Disable();
            }

            //创建新的
            MapCom.CurrMap = curMap;
#if UNITY_EDITOR
            MapCom.CurrMapName = curMap.MapName;
#endif
            ShowMap(curMap.MapPos);
            //通知地图块内的所有实体，地图块开启
            List<Entity> newEntitys = MapCom.EntityDict[MapCom.CurrMap.MapPos];
            for (int i = 0; i < newEntitys.Count; i++)
            {
                newEntitys[i].Enable();
            }
        }

        //获得其余范围的地图块
        private Vector2[] GetOtherMaps(MapData currMap)
        {
            Vector2 curPos = currMap.MapPos;
            Vector2[] others = new Vector2[8];
            //上下左右
            others[0] = new Vector2(curPos.x - TempConfig.MapSizeX, curPos.y);
            others[1] = new Vector2(curPos.x + TempConfig.MapSizeX, curPos.y);
            others[2] = new Vector2(curPos.x, curPos.y - TempConfig.MapSizeY);
            others[3] = new Vector2(curPos.x, curPos.y + TempConfig.MapSizeY);
            //斜方向
            others[4] = new Vector2(curPos.x - TempConfig.MapSizeX, curPos.y + TempConfig.MapSizeY);
            others[5] = new Vector2(curPos.x - TempConfig.MapSizeX, curPos.y - TempConfig.MapSizeY);
            others[6] = new Vector2(curPos.x + TempConfig.MapSizeX, curPos.y + TempConfig.MapSizeY);
            others[7] = new Vector2(curPos.x + TempConfig.MapSizeX, curPos.y - TempConfig.MapSizeY);
            return others;
        }

        //隐藏地图
        private void HideMap(Vector2 showMap)
        {
            ECSLocate.ECSLog.LogR("隐藏地图》》》》》》", showMap);
            MapCom.CurrShowMapDict.TryGetValue(showMap, out GameObject oldCurrGo);
            if (oldCurrGo != null)
            {
                oldCurrGo.SetActive(false);
                MapCom.CurrShowMapDict.Remove(showMap);
                MapCom.RecycleMapDict.Add(showMap, oldCurrGo);
            }
        }

        //显示地图
        private void ShowMap(Vector2 showMap)
        {
            GameObject mapGo = null;
            //回收
            if (MapCom.RecycleMapDict.ContainsKey(showMap))
            {
                mapGo = MapCom.RecycleMapDict[showMap];
                mapGo.SetActive(true);
                MapCom.RecycleMapDict.Remove(showMap);
            }
            else
            {
                if (MapCom.CurrShowMapDict.ContainsKey(showMap))
                {
                    return;
                }
                if (TempConfig.MapDict.ContainsKey(showMap))
                {
                    mapGo = CreateMap(showMap);
                    mapGo.SetActive(true);
                    
                }
            }
            if (mapGo!=null)
            {
                MapCom.CurrShowMapDict.Add(showMap, mapGo);
            }
        }

        //创建地图
        private GameObject CreateMap(Vector2 mapPos)
        {
            MapData mapData = TempConfig.MapDict[mapPos];
            GameObject mapGo = ImportMap(mapData);
            mapGo.name = mapPos.ToString();
            return mapGo;
        }

        //导入地图
        private GameObject ImportMap(MapData data)
        {
            GameObject gird = Object.Instantiate(data.MapAs);
            gird.transform.SetParent(GoCom.Tran);
            gird.name = data.MapName;

            //物体导入
            ImportGo(data, gird.transform);

            //位置更新
            gird.transform.localPosition = new Vector3(data.MapPos.x, data.MapPos.y, 0);
            gird.SetActive(true);

            return gird;
        }

        //导入地图物体
        private void ImportGo(MapData data, Transform root)
        {

            //实体列表
            List<Entity> entities = new List<Entity>();

            //Monster
            Transform goRoot = root.transform.Find(MonsterName);
            CreateGoEntity(goRoot, data.MonsterList, entities, data.MapPos);

            //Boss
            goRoot = root.transform.Find(BossName);
            CreateGoEntity(goRoot, data.BossList, entities, data.MapPos);

            //NPC
            goRoot = root.transform.Find(NPCName);
            CreateGoEntity(goRoot, data.NPCList, entities, data.MapPos);

            //保存
            MapCom.EntityDict.Add(data.MapPos, entities);
        }

        private void CreateGoEntity(Transform goRoot,List<GoInfo> goInfos, List<Entity> entities, Vector2Int mapPos)
        {
            //默认都是先不激活状态
            Entity entity = null;
            GameObject childgo = null;
            for (int i = 0; i < goInfos.Count; i++)
            {
                GoInfo info = goInfos[i];
                childgo = goRoot.transform.GetChild(info.GoIndex).gameObject;
                entity = ECSLocate.ECS.CreateEntity(info.GoEntityName, childgo);
                entity.Disable();
                entity.GetCom<SeekPathCom>().MapPos = mapPos;
                entities.Add(entity);
            }
        }

        #endregion

        #region 玩家地图信息相关
        private void UpdatePlayerMapPos(Vector2 playerPos)
        {
            if (MapCom.CurrMap==null)
            {
                ECSLocate.ECSLog.LogError("玩家地图位置更新失败，", playerPos.ToString());
                return;
            }

            Vector2Int playerPosInt = new Vector2Int(Mathf.FloorToInt(playerPos.x), Mathf.FloorToInt(playerPos.y));
            MapCom.PlayerMapPos = playerPosInt - MapCom.CurrMap.MapPos;
        }
        #endregion
    }
}