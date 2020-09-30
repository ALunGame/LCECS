using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 公共方法和配置
/// </summary>
namespace LCECS.Core.Tree
{
    /// <summary>
    /// 节点类型
    /// </summary>
    public enum NodeType
    {
        None,
        //根节点
        Root,
        //控制节点
        Control,
        //行为节点
        Action,
        Max,
    }

    /// <summary>
    /// 节点状态
    /// </summary>
    public class NodeState
    {
        //-------------------------------------------------------
        //小于0的都是错误状态
        //-------------------------------------------------------

        //默认状态
        public const int EXECUTING = 0;
        public const int FINISHED = 1;
        public const int TRANSITION = 2;

        //扩展记录状态  100-999
        public const int USER_EXECUTING = 100;

        //>=1000, 扩展记录状态完成
        public const int USER_FINISHED = 1000;

        //可执行
        static private bool IsOK(int runningStatus)
        {
            return runningStatus == FINISHED || runningStatus >= USER_FINISHED;
        }

        //错误
        static public bool IsError(int runningStatus)
        {
            return runningStatus < 0;
        }

        //完成
        static public bool IsFinished(int runningStatus)
        {
            return IsOK(runningStatus) || IsError(runningStatus);
        }

        //执行中
        static public bool IsExecuting(int runningStatus)
        {
            return !IsFinished(runningStatus);
        }
    }

    /// <summary>
    /// 并行种类
    /// </summary>
    public enum NodeParallelType
    {
        AND,
        OR,
    }
    
    /// <summary>
    /// 前提关系
    /// </summary>
    public enum PremiseType
    {
        AND,
        OR,
        XOR,
    }

    /// <summary>
    /// 节点时间戳       （通过更新这个让所有的Node刷新时间）
    /// </summary>
    public class NodeTime
    {
        /// <summary>
        /// 总时长
        /// </summary>
        public static float TotalTime
        {
            private set;
            get;
        }

        /// <summary>
        /// 每帧时间戳
        /// </summary>
        public static float DeltaTime
        {
            private set;
            get;
        }

        /// <summary>
        /// 更新时间
        /// </summary>
        /// <param name="deltaTime">每帧时间</param>
        /// <param name="timeScale">时间比例</param>
        public static void UpdateTime(float deltaTime, float timeScale)
        {
            DeltaTime = deltaTime * timeScale;
            TotalTime += DeltaTime;
        }
    }
}
