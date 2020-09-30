using System;
using System.Collections.Generic;
using System.Diagnostics;
using Demo.Com;
using Demo.Config;
using Demo.Help;
using LCECS;
using LCECS.Core.ECS;
using LCECS.Help;
using LCTileMap;
using UnityEngine;

namespace Demo.System
{
    /// <summary>
    /// 寻路系统
    /// </summary>
    public class SeekPathSystem:BaseSystem
    {
        private Stopwatch stopwatch = new Stopwatch();

        protected override List<Type> RegListenComs()
        {
            return new List<Type>(){typeof(SeekPathCom),typeof(SpeedCom)};
        }

        protected override void HandleComs(List<BaseCom> comList)
        {
            stopwatch.Restart();

            SeekPathCom seekPathCom = GetCom<SeekPathCom>(comList[0]);
            SpeedCom speedCom       = GetCom<SpeedCom>(comList[1]);

            HandleSeek(seekPathCom,speedCom);

            stopwatch.Stop();
            //Debug.Log("寻路系统一次轮询花费>>>>>>" + stopwatch.Elapsed.TotalMilliseconds);
        }
        
        //处理寻路
        private void HandleSeek(SeekPathCom seekPathCom,SpeedCom speedCom)
        {
            //请求寻路
            if (seekPathCom.ReqSeek)
            {
                Vector2Int curPos = seekPathCom.CurrPos;
                Vector2Int tarPos = seekPathCom.TargetPos;
                MapData mapData = TempConfig.GetMapData(seekPathCom.MapPos);

                //List<Vector2Int> path = AstarHelp.FindPath(curPos, tarPos, mapData.ObstaclePos);
                //seekPathCom.MovePath = path;

                TaskHelp.AddTaskTwoParam(seekPathCom, mapData, ExcuteFindPath, FinishFindPath);
                seekPathCom.ReqSeek=false;
            }
            
            if (seekPathCom.CurrPos.Equals(seekPathCom.TargetPos))
                return;
            if (seekPathCom.MovePath==null|| seekPathCom.MovePath.Count<=0)
                return;

            MoveFollowPath(seekPathCom.Trans, seekPathCom.MovePath, ref seekPathCom.CurrPathIndex, speedCom.MaxSpeed, seekPathCom);
            //HandleSeekMove(seekPathCom, speedCom, viewCom);
        }

        //路径移动
        public void MoveFollowPath(Transform obj, List<Vector2Int> path, ref int index, float speed, SeekPathCom seekPathCom)
        {
            if (index > path.Count - 1)
                return;
            obj.localPosition = Vector2.MoveTowards(obj.localPosition, path[index], speed * Time.deltaTime);
            float distance = Vector2.Distance(path[index], obj.localPosition);
            if (distance < 0.001f)
            {
                seekPathCom.CurrPos = path[index];
                index++;
                if (index == path.Count)
                {
                    obj.localPosition   = new Vector3(path[index - 1].x, path[index - 1].y,0);
                    seekPathCom.CurrPos = path[index-1];
                }
            }
        }

        //处理寻路移动
        private void HandleSeekMove(SeekPathCom seekPathCom,SpeedCom speedCom)
        {
            if (seekPathCom.MovePath==null||seekPathCom.MovePath.Count<1)
                return;

            Vector2Int lastPoint;
            Vector2Int targrtPoint;

            //最后一个点
            if (seekPathCom.MovePath.Count==1)
            {
                lastPoint = seekPathCom.CurrPos;
                targrtPoint = seekPathCom.MovePath[0];
            }
            else
            {
                lastPoint = seekPathCom.MovePath[0];
                targrtPoint = seekPathCom.MovePath[1];
            }


            //走到目标点---->换下一个点
            if (Vector2.Distance(new Vector2(seekPathCom.Trans.localPosition.x, seekPathCom.Trans.localPosition.y), targrtPoint)<=0.01f)
            {
                //更新当前所处地图位置
                seekPathCom.CurrPos = targrtPoint;
                //更新目标点
                seekPathCom.MovePath.RemoveAt(0);

                //到达目标点
                if (seekPathCom.MovePath.Count<1)
                {
                    seekPathCom.MovePath = null;
                    //ECSLocate.ECSLog.LogError("寻路结束》》》》目标点 ： ", viewCom.Trans.localPosition);
                    //更新位置
                    seekPathCom.Trans.localPosition = new Vector3(targrtPoint.x, targrtPoint.y, 0);
                    return;
                }

                //最后一个点
                if (seekPathCom.MovePath.Count == 1)
                {
                    lastPoint = seekPathCom.CurrPos;
                    targrtPoint = seekPathCom.MovePath[0];
                }
                else
                {
                    lastPoint = seekPathCom.MovePath[0];
                    targrtPoint = seekPathCom.MovePath[1];
                }
            }
            
            //移动
            float deltaDistance = speedCom.MaxSpeed * Definition.DeltaTime;
            Vector2 movePos     = CalMovePosition(lastPoint, targrtPoint, deltaDistance);
            seekPathCom.Trans.Translate(movePos);
        }

        //计算移动位移
        private Vector2 CalMovePosition(Vector2Int lastPoint,Vector2Int targrtPoint,float distance)
        {
            Vector2 dir=Vector2.zero;
            
            //水平方向
            if (targrtPoint.x>lastPoint.x)
            {
                dir.x = 1;
            }
            else if (targrtPoint.x==lastPoint.x)
            {
                dir.x = 0;
            }
            else
            {
                dir.x = -1;
            }
            
            //垂直方向
            if (targrtPoint.y>lastPoint.y)
            {
                dir.y = 1;
            }
            else if (targrtPoint.y==lastPoint.y)
            {
                dir.y = 0;
            }
            else
            {
                dir.y = -1;
            }
            dir = new Vector2((float)dir.x, (float)dir.y);
            return dir * distance;
        }

        //执行寻路
        private (SeekPathCom SeekPathCom, List<Vector2Int> Path) ExcuteFindPath(SeekPathCom seekPathCom, MapData mapData)
        {
            List<Vector2Int> path = null;
            if (seekPathCom.CanFly)
            {
                path = AstarHelp.FindPath(seekPathCom.CurrPos, seekPathCom.TargetPos, mapData.ObstaclePos);
            }
            else
            {
                path = AstarHelp.FindPath(seekPathCom.CurrPos, seekPathCom.TargetPos, mapData.ObstaclePos, mapData.RoadPos);
            }
            return (seekPathCom, path);
        }

        //完成寻路
        private void FinishFindPath((SeekPathCom SeekPathCom, List<Vector2Int> Path) item)
        {
            //重复路径删除
            int newPathIndex = 0;
            Vector2Int currPos = item.SeekPathCom.CurrPos;
            if (currPos == null)
                newPathIndex = 0;
            else
            {
                if (item.Path == null || item.Path.Count <= 0)
                    newPathIndex = 0;
                else
                {
                    for (int i = 0; i < item.Path.Count; i++)
                    {
                        if (item.Path[i].Equals(currPos))
                        {
                            newPathIndex = i;
                            ECSLocate.ECSLog.LogError("重复路径删除>>>>>>>", newPathIndex, item.Path[i]);
                            break;
                        }
                    }
                }
            }

            //ECSLocate.ECSLog.LogR("找到路径了>>>>>>>", newPathIndex, item.Path.Count);
            item.SeekPathCom.MovePath = item.Path;
            item.SeekPathCom.MovePath.Insert(item.SeekPathCom.MovePath.Count, item.SeekPathCom.TargetPos);
            item.SeekPathCom.CurrPathIndex = newPathIndex;
        }
    }
}