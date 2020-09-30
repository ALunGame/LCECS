using LCECS.Help;
using LCSkill;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


public class TestTaskThread
{
    static List<int> tmplist = new List<int>();

    public static int Test()
    {
        tmplist = new List<int>();
        tmplist.Add(10);
        tmplist.Add(22);

        Thread.Sleep(1000);
        Debug.Log("ExcuteTask Test >>>>>>>>>>> Thread ID  " + Thread.CurrentThread.ManagedThreadId);
        tmplist.RemoveAt(0);

        return tmplist.Count;
    }

    public static void TestLog(int tt)
    {
        Debug.LogError("TestLog》》》》》》》" + tt);
    }
}

public class Test001
{
    public string Name = "";

    public string Age = "";

    public string Cnt = "";

    private string tt = "";

    public string Tt { get => tt; set { tt = value; CallBack(); } }
    
    public void CallBack()
    {
        
    }
}

public class TestTask : MonoBehaviour
{
    public GameObject testGo00;
    public GameObject testGo01;
    public GameObject testGo02;
    public GameObject testGo03;
    public int CallCnt = 0;

    public string TimeKey;
    public object lockObj = new object();
    // Start is called before the first frame update
    void Start()
    {
        //TimeLine line = TimeLineHelp.CreateTimeLine();
        //line.AddTrack(1, 5).OnStart(() =>
        //{
        //    Debug.Log("OnStart");
        //}).OnUpdate(()=> 
        //{
        //    Debug.Log("OnUpdate");
        //}).OnCompleted(()=> 
        //{
        //    Debug.Log("OnCompleted");
        //});
        //line.Start();

        //Debug.Log("Start>>>>>>>   Thread ID  " + Thread.CurrentThread.ManagedThreadId);
        //TimeKey = TimerHelp.AddTimer(1, 2, () =>
        //  {
        //      lock (lockObj)
        //      {
        //          Debug.Log("AddTimer>>>>>>>   Thread ID  " + Thread.CurrentThread.ManagedThreadId);
        //          Debug.Log(testGo00.name);
        //          testGo00.name = "AddTimer";
        //      }
        //  });
        TaskHelp.AddTask(ExcuteTask11, FinishTask11);
    }

    private void Update()
    {
        TimeLineHelp.Update();
        if (Input.GetKeyDown(KeyCode.A))
        {
            CallCnt--;
        }    
    }

    private void OnDestroy()
    {
        TimerHelp.RemoveTime(TimeKey);
    }

    private void ExcuteSkill(SkillJson skill)
    {
        //skill.Data.Sort((SkillDataJson a, SkillDataJson b) => {
        //    return -a.Time.CompareTo(b.Time);
        //});
        //skill.Effects.Sort((SkillEffectJson a, SkillEffectJson b) => {
        //    return -a.Time.CompareTo(b.Time);
        //});
        //skill.Audios.Sort((SkillAudioJson a, SkillAudioJson b) => {
        //    return -a.Time.CompareTo(b.Time);
        //});
    }

    #region Task

    private string ExcuteTask00()
    {
        Thread.Sleep(5000);
        Debug.Log("ExcuteTask00 >>>>>>>>>>> Thread ID  " + Thread.CurrentThread.ManagedThreadId);
        return "ExcuteTask00 完成";
    }

    private void FinishTask00(string info)
    {
        Debug.LogError("CallCnt>>>>>>>>>>>" + CallCnt);
        CallCnt++;
        testGo00.name = info;
        //Debug.Log("FinishTask00>>>>>>>   Thread ID  " + Thread.CurrentThread.ManagedThreadId);
    }

    private string ExcuteTask01(string pam)
    {
        Debug.Log("参数》》》》》》" + pam);
        Debug.Log("ExcuteTask01 >>>>>>>>>>> Thread ID  " + Thread.CurrentThread.ManagedThreadId);
        return "ExcuteTask01 完成";
    }

    private void FinishTask01(string info)
    {
        Debug.Log("info>>>>>>>>>>>" + info);
        testGo01.name = info;
        Debug.Log("FinishTask01>>>>>>>   Thread ID  " + Thread.CurrentThread.ManagedThreadId);
    }

    private string ExcuteTask02(string pam01, string pam02)
    {
        Debug.Log("参数》》》》》》" + pam01 + " " + pam02);
        Debug.Log("ExcuteTask02 >>>>>>>>>>> Thread ID  " + Thread.CurrentThread.ManagedThreadId);
        return "ExcuteTask02 完成";
    }

    private void FinishTask02(string info)
    {
        Debug.Log("info>>>>>>>>>>>" + info);
        testGo02.name = info;
        Debug.Log("FinishTask02>>>>>>>   Thread ID  " + Thread.CurrentThread.ManagedThreadId);
    }

    private string ExcuteTask03(string pam01, string pam02, string pam03)
    {
        Debug.Log("参数》》》》》》" + pam01 + " " + pam02 + " " + pam03);
        Debug.Log("ExcuteTask03 >>>>>>>>>>> Thread ID  " + Thread.CurrentThread.ManagedThreadId);
        return "ExcuteTask03 完成";
    }

    private void FinishTask03(string info)
    {
        Debug.Log("info>>>>>>>>>>>" + info);
        testGo03.name = info;
        Debug.Log("FinishTask03>>>>>>>   Thread ID  " + Thread.CurrentThread.ManagedThreadId);
    }

    #endregion

    private (string,int) ExcuteTask11()
    {
        Thread.Sleep(5000);
        Debug.Log("ExcuteTask00 >>>>>>>>>>> Thread ID  " + Thread.CurrentThread.ManagedThreadId);
        return ("aaa", 1);
    }

    private void FinishTask11((string, int) info)
    {
        Debug.LogError("info.Item1>>>>>>>>>>>" + info.Item1);
        Debug.LogError("info.Item2>>>>>>>>>>>" + info.Item2);
        CallCnt++;
        testGo00.name = info.Item1;
        Debug.Log("FinishTask00>>>>>>>   Thread ID  " + Thread.CurrentThread.ManagedThreadId);
    }
}
