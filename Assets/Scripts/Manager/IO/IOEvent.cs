/**
* Copyright (c) 2012,广州纷享游艺设备有限公司
* All rights reserved.
* 
* 文件名称：IOEvent.cs
* 简    述：每条协议包含的信息
* 创建标识：meij  2015/11/2
* 修改标识：meij  2015/11/06
* 修改描述：代码优化。
*/
using UnityEngine;
using System.Collections;

public class IOEvent
{
    //1：投币  0：否
    public bool IsCoin { get; set; }

    //1: 开始   0：否
    public bool IsStart { get;  set; }

    //1: 导弹   0：否
    public bool IsMissile { get;  set; }

    //1: 左   0：否
    public bool IsTurnLeft { get; set; }

    //1: 右   0：否
    public bool IsTurnRight { get; set; }

    //1: 上   0：否
    public bool IsPullUp { get; set; }

    //1: 下   0：否
    public bool IsPullDown { get; set; }

    //1: 加速   0：否
    public bool IsGather { get; set; }

    //1: A   0：否
    public bool IsConfirm { get; set; }

    //1: B   0：否
    public bool IsSelect { get; set; }

    // 复位光眼
    public bool IsResetEye { get; set; }

    // 上光眼
    public bool IsUpEye { get; set; }

    // 下光眼
    public bool IsDownEye { get; set; }

    public bool IsTicket { get; set; }

    public void Reset_0()
    {
        IsCoin      = false;
        if (ioo.gameMode.State != GameState.Play)
        {
            IsStart = false;
        }
        IsMissile   = false;
        IsGather    = false;
        IsConfirm   = false;
        IsSelect    = false;
        IsResetEye  = false;
        IsUpEye     = false;
        IsDownEye   = false;
        IsTicket    = false;
    }

    public void Reset_1()
    {
        IsTurnLeft = false;
        IsTurnRight = false;
        IsPullUp = false;
        IsPullDown = false;
    }

    public bool IsOk
    {
        get
        {
            return IsCoin && IsStart && IsMissile && IsTurnLeft && IsTurnRight && IsPullUp && IsPullDown && IsConfirm && IsSelect && IsTicket;
        }
    }
}
