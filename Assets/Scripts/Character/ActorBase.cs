/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   ActorBase.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/5/31 9:47:12
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;

public class ActorBase
{
    /// <summary>
    /// 生命值
    /// </summary>
    protected float _life;

    /// <summary>
    /// 最终得分
    /// </summary>
    protected int _resulScore;

    /// <summary>
    /// 道具积分
    /// </summary>
    protected int _score;

    /// <summary>
    /// Boss积分
    /// </summary>
    protected int _bossScore;

    /// <summary>
    /// 机身受损
    /// </summary>
    protected int _damageScore;
}
