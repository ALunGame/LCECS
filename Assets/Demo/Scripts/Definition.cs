using UnityEngine;

namespace Demo
{
    /// <summary>
    /// 游戏定义
    /// </summary>
    public class Definition
    {
        //重力
        public static Vector2 Gravity       = new Vector2(0,-10f);

        //时间间隔
        public static float DeltaTime       = Time.deltaTime;

        //物理时间间隔
        public static float FixedDeltaTime  = Time.fixedDeltaTime;
    }

    /// <summary>
    /// 游戏物体标记
    /// </summary>
    public class GoTag
    {
        public const string Player = "Player";
    }

    /// <summary>
    /// 游戏物体层级
    /// </summary>
    public class GoLayer
    {
        public const string Player = "Player";
        public const string NPC = "NPC";
        public const string Monster = "Monster";

        public const string Ground = "Ground";
        public const string Wall = "Wall";
    }
}
