/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   SwitchCamera.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/9/5 14:39:38
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;

public class SwitchCamera : MonoBehaviour
{
    private Camera camera;
    void Start()
    {
        camera = GetComponent<Camera>();
        camera.enabled = false;
    }

    void Update()
    {
        if (ioo.gameMode.Player == null || ioo.gameMode.Player.PathType != Player.E_Path.Game)
        {
            return;
        }

        Vector3 dir = ioo.gameMode.Player.transform.position - transform.position;

        if (dir.magnitude < 150)
        {
            camera.enabled = true;
        }
        else
        {
            camera.enabled = false;
        }
    }
}
