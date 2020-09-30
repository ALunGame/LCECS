using System.Collections.Generic;
using LCECS.Core.ECS;
using UnityEngine;

namespace Demo.Com
{
    /// <summary>
    /// 寻路组件
    /// </summary>
    [Com(ViewName = "寻路组件")]
    public class SeekPathCom:BaseCom
    {
        public Transform Trans;

        //地图位置
        [ComValue]
        public Vector2Int MapPos;

        //请求移动
        [ComValue]
        public bool ReqSeek = false;
        
        //可以飞的敌人
        [ComValue(ViewEditor =true)]
        public bool CanFly = false;

        //移动路径
        public List<Vector2Int> MovePath=new List<Vector2Int>();
        //当前寻路索引
        [ComValue]
        public int CurrPathIndex = 0;

        //当前点
        [ComValue]
        public Vector2Int CurrPos=Vector2Int.zero;
        //目标点
        [ComValue]
        public Vector2Int TargetPos=Vector2Int.zero;
        
        protected override void OnInit(GameObject go)
        {
            Trans   = go.transform;
            CurrPos = new Vector2Int((int)go.transform.localPosition.x, (int)go.transform.localPosition.y);
        }
    }
}