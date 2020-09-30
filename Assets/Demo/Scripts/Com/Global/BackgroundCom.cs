using System;
using System.Collections.Generic;
using LCECS.Core.ECS;
using UnityEngine;

namespace Demo.Com
{
    /// <summary>
    /// 地图组件
    /// </summary>
    [Com(ViewName = "背景组件",IsGlobal =true)]
    public class BackgroundCom:BaseCom
    {
        public List<Transform> BGBoxs=new List<Transform>();

        public Transform CameraTran;
        public Vector3 LastCameraPosition=Vector3.zero;
        public Vector3 Velocity=Vector3.zero;
        public float SmoothTime;

        protected override void OnInit(GameObject go)
        {
            for (int i = 0; i < 3; i++)
            {
                Transform tmpTran=go.transform.Find("Layer0"+i);
                if (tmpTran!=null)
                {
                    BGBoxs.Add(tmpTran);
                }
            }

            //CameraTran=(Transform)data[0];

            //LastCameraPosition = CameraTran.position;
        }
    }
}