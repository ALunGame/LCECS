using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundParallax : MonoBehaviour
{
    public new Camera camera;//摄像机对象

    public Transform[] backgrounds;//不同层次的背景
    public float[] parallaxScales;//每个层次的视差比例（即背景与摄影机的移动量的比例）

    public float smoothTime;//看做是速度的倒数就好~

    private Vector3 lastCameraPosition;//上一次相机的位置，用于计算滚动量
    private Vector3 velocity;//速度向量，Vector3.SmoothDamp用到的变量

    void Start()
    {
        lastCameraPosition = camera.transform.position;//初始化lastCameraPosition
        velocity = Vector3.zero;//初始化velocity
    }

    void Update()
    {
        for (int i = 0, count = backgrounds.Length; i < count; i++)//遍历每个层次的背景
        {
            Vector3 parallax = (lastCameraPosition - camera.transform.position) * parallaxScales[i];//计算滚动量（滚动距离=摄像机移动距离*视差比例）
            parallax.z = 0f;//设置z轴滚动量为0，防止背景前后移动
            parallax.y = 0f;
            Vector3 target = backgrounds[i].position + parallax;//目标坐标=当前背景坐标+滚动量

            backgrounds[i].position = Vector3.SmoothDamp(backgrounds[i].position, target, ref velocity, smoothTime);//用Vector3.SmoothDamp将背景平滑移动
        }
        lastCameraPosition = camera.transform.position;//更新lastCameraPosition
    }
}
