/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   Plane.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/8/3 17:42:52
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Plane : MonoBehaviour
{

    public GameObject PathObj;

    public float MoveSpeed = 60;

    public float AngleSpeed = 10;

    public List<Vector3> PathList;

    public int Index = 0;

    // Use this for initialization
    void Start()
    {
        if (PathObj == null)
            return;

        PathList = new List<Vector3>();

        for (int i = 0; i < PathObj.transform.childCount; ++i)
        {
            PathList.Add(PathObj.transform.GetChild(i).transform.position);
        }

        olddir = PathList[Index] - transform.position;
    }

    void Destroy()
    {

    }

    private Vector3 olddir;
    // Update is called once per frame
    void Update()
    {
        Vector3 dir = PathList[Index] - transform.position;
        //if (dir.magnitude < 0.2f || Vector3.Angle(dir, olddir) > 120)
        if(dir.magnitude < 1.5f)
        {
            ++Index;
            Index %= PathList.Count;
        }
        else
        {
            transform.position += dir.normalized * Time.deltaTime * MoveSpeed;
        }
        Quaternion toRotation = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, Time.deltaTime * AngleSpeed);
        olddir = dir;
    }
}
