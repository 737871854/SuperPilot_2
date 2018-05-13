/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   Around.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/9/5 15:06:39
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;

public class Around : MonoBehaviour
{
    private Transform Center;
    private Camera camera;
    // Use this for initialization
    void Start()
    {
        Center = transform.parent;
        camera = GetComponent<Camera>();
        camera.enabled = false;
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

        if (dir.magnitude < 600 && last < 8)
        {
            last += Time.deltaTime;
            Center.RotateAroundLocal(Vector3.up, Time.deltaTime);
            camera.enabled = true;
            transform.LookAt(Center.position);
        }
        else
        {
            camera.enabled = false;
        }
    }
}
