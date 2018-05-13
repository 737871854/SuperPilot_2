/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   PanelSelectProxy.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/5/9 17:07:37
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;

public class PanelSelectProxy : PureMVC.Patterns.Proxy
{
    public const string NAME = MyProxyName.PanelSelectProxy;
    public const string UPDATE_COIN = MyProxyUpdated.PanelSelectProxy_UpdateCoin;

    public PanelSelectProxy(string proxyName) : base(proxyName, null) { }

    private int coin;
    private int rate;

    public int Coin { get { return coin; } }
    public int Rate { get { return rate; } }

    public override void OnRegister()
    {
        base.OnRegister();
    }

    public override void OnRemove()
    {
        base.OnRemove();
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
