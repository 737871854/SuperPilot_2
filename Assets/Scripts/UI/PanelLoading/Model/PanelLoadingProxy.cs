/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   PanelLoadingProxy.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/5/23 15:13:38
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;

public class PanelLoadingProxy : PureMVC.Patterns.Proxy
{
    public const string NAME = MyProxyName.PanelLoadingProxy;
    public const string UPDATED_PROGRESS = MyProxyUpdated.PanelLoading_Update_Num;
    public const string UPDATE_COIN = MyProxyUpdated.PanelLoading_Update_Coin;

    private int _ToProgress;
    private int coin;
    private int rate;

    public int Coin { get { return coin; } }
    public int Rate { get { return rate; } }

    public int ToProgress { get { return _ToProgress; } }

    public PanelLoadingProxy(string proxyName) : base(proxyName, null) { }

    public override void OnRegister()
    {
        base.OnRegister();
    }

    public override void OnRemove()
    {
        base.OnRemove();
    }

    public void Progress(int progress)
    {
        _ToProgress = progress;
        SendNotification(UPDATED_PROGRESS);
    }

    public void Init(int value0, int value1)
    {
        coin = value0;
        rate = value1;
        SendNotification(UPDATE_COIN);
    }

    public void AddCoin()
    {
        ++coin;
        SendNotification(UPDATE_COIN);
    }

}
