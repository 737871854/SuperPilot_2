/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   EngineBehaviour.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/8/8 17:55:08
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;
using Need.Mx;

public class EngineBehaviour : MonoBehaviour
{
    public Vector3 LocalStart;  // 飞机挂点
    public Vector3 LocalEnd;    // 消失点

    public GameObject Engine0;
    public GameObject Engine1;

    public float Speed = 2.5f;

    private Vector3 targetPos;
    private Vector3 olddir;
    // Use this for initialization
    void Start()
    {
        EventDispatcher.AddEventListener<bool>(EventDefine.Event_Engine_Speed_Up, OnSpeedUp);
    }

    void Destroy()
    {
        EventDispatcher.RemoveEventListener<bool>(EventDefine.Event_Engine_Speed_Up, OnSpeedUp);
    }

    // Update is called once per frame
    void Update()
    {
        if (ioo.gameMode.Player.PathType == Player.E_Path.Land)
        {
            gameObject.SetActive(false);
        }

        Vector3 dir = targetPos - transform.localPosition;
        if (dir.magnitude < 0.1f || Vector3.Angle(olddir, dir) > 120)
        {
            if (targetPos == LocalStart)
            {
                transform.localPosition = targetPos;
            }
            else
            {
                Engine0.SetActive(false);
                Engine1.SetActive(false);
            }
        }
        else
        {
            transform.localPosition += dir.normalized * Time.deltaTime * Speed;
            olddir = dir;
        }
    }

    private void OnSpeedUp(bool speedup)
    {
        if (speedup)
        {
            transform.localPosition = LocalEnd;
            targetPos = LocalStart;
            Engine0.SetActive(true);
            Engine1.SetActive(true);
            olddir = LocalStart - LocalEnd;
        }
        else
        {
            targetPos = LocalEnd;
            olddir = LocalEnd - LocalStart;
        }
    }
}
