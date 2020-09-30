using System;
using LCECS.Core.Tree.Nodes.Action;
using Demo.Com;
using LCECS;
using LCECS.Core.Tree.Base;
using LCECS.Data;
using UnityEngine;

namespace Demo.BevNode
{
    [Node(ViewName = "玩家移动节点",IsBevNode =true)]
    public class PlayerMoveNode : NodeAction
    {
        
        protected override void OnEnter(NodeData wData)
        {
            EntityWorkData workData = wData as EntityWorkData;
            //参数
            bool doDash = workData.GetReqParam(EntityReqId.PlayerMove).GetBool();
            Vector2 inputMove = workData.GetReqParam(LCECS.EntityReqId.PlayerMove).GetVect2();

            //组件
            PlayerCom playerCom = workData.MEntity.GetCom<PlayerCom>();
            PlayerSpeedCom speedCom = workData.MEntity.GetCom<PlayerSpeedCom>();
            PlayerPhysicsCom physicsCom = workData.MEntity.GetCom<PlayerPhysicsCom>();

            #region 点击加速
            //初始速度
            //speedCom.Speed = speedCom.MinSpeed;
            #endregion
            
            HandleDashInput(ref doDash,physicsCom, speedCom);
            HandleWallInputMove(ref inputMove, physicsCom, speedCom,playerCom);
            HandleNormalInputMove(ref inputMove, physicsCom, speedCom);

            if (doDash)
            {
                ECSLocate.ECSLog.LogR("请求冲刺》》》",speedCom.DoDash,speedCom.IsDash);
            }
            speedCom.DoDash  = doDash;
            speedCom.UpSpeed = speedCom.MaxUpSpeed * inputMove.y;
            speedCom.Speed   = speedCom.MaxSpeed * inputMove.x;

            #region 点击加速
            // if (inputMove.x>0)
            // {
            //     speedCom.Speed  += speedCom.AddSpeed * inputMove.x;
            //     speedCom.Speed   = speedCom.Speed > speedCom.MaxSpeed ? speedCom.MaxSpeed : speedCom.Speed;
            // }
            #endregion

            //水平速度限制
            speedCom.Speed   = speedCom.Speed > speedCom.MaxSpeed ? speedCom.MaxSpeed : speedCom.Speed;

            //请求冲刺
            if (speedCom.DoDash)
            {
                ECSLocate.ECS.SetGlobalSingleComData((EffectCom com) =>
                {
                    com.CurrShowEffectId = 1001;
                    com.EntityId = workData.Id;
                });
            }
        }

        //处理冲刺输入值
        private void HandleDashInput(ref bool doDash,PlayerPhysicsCom physicsCom, PlayerSpeedCom speedCom)
        {
            if (speedCom.IsDash)
            {
                doDash=false;
                return;
            }

            if (speedCom.DoDash)
            {
                doDash=false;
                return;
            }
        }

        //处理墙上移动输入值
        private void HandleWallInputMove(ref Vector2 inputMove, PlayerPhysicsCom physicsCom, PlayerSpeedCom speedCom,PlayerCom playerCom)
        {
            if (physicsCom.CollideDir != ColliderDir.Left && physicsCom.CollideDir != ColliderDir.Right)
                return;

            if (playerCom.Energy<=0)
            {
                ECSLocate.ECSLog.LogR("体力用完了");
                return;
            }

            //上墙
            if (inputMove.y == 0)
            {
                if (physicsCom.CollideDir == ColliderDir.Left)
                {
                    if (inputMove.x >= 0)
                    {
                        //TODO
                        //玩家动画方向
                        inputMove.x = 0;
                    }
                    else
                    {
                        inputMove.y = -inputMove.x;
                        inputMove.x = 0;
                    }
                }
                else if (physicsCom.CollideDir == ColliderDir.Right)
                {
                    if (inputMove.x <= 0)
                    {
                        //TODO
                        //玩家动画方向
                        inputMove.x = 0;
                    }
                    else
                    {
                        inputMove.y = inputMove.x;
                        inputMove.x = 0;
                    }
                }
            }

            //横跳
            if (inputMove.y != 0)
            {
                if (physicsCom.CollideDir == ColliderDir.Left)
                {
                    //右横跳
                    if (inputMove.x > 0)
                    {
                        inputMove.x = 1;
                        inputMove.y = 1;
                    }
                }
                else if (physicsCom.CollideDir == ColliderDir.Right)
                {
                    //左横跳
                    if (inputMove.x < 0)
                    {
                        inputMove.x = -1;
                        inputMove.y = 1;
                    }
                }
            }            
        }

        //处理普通移动输入值
        private void HandleNormalInputMove(ref Vector2 inputMove, PlayerPhysicsCom physicsCom, PlayerSpeedCom speedCom)
        {
            //if (physicsCom.CollideDir != ColliderDir.None && physicsCom.CollideDir != ColliderDir.Down &&
            //    physicsCom.CollideDir != ColliderDir.Up)
            //    return;

            //浮空时受力
            //if (physicsCom.CollideDir == ColliderDir.None)
            //{
            //    inputMove.x *= 0.5f;
            //}

            //重置跳跃索引
            if (physicsCom.CollideDir == ColliderDir.Down)
            {
                speedCom.CurrJumpIndex = 0;
            }

            //垂直输入处理(左右方向爬墙)
            if (physicsCom.CollideDir != ColliderDir.Right&& physicsCom.CollideDir != ColliderDir.Left)
            {
                SetInputJumpValue(speedCom, ref inputMove, inputMove.y);
            }

            //水平输入处理
            //地面左右有阻挡
            if (physicsCom.SubCollideDir== ColliderDir.Left && inputMove.x<0)
            {
                inputMove.x = 0;
            }
            if (physicsCom.SubCollideDir == ColliderDir.Right && inputMove.x > 0)
            {
                inputMove.x = 0;
            }
        }

        private void SetInputJumpValue(PlayerSpeedCom speedCom, ref Vector2 inputMove, float value)
        {
            if (value == 0)
            {
                inputMove.y = 0;
                return;
            }

            if (CheckCanJump(speedCom))
            {
                inputMove.y = value;
                speedCom.CurrJumpIndex++;
            }
            else
            {
                inputMove.y = 0;
            }
        }

        private bool CheckCanJump(PlayerSpeedCom speedCom)
        {
            if (speedCom.CurrJumpIndex < speedCom.MaxJumpIndex)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}