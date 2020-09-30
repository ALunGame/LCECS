using System;
using System.Collections.Generic;
using LCECS.Core.ECS;
using Demo.Com;
using UnityEngine;

namespace Demo.System
{
    //玩家体力，能量系统
    public class PlayerEnergySystem : BaseSystem
    {
        protected override List<Type> RegListenComs()
        {
            return new List<Type>(){typeof(PlayerCom),typeof(PlayerPhysicsCom)};
        }

        protected override void HandleComs(List<BaseCom> comList)
        {
            PlayerCom playerCom = GetCom<PlayerCom>(comList[0]);
            PlayerPhysicsCom physicsCom = GetCom<PlayerPhysicsCom>(comList[1]);
            RecoverEnergy(playerCom,physicsCom);
        }

        //恢复体力
        private void RecoverEnergy(PlayerCom playerCom,PlayerPhysicsCom physicsCom)
        {
            if (physicsCom.Velocity.y>0)
            {
                //上墙
                if (physicsCom.CollideDir == ColliderDir.Left || physicsCom.CollideDir == ColliderDir.Right)
                {
                    playerCom.Energy-=playerCom.AddEnergy*Definition.DeltaTime*2;
                    playerCom.Energy=playerCom.Energy<=0?0:playerCom.Energy;
                }
            }
            
            if (physicsCom.CollideDir == ColliderDir.Down)
            {
                playerCom.Energy+=playerCom.AddEnergy*Definition.DeltaTime;
                playerCom.Energy=playerCom.Energy>=playerCom.MaxEnergy?playerCom.MaxEnergy:playerCom.Energy;
            }
        }
    }
}
