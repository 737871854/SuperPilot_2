/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   PathManager.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/6/30 9:06:17
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;

public class PathManager : SingletonBehaviour<PathManager>
{
    public enum E_Type
    {
        City,
        Gorge,
    }

    public PathInfo PathInfo0;

    public PathInfo PathInfo1;

    public PathInfo PathInfo2;

    public E_Type Type;

    public void InitPath()
    {
        GameObjectPathList pathlist = Resources.Load<GameObjectPathList>(Const.Path_Config_Obj_Ready);
        PathInfo0.PathLength    = pathlist.PathLength;
        PathInfo0.Path          = pathlist.Path;
        PathInfo0.PathUp        = pathlist.PathUp;
        PathInfo0.PathRight     = pathlist.PathRight;

        if (Type == E_Type.City)
        {
            pathlist                = Resources.Load<GameObjectPathList>(Const.Path_Config_Obj_City);
            PathInfo1.PathLength    = pathlist.PathLength;
            PathInfo1.Path          = pathlist.Path;
            PathInfo1.PathUp        = pathlist.PathUp;
            PathInfo1.PathRight     = pathlist.PathRight;
        }

        if (Type == E_Type.Gorge)
        {
            pathlist                = Resources.Load<GameObjectPathList>(Const.Path_Config_Obj_Gorge);
            PathInfo1.PathLength    = pathlist.PathLength;
            PathInfo1.Path          = pathlist.Path;
            PathInfo1.PathUp        = pathlist.PathUp;
            PathInfo1.PathRight     = pathlist.PathRight;
        }

        pathlist                = Resources.Load<GameObjectPathList>(Const.Path_Config_Obj_Land);
        PathInfo2.PathLength    = pathlist.PathLength;
        PathInfo2.Path          = pathlist.Path;
        PathInfo2.PathUp        = pathlist.PathUp;
        PathInfo2.PathRight     = pathlist.PathRight;
        ReversePath(PathInfo2);
    }

    public override void Awake()
    {
        base.Awake();
    }

    public void ReversePath(PathInfo pathinfo)
    {
        int length = pathinfo.Path.Length;
        Vector3[] temp = new Vector3[length];
        for (int i = 0; i < temp.Length; ++i )
        {
            temp[i] = pathinfo.Path[length -1 - i];
        }
        pathinfo.Path = temp;
    }
}
