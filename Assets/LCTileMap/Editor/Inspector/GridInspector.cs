using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Demo.Config;
using Demo.Help;
using LCECS.Help;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LCTileMap.Window
{
    [CustomEditor(typeof(Grid))]
    public class GridInspector:Editor
    {
        private Grid TargetGrid;

        private bool ClearOtherTile=true;
        
        //检测的瓦片种类
        private string BaseTile = typeof(Tile).FullName;
        private string RuleTile = typeof(RuleTile).FullName;
        
        //默认地图大小
        private int DefaultXSize = TempConfig.MapSizeX;
        private int DefaultYSize = TempConfig.MapSizeY;
        private int DefaultTileSize = 56;
        
        private int DefaultColliderSize = 350;
        
        //怪物节点
        private string MonsterName = "Monster";
        private string BossName    = "Boss";
        private string NPCName     = "NPC";

        //瓦片图
        private Sprite TileSp = null;
        
        //碰撞辅助图
        private string ColliderName     = "Colliders";
        private Sprite ColliderSp = null;
        
        private void OnEnable()
        {
            Texture2D t = new Texture2D(DefaultTileSize, DefaultTileSize);
            for (int w = 0; w < DefaultTileSize; w++){
                for (int h = 0; h < DefaultTileSize; h ++){
                    t.SetPixel(w,h,Color.grey);
                }
            }
            t.Apply();
            TileSp=Sprite.Create(t, new Rect(0, 0, 56, 56), new Vector2(0.5f, 0.5f));
            
            Texture2D colliderTexture = new Texture2D(DefaultColliderSize, DefaultColliderSize);
            for (int w = 0; w < DefaultColliderSize; w++){
                for (int h = 0; h < DefaultColliderSize; h ++){
                    colliderTexture.SetPixel(w,h,Color.gray);
                }
            }
            colliderTexture.Apply();
            ColliderSp=Sprite.Create(colliderTexture, new Rect(0, 0, DefaultColliderSize, DefaultColliderSize), new Vector2(0.5f, 0.5f));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            TargetGrid = (Grid) target;

            ClearOtherTile = EditorGUILayout.Toggle("导出地图是否清除标注瓦片：", ClearOtherTile);

            EDButton.CreateBtn("生成预制地图范围", 220, 15, (() => { ImportDefaultMap(); }));
            
            EDButton.CreateBtn("导出地图信息", 220, 45, (() => { ExportMap(); }));
        }

        /// <summary>
        /// Callback to draw gizmos only if the object is selected.
        /// </summary>
        void OnDrawGizmosSelected()
        {
            Vector3 pos  = new Vector3(TargetGrid.transform.position.x-((float)TempConfig.MapSizeX/2),TargetGrid.transform.position.y-((float)TempConfig.MapSizeY/2)); 
            Vector3 size = new Vector3(TempConfig.MapSizeX, TempConfig.MapSizeY);
            Rect mapCheck=new Rect(pos,size);
            EDGizmos.DrawRect(mapCheck,Color.green);
        }
        
        //生成默认地图画板
        private void ImportDefaultMap()
        {
            //创建面板
            int xSize = DefaultXSize;
            int ySize = DefaultYSize;
            
            Tile tile = CreateInstance<Tile>();
            tile.sprite = TileSp;
            
            Tilemap tilemap = TargetGrid.GetComponentInChildren<Tilemap>();
            tilemap.ClearAllTiles();
            for (int i = 0; i < xSize; i++)
            {
                tilemap.SetTile(new Vector3Int(i,0,0),tile);
                tilemap.SetTile(new Vector3Int(i,ySize-1,0),tile);
            }
            for (int j = 0; j < ySize; j++)
            {
                tilemap.SetTile(new Vector3Int(0,j,0),tile);
                tilemap.SetTile(new Vector3Int(xSize-1,j,0),tile);
            }

            //创建物体节点
            GameObject goRoot = null;
            if (TargetGrid.transform.Find(MonsterName)==null)
            {
                goRoot=new GameObject(MonsterName);
                goRoot.transform.SetParent(TargetGrid.transform);
            }
            if (TargetGrid.transform.Find(BossName)==null)
            {
                goRoot=new GameObject(BossName);
                goRoot.transform.SetParent(TargetGrid.transform);
            }
            if (TargetGrid.transform.Find(NPCName)==null)
            {
                goRoot=new GameObject(NPCName);
                goRoot.transform.SetParent(TargetGrid.transform);
            }
            
            //碰撞节点
            if (TargetGrid.transform.Find(ColliderName)==null)
            {
                goRoot=new GameObject(ColliderName);
                goRoot.transform.SetParent(TargetGrid.transform);
                goRoot.transform.localPosition=Vector3.zero;
                
                GameObject childCollider = new GameObject("Collider01");
                SpriteRenderer renderer  = childCollider.AddComponent<SpriteRenderer>();
                renderer.sprite = ColliderSp;
                childCollider.AddComponent<BoxCollider2D>();
                childCollider.transform.SetParent(goRoot.transform);
                childCollider.transform.localPosition=Vector3.zero;
            }
        }
        
        //导出地图
        private void ExportMap()
        {
            string assetPath = "Assets/Resources/Map/"+TargetGrid.name+".asset";
            string assetMapPath = "Assets/Resources/Map/"+TargetGrid.name+"_as.prefab";
            AssetDatabase.CreateFolder("Assets/Resources/", "Map");

            string[] results = AssetDatabase.FindAssets(assetPath);
            for (int i = 0; i < results.Length; i++)
            {
                AssetDatabase.DeleteAsset(results[i]);
            }

            GameObject mapAs = PrefabUtility.SaveAsPrefabAsset(TargetGrid.gameObject,assetMapPath);
            
            MapData mapData = GridToMapData(TargetGrid);
            ExportGoInfos(ref mapData);
            mapData.MapAs = mapAs;
            AssetDatabase.CreateAsset(mapData,assetPath);

            HandleMapPrefabGrid(mapAs, mapData);

            EditorUtility.SetDirty(mapAs);
            EditorUtility.SetDirty(mapData);
        }
        
        //处理地图预制体的瓦片Tile
        private void HandleMapPrefabGrid(GameObject mapAs, MapData mapData)
        {
            if (ClearOtherTile==false)
            {
                return;
            }
            Tilemap tilemap = mapAs.GetComponentInChildren<Tilemap>();

            //将道路的瓦片清除
            for (int i = 0; i < mapData.RoadPos.Count; i++)
            {
                tilemap.SetTile(new Vector3Int(mapData.RoadPos[i].x, mapData.RoadPos[i].y,0),null);
            }
        }

        private MapData GridToMapData(Grid grid)
        {
            Tilemap tilemap = grid.GetComponentInChildren<Tilemap>();
            MapData data = CreateInstance<MapData>();
            data.MapPos = new Vector2Int(Mathf.FloorToInt(TargetGrid.transform.position.x), Mathf.FloorToInt(TargetGrid.transform.position.y));;
            data.MapName = grid.name;
            data.Init();

            int xSize = DefaultXSize;
            
            int ySize = DefaultYSize;
            for (int i = 0; i < xSize; i++)
            {
                for (int j = 0; j < ySize; j++)
                {
                    Vector3Int pos = new Vector3Int(i, j, 0);
                    TileBase tmpTile = tilemap.GetTile(pos);
                    
                    if (tmpTile!=null)
                    {
                        Debug.Log("瓦片》》》》》》》》》"+pos.x+"-------"+pos.y);
                        bool isObstacle  = CheckTileMapIsObstacle(tmpTile);
                        bool isRoad      = CheckTileMapIsRoad(tmpTile);

                        //障碍物路径
                        if (isObstacle)
                        {
                            Debug.Log("障碍物路径》》》》》》》》》"+pos.x+"-------"+pos.y);
                            data.ObstaclePos.Add(new Vector2Int(pos.x,pos.y));
                        }
                        //道路路径
                        if (isRoad)
                        {
                            Debug.Log("道路路径》》》》》》》》》"+pos.x+"-------"+pos.y);
                            data.RoadPos.Add(new Vector2Int(pos.x,pos.y));
                        }
                    }
                }
            }
            return data;
        }

        private bool CheckTileMapIsObstacle(TileBase tile)
        {
            string titleFullName = tile.GetType().FullName;
            Debug.Log("检测障碍》》》》》"+titleFullName+"------"+BaseTile+"-------"+RuleTile);
            if (titleFullName==BaseTile)
            {
                return ((Tile) tile).colliderType != Tile.ColliderType.None;
            }
            else if (titleFullName==RuleTile)
            {
                return ((RuleTile) tile).m_DefaultColliderType != Tile.ColliderType.None;
            }
            return false;
        }
        
        private bool CheckTileMapIsRoad(TileBase tile)
        {
            string titleFullName = tile.GetType().FullName;
            Debug.Log("检测道路》》》》》"+titleFullName+"------"+BaseTile+"-------"+RuleTile);
            if (titleFullName==BaseTile)
            {
                return ((Tile) tile).colliderType == Tile.ColliderType.None;
            }
            else if (titleFullName==RuleTile)
            {
                return ((RuleTile) tile).m_DefaultColliderType == Tile.ColliderType.None;
            }
            return false;
        }

        private void ExportGoInfos(ref MapData mapData)
        {
            Transform rootTrans = TargetGrid.transform.Find(MonsterName);
            if (rootTrans!=null)
            {
                mapData.MonsterList=GetGoInfos(rootTrans);
            }
            
            rootTrans = TargetGrid.transform.Find(BossName);
            if (rootTrans!=null)
            {
                mapData.BossList=GetGoInfos(rootTrans);
            }
            
            rootTrans = TargetGrid.transform.Find(NPCName);
            if (rootTrans!=null)
            {
                mapData.NPCList=GetGoInfos(rootTrans);
            }
        }

        private List<GoInfo> GetGoInfos(Transform rootTrans)
        {
            List<GoInfo> list=new List<GoInfo>();
            for (int i = 0; i < rootTrans.childCount; i++)
            {
                GoInfo info = new GoInfo();
                Transform child = rootTrans.GetChild(i);
                string goAssetName = child.name;
                goAssetName.Replace("（", "(").Replace("）",")");
                goAssetName = Regex.Replace(goAssetName.Replace("（", "(").Replace("）", ")"), @"\([^\(]*\)", "");

                info.GoEntityName = goAssetName;
                info.GoIndex = i;
                info.Pos = child.localPosition;
                list.Add(info);
                Debug.Log("资源名："+goAssetName);
            }

            return list;
        }

    }
}