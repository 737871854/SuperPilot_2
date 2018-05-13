/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   Fushi.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/8/3 17:53:17
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;

public class Fushi : MonoBehaviour
{
    public Transform StartTran;
    public Transform EndTran;
    public float MaxSpeed = 40;
    public float MinSpeed = 20;
    public float Gravity = 9.8f;
    public bool IsDown = true;

    private Transform TargetTran;
    private float CurSpeed;
    // Use this for initialization
    void Start()
    {
        if (IsDown)
            TargetTran = EndTran;
        else
            TargetTran = StartTran;

        OldDir = TargetTran.position - transform.position;
        MinSpeed = MinSpeed <= 0 ? 1 : MinSpeed;
    }

    private Vector3 OldDir;
    // Update is called once per frame
    void Update()
    {
        Vector3 dir = TargetTran.position - transform.position;
        if (dir.magnitude < 0.2f || Vector3.Angle(OldDir, dir) > 150)
        {
            IsDown = !IsDown;
            if (IsDown)
            {
                TargetTran = EndTran;
                OldDir = EndTran.position - StartTran.position;
            }
            else
            {
                TargetTran = StartTran;
                OldDir = StartTran.position - EndTran.position;
            }
        }
        else
        {
            transform.position += dir.normalized * Time.deltaTime * CurSpeed;
            OldDir = dir;
        }

        // Up
        if (IsDown)
        {
            if (CurSpeed < MaxSpeed)
                CurSpeed += Gravity * Time.deltaTime;
            else
                CurSpeed = MaxSpeed;
        }
        else
        {
            if (CurSpeed > MinSpeed)
                CurSpeed -= Gravity * Time.deltaTime;
            else
                CurSpeed = MinSpeed;
        }
    }
}
