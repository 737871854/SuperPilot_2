/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   FirstView.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/9/5 14:52:33
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;

public class FirstView : MonoBehaviour
{
    private Camera camera;
    void Start()
    {
        camera = GetComponent<Camera>();
        camera.enabled = false;
    }


    void Destroy()
    {

    }

    private float last;
    // Update is called once per frame
    void Update()
    {
        if (ioo.gameMode.Player == null || ioo.gameMode.Player.PathType != Player.E_Path.Game)
        {
            return;
        }

        Vector3 dir = ioo.gameMode.Player.transform.position - transform.position;

        if (dir.magnitude < 200 && last < 10)
        {
            camera.enabled = true;
            last += Time.deltaTime;
            transform.SetParent(ioo.gameMode.Player.FirstCameraView.transform);
            transform.localEulerAngles = Vector3.zero;
            transform.localPosition = Vector3.zero;
        }
        else
        {
            camera.enabled = false;
            transform.SetParent(null);
        }
    }
}
