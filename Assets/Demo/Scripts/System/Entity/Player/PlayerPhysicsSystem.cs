using Demo.Com;
using LCECS.Core.ECS;
using System;
using System.Collections.Generic;
using LCECS.Help;
using UnityEngine;
using DG.Tweening;

namespace Demo.System
{
    /// <summary>
    /// 玩家物理系统
    /// </summary>
    public class PlayerPhysicsSystem : BaseSystem
    {
        protected override List<Type> RegListenComs()
        {
            return new List<Type>() { typeof(PlayerPhysicsCom),typeof(PlayerSpeedCom),typeof(PlayerCom)};
        }

        protected override void HandleComs(List<BaseCom> comList)
        {
            HandleCollider(comList);
            HandleMove(comList);
            HandleMass(comList);
            HandleGravityVelocity(comList);
        }

        //处理碰撞方向
        private void HandleCollider(List<BaseCom> comList)
        {
            PlayerPhysicsCom physicsCom = GetCom<PlayerPhysicsCom>(comList[0]);

            //地面射线
            bool isGround = Physics2D.OverlapCircle((Vector2)physicsCom.Rig2D.position + physicsCom.BottomOffset, physicsCom.CollisionRadius,physicsCom.GroundLayer);

            //右射线
            bool isRightWall = Physics2D.OverlapCircle((Vector2)physicsCom.Rig2D.position + physicsCom.RightOffset, physicsCom.CollisionRadius,physicsCom.GroundLayer);

            //左射线
            bool isLeftWall = Physics2D.OverlapCircle((Vector2)physicsCom.Rig2D.position + physicsCom.LeftOffset, physicsCom.CollisionRadius,physicsCom.GroundLayer);

            //赋值
            if (isGround)
            {
                physicsCom.CollideDir        = ColliderDir.Down;
                //赋值子碰撞
                if (isRightWall)
                    physicsCom.SubCollideDir = ColliderDir.Right;
                else if (isLeftWall)
                    physicsCom.SubCollideDir = ColliderDir.Left;
                else
                    physicsCom.SubCollideDir = ColliderDir.None;
            }
            else
            {
                if (isRightWall)
                {
                    physicsCom.SubCollideDir = ColliderDir.Right;
                    physicsCom.CollideDir    = ColliderDir.Right;
                }
                else if (isLeftWall)
                {
                    physicsCom.SubCollideDir = ColliderDir.Left;
                    physicsCom.CollideDir    = ColliderDir.Left;
                }
                else
                {
                    physicsCom.SubCollideDir = ColliderDir.None;
                    physicsCom.CollideDir    = ColliderDir.None;
                }
            }
        }

        //处理移动
        private void HandleMove(List<BaseCom> comList)
        {
            PlayerPhysicsCom physicsCom = GetCom<PlayerPhysicsCom>(comList[0]);
            PlayerSpeedCom speedCom = GetCom<PlayerSpeedCom>(comList[1]);
            PlayerCom playerCom = GetCom<PlayerCom>(comList[2]);

            if (speedCom.DoDash)
            {
                Dash(physicsCom,speedCom, playerCom);
                speedCom.DoDash=false;
                return;
            }

            if (speedCom.IsDash)
            {
                return;
            }

            //速度赋值
            physicsCom.Velocity.x = speedCom.Speed;
            Movement(physicsCom);

            if (speedCom.UpSpeed!=0)
            {
                physicsCom.Velocity.y=speedCom.UpSpeed;
                Jump(physicsCom);
            }
        }

        //处理质量
        private void HandleMass(List<BaseCom> comList)
        {
            PlayerPhysicsCom physicsCom = GetCom<PlayerPhysicsCom>(comList[0]);
            PlayerSpeedCom speedCom = GetCom<PlayerSpeedCom>(comList[1]);
            PlayerCom playerCom = GetCom<PlayerCom>(comList[2]);

            //冲刺时无重力
            if (speedCom.IsDash)
            {
                physicsCom.Mass=0;
                return;
            }

            //没有体力了（直接默认质量）
            if (playerCom.Energy<=0)
            {
                physicsCom.Mass=physicsCom.DefaultMass;
                return;
            }

            //墙上质量降低（摩擦）
            if (physicsCom.CollideDir==ColliderDir.Left||physicsCom.CollideDir==ColliderDir.Right)
            {
                physicsCom.Mass=physicsCom.DefaultMass*0.6f;
            }

            physicsCom.Mass=physicsCom.DefaultMass;
        }

        //处理重力速度
        private void HandleGravityVelocity(List<BaseCom> comList)
        {
            PlayerPhysicsCom phyCom = GetCom<PlayerPhysicsCom>(comList[0]);
            PlayerSpeedCom speedCom = GetCom<PlayerSpeedCom>(comList[1]);

            //phyCom.Rig2D.gravityScale = phyCom.Mass;

            // if (phyCom.CollideDir==ColliderDir.Down&&speedCom.UpSpeed==0)
            // {
            //     phyCom.Rig2D.velocity=new Vector2(phyCom.Rig2D.velocity.x,0);
            //     return;
            // }
            //降落
            if (phyCom.Rig2D.velocity.y<0)
            {
                phyCom.Rig2D.velocity += Definition.Gravity * speedCom.FallMultiplier * phyCom.Mass * Time.deltaTime;
            }
            //跳跃
            else if (phyCom.Rig2D.velocity.y>0)
            {
               if (speedCom.UpSpeed==0)
               {
                   phyCom.Rig2D.velocity += Definition.Gravity * speedCom.LowJumpMultiplier * phyCom.Mass * Time.deltaTime;
               } 
            }
        }

        //执行移动
        private void Movement(PlayerPhysicsCom phyCom)
        {
            phyCom.Rig2D.velocity = new Vector2(phyCom.Velocity.x, phyCom.Rig2D.velocity.y);
        }

        //执行跳跃
        private void Jump(PlayerPhysicsCom phyCom)
        {
            phyCom.Rig2D.velocity  = new Vector2(phyCom.Rig2D.velocity.x, phyCom.Velocity.y);
        }
        
        //执行冲刺
        private void Dash(PlayerPhysicsCom phyCom,PlayerSpeedCom speedCom, PlayerCom playerCom)
        {
            speedCom.IsDash=true;

            //速度归零
            phyCom.Rig2D.velocity = Vector2.zero;

            //计算冲刺速度
            Vector2 dir = Vector2.zero;
            dir.x       = playerCom.SpriteRender.flipX ? -1:1;
            dir.y       = speedCom.UpSpeed == 0?0:1;
            phyCom.Rig2D.velocity += dir *  speedCom.MaxDashSpeed;

            //拖拽还原状态
            DOVirtual.Float(8, 0, speedCom.DashDragTime, (x)=>{
                phyCom.Rig2D.drag = x;
                if (x==0)
                {
                    speedCom.IsDash = false;
                    phyCom.Mass = phyCom.DefaultMass;
                }
            });
        }
    }

}
