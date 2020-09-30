using System;
using System.Collections.Generic;
using Demo.Config;
using LCECS;
using UnityEngine;

namespace Demo.Help
{
    /*
     * A*算法  F = G + H
     * G = 从起点 A 移动到指定方格的移动代价，沿着到达该方格而生成的路径。
     * H = 从指定的方格移动到终点 B 的估算成本。
     * 博客：https://blog.csdn.net/hitwhylz/article/details/23089415
     */

    public class PathNode
    {
        //总花费
        public int F;
        //移动花费
        public int G;
        //估算花费
        public int H;

        //位置
        public Vector2Int Pos;

        //是否可通过
        public bool IsObstacle;

        //是否是道路
        public bool IsRoad;

        //是否已经查询过
        public bool IsClose = false;

        //父节点
        public PathNode ParentNode;

        public PathNode(int x, int y, bool isObstacle)
        {
            Pos = new Vector2Int { x = x, y = y };
            IsObstacle = isObstacle;
            IsRoad = false;
            IsClose = false;
            F = 0;
            G = 2147483647;
            H = 0;
            ParentNode = null;
        }

        public void ReSetNode()
        {
            IsObstacle = false;
            IsRoad = false;
            IsClose = false;
            F = 0;
            G = 2147483647;
            H = 0;
            ParentNode = null;
        }

        public void SetObstacleNode()
        {
            IsObstacle = true;
        }

        public void SetRoadNode()
        {
            IsRoad = true;
        }
    }

    public static class AstarHelp
    {
        private static readonly object LockObj = new object();
        private static Vector2Int MapSize = new Vector2Int(TempConfig.MapSizeX, TempConfig.MapSizeY);
        private static Vector2Int StartPos;
        private static Vector2Int EndPos;

        private static PathNode[,] Map = new PathNode[TempConfig.MapSizeX, TempConfig.MapSizeY];
        private static bool CheckRoad;

        private static List<PathNode> CloseList = new List<PathNode>();
        private static List<PathNode> OpenList = new List<PathNode>();

        static AstarHelp()
        {
            for (int i = 0; i < TempConfig.MapSizeX; i++)
            {
                for (int j = 0; j < TempConfig.MapSizeY; j++)
                {
                    Map[i, j] = new PathNode(i, j, false);
                }
            }
        }

        /// <summary>
        /// 计算H值：估算花费
        /// </summary>
        /// <returns></returns>
        private static int CalH(Vector2Int pos)
        {
            return 10 * (Mathf.Abs(pos.x - EndPos.x) + Mathf.Abs(pos.y - EndPos.y));
        }

        /// <summary>
        /// 计算G值：移动花费
        /// </summary>
        /// <returns></returns>
        private static int CalG(Vector2Int pos, Vector2Int targetpos)
        {
            int xMove = targetpos.x - pos.x;
            int yMove = targetpos.y - pos.y;
            //对角线
            if (Mathf.Abs(xMove) + Mathf.Abs(yMove) == 2)
            {
                return 14;
            }
            else
            {
                return 10;
            }
        }

        /// <summary>
        /// 计算总花费
        /// </summary>
        /// <returns></returns>
        private static int CalF(Vector2Int pos, Vector2Int targetpos)
        {
            return CalH(pos) + CalG(pos, targetpos);
        }

