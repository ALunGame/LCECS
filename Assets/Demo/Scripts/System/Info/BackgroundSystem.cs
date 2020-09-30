using System;
using System.Collections.Generic;
using Demo.Com;
using Demo.Info;
using LCECS;
using LCECS.Core.ECS;
using UnityEngine;

namespace Demo.System
{
    public class BackgroundSystem: BaseSystem
    {
        protected override List<Type> RegListenComs()
        {
            return new List<Type>(){typeof(BackgroundCom)};
        }

        protected override void HandleComs(List<BaseCom> comList)
        {
            BackgroundCom com = GetCom<BackgroundCom>(comList[0]);

            //背景滚动
            for (int i = 0; i < com.BGBoxs.Count; i++)
            {
                MoveBg(com,com.BGBoxs[i],0.1f+i*0.1f);        
            } 

            //背景复用
            PlayerInfoData playerInfo = ECSLayerLocate.Info.GetWorldInfo<PlayerInfoData>(WorldInfoKey.PlayerInfo);
            Vector3 playerPos = playerInfo.Pos;

            Vector3 pos  = new Vector3(playerPos.x-45,-10); 
            Vector3 size = new Vector3(90,20);
            Rect bgCheck = new Rect(pos,size);
            for (int i = 0; i < com.BGBoxs.Count; i++)
            {
                ReplaceBg(com,com.BGBoxs[i],bgCheck,playerPos.x);        
            } 

            com.LastCameraPosition = com.CameraTran.position;
        }

        private void MoveBg(BackgroundCom com,Transform layerTrans,float scale)
        {
            for (int i = 1; i <= 3; i++)
            {
                Transform trans = layerTrans.Find("0"+i);

                Vector3 parallax = (com.LastCameraPosition - com.CameraTran.position) * scale;
                parallax.z = 0f;
                parallax.y = 0f;
                Vector3 target = trans.position + parallax;

                trans.position = Vector3.SmoothDamp(trans.position, target, ref com.Velocity, com.SmoothTime);
            }
        }

        private void ReplaceBg(BackgroundCom com,Transform layerTrans,Rect bgCheck,float playerX)
        {
            for (int i = 1; i <= 3; i++)
            {
                Transform trans = layerTrans.Find("0"+i);
                Vector2 checkPos=new Vector3(trans.localPosition.x,0);
                if (bgCheck.Contains(checkPos)==false)
                {   
                    //ECSLocate.ECSLog.Log("替换>>>>>",trans.name,trans.position,playerPos);
                    //左边
                    if (playerX>trans.position.x)
                    {
                        trans.gameObject.SetActive(false);
                        trans.position=new Vector3(trans.position.x+90,0);
                        trans.gameObject.SetActive(true); 
                    }
                    else
                    {
                        trans.gameObject.SetActive(false);
                        trans.position=new Vector3(trans.position.x-90,0);
                        trans.gameObject.SetActive(true);
                    }
                }
            }
        }
    }
}