using System;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

namespace LCECS.Help
{
    /// <summary>
    /// 时间片段
    /// </summary>
    public class TimeTrack
    {
        public float StartTime;
        public float EndTime;
        public bool IsStart;
        public bool IsEnd;

        private Action onStart;
        private Action onUpdate;
        private Action onCompleted;

        public TimeTrack(float startTime, float endTime)
        {
            StartTime = startTime;
            EndTime   = endTime;
            IsStart   = false;
            IsEnd     = false;
        }

        public TimeTrack OnStart(Action onStart)
        {
            this.onStart = onStart;
            return this;
        }
        public void CallStart()
        {
            onStart?.Invoke();
        }

        public TimeTrack OnUpdate(Action onUpdate)
        {
            this.onUpdate = onUpdate;
            return this;
        }
        public void CallUpdate()
        {
            onUpdate?.Invoke();
        }

        public TimeTrack OnCompleted(Action onCompleted)
        {
            this.onCompleted = onCompleted;
            return this;
        }
        public void CallCompleted()
        {
            onCompleted?.Invoke();
        }
    }

    /// <summary>
    /// 时间线
    /// </summary>
    public class TimeLine
    {
        public bool RunFinish = false;

        private bool SetStartTime = false;
        private bool IsRunning = false;
        private List<TimeTrack> trackList = new List<TimeTrack>();

        /// <summary>
        /// 添加一个时间片
        /// </summary>
        /// <param name="delayTime">在时间线后几秒调用</param>
        /// <param name="continueTime">持续时间</param>
        /// <returns></returns>
        public TimeTrack AddTrack(float delayTime,float continueTime)
        {
            TimeTrack task = new TimeTrack(delayTime, delayTime + continueTime);
            trackList.Add(task);
            return task;
        }

        //开始
        public void Start()
        {
            SetStartTime = false;
            for (int i = 0; i < trackList.Count; i++)
            {
                if (trackList[i].IsEnd == false)
                {
                    trackList[i].StartTime += TimeLineMgr.CurrTime;
                    trackList[i].EndTime += TimeLineMgr.CurrTime;
                }
            }
            IsRunning = true;
            RunFinish = false;
        }

        //暂停
        public void Stop()
        {
            IsRunning = false;
        }

        //重置
        public void ReSet()
        {
            IsRunning = false;
            RunFinish = true;
            trackList.Clear();
        }

        public void Update()
        {
            //运行结束
            if (RunFinish)
                return;

            //不在运行状态
            if (IsRunning==false)
                return;

            RunFinish = true;
            float curTime = TimeLineMgr.CurrTime;
            for (int i = 0; i < trackList.Count; i++)
            {
                bool isEnd = TriggerTimeTrack(curTime,trackList[i]);
                //有一个没有结束，就是没有结束
                if (isEnd == false)
                    RunFinish = false;
            }
        }

        private bool TriggerTimeTrack(float curTime,TimeTrack track)
        {
            if (track.IsEnd)
            {
                return true;
            }
            RunFinish = false;

            //Debug.Log("TriggerTimeTrack>>>>>>>>>>>>>>"+curTime);
            //Debug.Log("TriggerTimeTrack StartTime>>>>>>>>>>>>>>" + track.StartTime);
            //Debug.Log("TriggerTimeTrack EndTime >>>>>>>>>>>>>>" + track.EndTime);
            //start检测
            if (track.IsStart == false && curTime >= track.StartTime)
            {
                track.CallStart();
                track.IsStart = true;
            }

            //update检测
            if (track.IsEnd == false && curTime > track.StartTime && curTime < track.EndTime)
            {
                track.CallUpdate();
            }

            //end检测
            if (track.IsEnd == false && curTime >= track.EndTime)
            {
                track.CallCompleted();
                track.IsEnd = true;
            }

            return false;
        }
    }

    /// <summary>
    /// 时间线管理
    /// </summary>
    public class TimeLineMgr
    {
        public static float CurrTime
        {
            get
            {
                return Time.realtimeSinceStartup;
            }
        }

        private List<TimeLine> TimeLines = new List<TimeLine>();

        public void Update()
        {
            for (int i = 0; i < TimeLines.Count; i++)
            {
                if (TimeLines[i].RunFinish)
                {
                    TimeLines.RemoveAt(i);
                }
                else
                {
                    TimeLines[i].Update();
                }
            }
        }

        public TimeLine CreateTimeLine()
        {
            TimeLine line = new TimeLine();
            TimeLines.Add(line);
            return line;
        }

        public TimeLine AddTimeLine(TimeLine line)
        {
            TimeLines.Add(line);
            return line;
        }

        public void Clear()
        {
            TimeLines.Clear();
        }
    }

    /// <summary>
    /// 时间线辅助函数
    /// </summary>
    public static class TimeLineHelp
    {
        private static TimeLineMgr timeLineMgr = new TimeLineMgr();

        public static void Update()
        {
            timeLineMgr.Update();
        }

        public static void Clear()
        {
            timeLineMgr.Clear();
        }

        public static TimeLine CreateTimeLine()
        {
            return timeLineMgr.CreateTimeLine();
        }

        public static TimeLine AddTimeLine(TimeLine line)
        {
            return timeLineMgr.AddTimeLine(line);
        }
    }
}