        /// <summary>
        /// 找到下一个路径点
        /// </summary>
        /// <returns></returns>
        private static bool FindNextPathNode(PathNode parentNode)
        {
            PathNode nextNode;
            //查找相邻的八个相邻节点
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    Vector2Int nextPos = new Vector2Int(parentNode.Pos.x + i, parentNode.Pos.y + j);
                    //1,检测下一步是不是终点
                    if (nextPos.Equals(EndPos))
                        return true;
                    //2,检测下一步是否超出地图边界或者已经加入closeList或者是不可到达状态。
                    if (nextPos.x >= MapSize.x || nextPos.y >= MapSize.y || nextPos.x < 0 || nextPos.y < 0)
                        continue;
                    //Debug.Log("下一步》》》》"+nextPos.ToString());
                    //3,检测下一步是否加入closeList或者是不可到达状态。
                    if (Map[nextPos.x, nextPos.y].IsClose || Map[nextPos.x, nextPos.y].IsObstacle)
                        continue;
                    //4,如果是检测道路
                    if (CheckRoad && Map[nextPos.x, nextPos.y].IsRoad == false)
                        continue;
                    nextNode = Map[nextPos.x, nextPos.y];
                    //需要新加入openlist
                    if (!OpenList.Contains(nextNode))
                    {
                        OpenList.Add(nextNode);
                        nextNode.G = parentNode.G + CalG(parentNode.Pos, nextPos);
                        nextNode.H = CalH(nextPos);
                        nextNode.F = nextNode.G + nextNode.H;
                        nextNode.ParentNode = parentNode;
                    }
                    else
                    {
                        //判断openlist中是否有更快到达下个点的方式
                        if (Mathf.Abs(i) + Mathf.Abs(j) == 2)
                        {
                            //对角线移动的情况
                            if (parentNode.G + 14 < nextNode.G)
                            {
                                nextNode.G = parentNode.G + 14;
                                nextNode.H = CalH(nextPos);
                                nextNode.F = nextNode.G + nextNode.H;
                                nextNode.ParentNode = parentNode;
                            }
                        }
                        else
                        {
                            //水平（竖直）移动的情况
                            if (parentNode.G + 10 < nextNode.G)
                            {
                                nextNode.G = parentNode.G + 10;
                                nextNode.H = CalH(nextPos);
                                nextNode.F = nextNode.G + nextNode.H;
                                nextNode.ParentNode = parentNode;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private static PathNode AstarSearch()
        {
            //1，将起点加入openList
            if (Map[StartPos.x, StartPos.y].IsObstacle)
            {
                ECSLocate.ECSLog.LogR("起始点不能是障碍");
                return null;
            }
            if (EndPos.x > TempConfig.MapSizeX || EndPos.y > TempConfig.MapSizeY)
            {
                ECSLocate.ECSLog.LogR("目标点超过地图范围>>>>>>>>>>>>>>>>", EndPos);
                return null;
            }
            if (Map[EndPos.x, EndPos.y].IsObstacle)
            {
                ECSLocate.ECSLog.LogR("目标点不能是障碍");
                return null;
            }

            PathNode startNode = Map[StartPos.x, StartPos.y];
            startNode.H = CalH(StartPos);
            startNode.G = 0;
            startNode.F = CalF(StartPos, EndPos);
            OpenList.Add(startNode);

            //2.重复以下步骤：a.遍历open list查找F最小的节点，视作当前要处理的点
            PathNode currentNode = startNode;
            int Fmin = 2147483647;
            for (int z = 0; z < 1000; z++)
            {
                if (OpenList.Count == 0)
                {
                    //Debug.Log("没有路径!!!!!!!!!!");
                    //没有找到就直接将最近可以走的设置为路径
                    currentNode= Map[StartPos.x, StartPos.y];
                    return currentNode;
                }

                //1,找到最小的F值节点
                int currentNodeIndex = 0;
                for (int i = 0; i < OpenList.Count; i++)
                {
                    if (OpenList[i].F < Fmin)
                    {
                        currentNodeIndex = i;
                        Fmin = OpenList[i].F;
                    }
                }

                currentNode = OpenList[currentNodeIndex];

                //2.把这个点移到closeList
                currentNode.IsClose = true;
                OpenList.RemoveAt(currentNodeIndex);

                //3,找下一点
                if (FindNextPathNode(currentNode))
                {
                    //currentNode.ParentNode=new PathNode(EndPos.x,EndPos.y,false);
                    return currentNode;
                }
            }
            return currentNode;
        }

        private static List<Vector2Int> GetPathList(PathNode node)
        {
            List<Vector2Int> path = new List<Vector2Int>();
            if (node == null)
            {
                return path;
            }

            PathNode current = node;
            Stack<PathNode> sta = new Stack<PathNode>();
            while (current.ParentNode != null)
            {
                sta.Push(current);
                current = current.ParentNode;
            }


            while (sta.Count > 0)
            {
                PathNode tmpNode = sta.Pop();
                path.Add(new Vector2Int(tmpNode.Pos.x, tmpNode.Pos.y));
            }

            return path;
        }

        public static List<Vector2Int> FindPath(Vector2Int startPos, Vector2Int endPos, List<Vector2Int> obstaclePos, List<Vector2Int> roadPath = null)
        {
            lock (LockObj)
            {
                StartPos = startPos;
                EndPos = endPos;
                CheckRoad = roadPath != null && roadPath.Count > 0;

                //先重置
                for (int i = 0; i < TempConfig.MapSizeX; i++)
                {
                    for (int j = 0; j < TempConfig.MapSizeY; j++)
                    {
                        Map[i, j].ReSetNode();
                    }
                }
                CloseList.Clear();
                OpenList.Clear();

                //刷新地图位置
                for (int i = 0; i < obstaclePos.Count; i++)
                {
                    Vector2Int pos = obstaclePos[i];
                    Map[pos.x, pos.y].SetObstacleNode();
                }
                if (roadPath != null && roadPath.Count > 0)
                {
                    for (int i = 0; i < roadPath.Count; i++)
                    {
                        Vector2Int pos = roadPath[i];
                        Map[pos.x, pos.y].SetRoadNode();
                    }
                }

                PathNode pathNode = AstarSearch();
                List<Vector2Int> path = GetPathList(pathNode);

                CheckRoad = false;

                return path;
            }
        }
    }

    //多线程支持的
    public static class AstarThreadHelp
    {

    }

}