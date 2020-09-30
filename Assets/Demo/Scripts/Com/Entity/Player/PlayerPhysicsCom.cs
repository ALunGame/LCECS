﻿using LCECS.Core.ECS;
using UnityEngine;

namespace Demo.Com
{
    public enum ColliderDir:short
    {
        None,
        Up,
        Down,
        Left,
        Right,
    }
        /// <summary>
    /// 玩家物理组件
    /// </summary>
    [Com(ViewName = "玩家物理组件",GroupName="Player")]
    public class PlayerPhysicsCom: BaseCom
    {
        public Rigidbody2D Rig2D;
        public int GroundLayer=LayerMask.GetMask("Ground");

        //质量
        [ComValue(ViewEditor = true)]
        public float Mass = 1f;

        //默认质量
        [ComValue(ViewEditor = true)]
        public float DefaultMass = 1f;

        //碰撞方向
        [ComValue]
        public ColliderDir CollideDir = ColliderDir.None;

        //辅助碰撞方向（当碰撞较多时）
        [ComValue]
        public ColliderDir SubCollideDir = ColliderDir.None;

        //速度
        [ComValue]
        public Vector2 Velocity;

        public float CollisionRadius = 0.1f;
        public Vector2 BottomOffset=new Vector2(0,-0.2f);
        public Vector2 RightOffset=new Vector2(0.2f,0);
        public Vector2 LeftOffset=new Vector2(-0.2f,0);

        protected override void OnInit(GameObject go)
        {
            Rig2D=go.GetComponent<Rigidbody2D>();
        }
    }
}
