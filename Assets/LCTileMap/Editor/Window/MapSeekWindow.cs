using System;
using UnityEditor;
using UnityEngine;
using LCECS.Help;
using LCHelp;
using Demo.Config;
using System.Collections.Generic;
using Demo.Help;

namespace LCTileMap.Window
{
    /// <summary>
    /// 地图寻路预览
    /// </summary>
    public class MapSeekWindow : EditorWindow
    {
        private MapData SelMap;
        private Vector2Int CurPos;
        private Vector2Int TargetPos;

        private List<Vector2Int> Path=new List<Vector2Int>();

        private static int BoxSize=30;
        private static Texture2D NullImg;

        private static Texture2D RoadImg;

        private static Texture2D WallImg;

        private static Texture2D PathImg;


        private static Texture2D CurImg;


        private static Texture2D TargetImg;


        [MenuItem("LCECS/地图寻路")]
        private static void OpenWindow()
        {

            InitBoxImg();


            MapSeekWindow window = GetWindow<MapSeekWindow>();
            window.titleContent = new GUIContent("地图寻路");
        }

        private static void InitBoxImg()
        {
            NullImg = new Texture2D(BoxSize, BoxSize);
            for (int w = 0; w < BoxSize; w++){
                for (int h = 0; h < BoxSize; h ++){
                    NullImg.SetPixel(w,h,Color.white);
                }
            }
            NullImg.Apply();

            RoadImg = new Texture2D(BoxSize, BoxSize);
            for (int w = 0; w < BoxSize; w++){
                for (int h = 0; h < BoxSize; h ++){
                    RoadImg.SetPixel(w,h,Color.gray);
                }
            }
            RoadImg.Apply();

            WallImg = new Texture2D(BoxSize, BoxSize);
            for (int w = 0; w < BoxSize; w++){
                for (int h = 0; h < BoxSize; h ++){
                    WallImg.SetPixel(w,h,Color.black);
                }
            }
            WallImg.Apply();

            PathImg = new Texture2D(BoxSize, BoxSize);
            for (int w = 0; w < BoxSize; w++){
                for (int h = 0; h < BoxSize; h ++){
                    PathImg.SetPixel(w,h,Color.green);
                }
            }
            PathImg.Apply();

            CurImg = new Texture2D(BoxSize, BoxSize);
            for (int w = 0; w < BoxSize; w++){
                for (int h = 0; h < BoxSize; h ++){
                    CurImg.SetPixel(w,h,Color.red);
                }
            }
            CurImg.Apply();

            TargetImg = new Texture2D(BoxSize, BoxSize);
            for (int w = 0; w < BoxSize; w++){
                for (int h = 0; h < BoxSize; h ++){
                    TargetImg.SetPixel(w,h,Color.yellow);
                }
            }
            TargetImg.Apply();
        }

        private void OnGUI() 
        {
            EDLayout.CreateVertical("box",position.width, position.height,()=>{
                SelMap=(MapData)EditorGUILayout.ObjectField(SelMap,typeof(MapData),true);
                CurPos=(Vector2Int)EditorGUILayout.Vector2IntField("当前位置",CurPos);
                TargetPos=(Vector2Int)EditorGUILayout.Vector2IntField("目标位置",TargetPos);

                EDButton.CreateBtn("寻路",100,50,()=>{
                    //Debug.LogWarningFormat("寻路({0},{1} ==> ({2},{3}))",CurPos.x,CurPos.y,TargetPos.x,TargetPos.y);
                    Path = AstarHelp.FindPath(CurPos, TargetPos, SelMap.ObstaclePos);
                    //Debug.LogWarningFormat("找到 {0} 路径点",Path.Count);
                });


                EDButton.CreateBtn("道路寻路",100,50,()=>{
                    //Debug.LogWarningFormat("道路寻路({0},{1} ==> ({2},{3}))",CurPos.x,CurPos.y,TargetPos.x,TargetPos.y);
                    Path = AstarHelp.FindPath(CurPos, TargetPos, SelMap.ObstaclePos,SelMap.RoadPos);
                    //Debug.LogWarningFormat("找到 {0} 路径点",Path.Count);
                });

                EditorGUILayout.Space();

                if (SelMap!=null)
                {
                    for (int y = TempConfig.MapSizeY-1; y >=0; y--)
                    {
                        EDLayout.CreateHorizontal("box",BoxSize*TempConfig.MapSizeY, BoxSize,()=>{

                        for (int x = 0; x < TempConfig.MapSizeX; x++)
                        {
                            // Color showColor = GetShowTextColor(x,y);
                            // GUI.color=showColor;
                            // string pos=string.Format("{0},{1}",x,y);
                            Texture showImg=GetBoxShowImg(x,y);
                            GUILayout.Box(new GUIContent(showImg),GUILayout.Height(BoxSize),GUILayout.Width(BoxSize));
                        }
                    });
                    }
                } 
            });
        }


        private Texture GetBoxShowImg(int x,int y)
        {
            Vector2Int checkPos=new Vector2Int(x,y);
            foreach (var item in SelMap.ObstaclePos)
            {
                if (item.x==checkPos.x&&item.y==checkPos.y)
                {
                    return WallImg;
                }
            }
            

            if (CurPos.x==checkPos.x&&CurPos.y==checkPos.y)
            {
                return CurImg;
            }

            if (TargetPos.x==checkPos.x&&TargetPos.y==checkPos.y)
            {
                return TargetImg;
            }

            foreach (var item in Path)
            {
                if (item.x==checkPos.x&&item.y==checkPos.y)
                {
                    return PathImg;
                }
            }

            foreach (var item in SelMap.RoadPos)
            {
                if (item.x==checkPos.x&&item.y==checkPos.y)
                {
                    return RoadImg;
                }
            }


            return NullImg;
        }
    
        private Color GetShowTextColor(int x,int y)
        {
            Vector2Int checkPos=new Vector2Int(x,y);
            foreach (var item in SelMap.ObstaclePos)
            {
                if (item.x==checkPos.x&&item.y==checkPos.y)
                {
                    return Color.black;
                }
            }
            
            foreach (var item in SelMap.RoadPos)
            {
                if (item.x==checkPos.x&&item.y==checkPos.y)
                {
                    return Color.yellow;
                }
            }


            return Color.white;
        }
    }
}