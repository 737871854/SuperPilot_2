/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   PanelMainProxy.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/5/10 9:53:50
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;

public class PanelMainProxy : PureMVC.Patterns.Proxy
{
    public const string NAME = MyProxyName.PanelMainProxy;
    public const string UPDATED_COIN = MyProxyUpdated.PanelMainProxy_UpdateCoin;

    public PanelMainProxy(string proxyName) : base(proxyName, null) { }

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
        SendNotification(UPDATED_COIN);
    }

    public void AddCoin()
    {
        ++coin;
        SendNotification(UPDATED_COIN);
    }

    public bool CanPlay()
    {
        if (coin >= rate)
            return true;
        return false;
    }

    public void ChangePlay()
    {
        coin -= rate;
        SendNotification(UPDATED_COIN);
        SettingManager.Instance.UseCoin();
        SettingManager.Instance.Save();
    }
}
