using LCECS.Core.ECS;
using System.Collections.Generic;
using UnityEngine;

namespace Demo.Com
{
    public enum EnemyState
    {
        None,                       //啥都不做
        SeekPlayer,                 //找玩家
        Wander,                     //游荡
        Attack,                     //攻击
    }

    [Com(GroupName = "Enemy",ViewName ="敌人基础数据")]
    public class EnemyCom:BaseCom
    {
        [ComValue(ShowView = true)]
        public EnemyState State = EnemyState.None;

        //警戒范围，单位地图格
        [ComValue(ShowView =true,ViewEditor =true)]
        public Vector2Int GuardArea = Vector2Int.zero;

        //攻击范围，单位地图格
        [ComValue(ShowView = true, ViewEditor = true)]
        public Vector2Int AttackArea = Vector2Int.one;

        //游荡索引
        [ComValue]
        public int WanderIndex = 0;
        //游荡路径
        public List<Vector2Int> WanderPath = new List<Vector2Int>();
        //出生位置
        public Vector2Int SpawnPos = Vector2Int.zero;

        protected override void OnInit(GameObject go)
        {
            SpawnPos = new Vector2Int((int)go.transform.localPosition.x, (int)go.transform.localPosition.y);

            //寻路路径
            Transform wanderTran = go.transform.Find("WanderPath");
            if (wanderTran!=null)
            {
                for (int i = 0; i < wanderTran.childCount; i++)
                {
                    Vector2Int path = new Vector2Int((int)wanderTran.GetChild(i).localPosition.x, (int)wanderTran.GetChild(i).localPosition.y);
                    path += SpawnPos;
                    WanderPath.Add(path);
                }
            }

        }
    }
}
