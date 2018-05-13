/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   Follow.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/9/5 14:14:16
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;

public class Follow : MonoBehaviour
{
    private Camera camera;
    public float MoveSpeed = 15;
    public float RotationSpeed = 20;
    // Use this for initialization
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
        if (ioo.gameMode.Player == null|| ioo.gameMode.Player.PathType != Player.E_Path.Game)
            return;
        camera.enabled = true;
       
        last += Time.deltaTime;
        if (last <= 9)
        {
            transform.position -= Vector3.up * Time.deltaTime * MoveSpeed;

            transform.localEulerAngles += Vector3.up * Time.deltaTime * RotationSpeed;
        }else if (last > 9 && last < 10)
        {
            transform.localEulerAngles = Vector3.right * 77;
        }else if (last > 10 && last <= 20)
        {
            transform.localEulerAngles -= Vector3.right * Time.deltaTime * 10;
        }
        else if (last > 20)
        {
            camera.enabled = false;
        }
    }
}
