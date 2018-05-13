/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   GameObjectPath.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/7/15 11:40:08
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;

public class GameObjectPathList : ScriptableObject
{
    public Vector3[] Path;
    public Vector3[] PathUp;
    public Vector3[] PathRight;
    public float PathLength;
}
