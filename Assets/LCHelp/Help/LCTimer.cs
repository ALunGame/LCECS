using System;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

namespace LCHelp
{
    public class TimerModel
    {
        public string Key="";

        public long Time;

        //时间间隔(毫秒)
        public int IntervalTime;

        //循环次数
        public int LoopTimes;

        private Action TimeCallBack;

        public TimerModel(string key, long time, int intervalTime, int loopTimes,Action callBack)
        {
            this.Key            = key;
            this.Time           = time;
            this.IntervalTime   = intervalTime;
            this.LoopTimes      = loopTimes;
            this.TimeCallBack   = callBack;
        }

        public void Run()
        {
            TaskHelp.AddTask(() => { }, TimeCallBack);
        }

    }

    /// <summary>
    /// 定时器辅助类
    /// </summary>
    public static class TimerHelp
    {
        private static Timer timer;
        private static List<TimerModel> timeList = new List<TimerModel>();
        
        static TimerHelp()
        {
            timer          = new Timer(100);
            timer.Elapsed += TimerCallBack;
        }

        private static long GetCurrMillisecond()
        {
            return DateTime.Now.Ticks / 10000;
        }

        private static void TimerCallBack(object obj, ElapsedEventArgs e)
        {
            for (int i = 0; i < timeList.Count; i++)
            {
                TimerModel tim = timeList[i];
                //到时间了
                if (tim.Time <= GetCurrMillisecond())
                {
                    tim.Run();
                    //一直循环
                    if (tim.LoopTimes==-1)
                    {
                        //更新时间
                        tim.Time = GetCurrMillisecond() + tim.IntervalTime;
                    }
                    else
                    {
                        tim.LoopTimes--;
                        if (tim.LoopTimes<=0)
                        {
                            timeList.RemoveAt(i);
                        }
                        else
                        {
                            //更新时间
                            tim.Time = GetCurrMillisecond() + tim.IntervalTime;
                        }
                    }
                }
            }

            if (timeList.Count<=0)
            {
                timer.Stop();
            }
        }

        private static TimerModel GetTimerModel(string key)
        {
            for (int i = 0; i < timeList.Count; i++)
            {
                if (timeList[i].Key== key)
                {
                    return timeList[i];
                }
            }

            return null;
        }

        private static string AddTimeModel(long delayTime, int intervalTime,int loopTime, Action callBack)
        {
            string key = callBack.Target.ToString() + callBack.Method.Name;
            TimerModel model= GetTimerModel(key);
            if (model!=null)
            {
                Debug.LogError("重复的计时器》》》》》"+key);
                return key;
            }

            model = new TimerModel(key, GetCurrMillisecond() + delayTime, intervalTime, loopTime, callBack);
            timeList.Add(model);

            if (timer.Enabled==false)
            {
                timer.Start();
            }
            return key;
        }

        public static void RemoveTime(string key)
        {
            for (int i = 0; i < timeList.Count; i++)
            {
                if (timeList[i].Key == key)
                {
                    timeList.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// 添加定时任务 指定延迟的时间  秒
        /// </summary>
        public static string AddTimer(float delayTime, float intervalTime,Action timeCallBack, int loopTime=-1)
        {
            return AddTimeModel((long)(delayTime * 1000), (int)(intervalTime * 1000), loopTime, timeCallBack);
        }
    }
}