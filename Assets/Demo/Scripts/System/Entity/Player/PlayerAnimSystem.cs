﻿using System;
using System.Collections.Generic;
using LCECS.Core.ECS;
using Demo.Com;
using UnityEngine;

namespace Demo.System
{
    //玩家动画系统
    public class PlayerAnimSystem : BaseSystem
    {
        protected override List<Type> RegListenComs()
        {
            return new List<Type>() {typeof(PlayerAnimCom),typeof(PlayerSpeedCom),typeof(PlayerCom),typeof(PlayerPhysicsCom)};
        }

        protected override void HandleComs(List<BaseCom> comList)
        {
            PlayerAnimCom animCom = GetCom<PlayerAnimCom>(comList[0]);
            PlayerSpeedCom speedCom = GetCom<PlayerSpeedCom>(comList[1]);
            PlayerCom playerCom = GetCom<PlayerCom>(comList[2]);
            PlayerPhysicsCom physicsCom = GetCom<PlayerPhysicsCom>(comList[3]);
            
            animCom.Animtor.SetFloat("Speed",    physicsCom.Velocity.x);
            animCom.Animtor.SetFloat("UpSpeed",  physicsCom.Velocity.y);
            animCom.Animtor.SetBool ("IsGround", physicsCom.CollideDir == ColliderDir.Down);
            if (speedCom.Speed > 0)
            {
                playerCom.SpriteRender.flipX = false;
            }
            else if (speedCom.Speed < 0)
            {
                playerCom.SpriteRender.flipX = true;
            }
            
            animCom.WaveAnimTor.SetFloat("UpSpeed",  physicsCom.Velocity.y);
            animCom.WaveAnimTor.SetBool ("IsGround", physicsCom.CollideDir == ColliderDir.Down);
            animCom.WaveAnimTor.SetBool ("IsRight", playerCom.SpriteRender.flipX == false);
        }
    }
}
