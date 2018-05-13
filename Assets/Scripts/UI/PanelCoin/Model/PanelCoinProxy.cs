/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   PanelCoinProxy.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/5/9 10:57:33
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;

public class PanelCoinProxy : PureMVC.Patterns.Proxy
{
    public const string NAME = MyProxyName.PanelCoinProxy;
    public const string UPDATED_VIEW = MyProxyUpdated.PanelCoinProxy_UpdatedView;

    public int m_coin;
    public int m_need;

    public int Coins { get { return m_coin; } }
    public int Need { get { return m_need; } }

    public PanelCoinProxy(string proxyName) : base(proxyName, null) { }
    public MovieTexture MovieTex { get; set; }

    public override void OnRegister()
    {

    }

    public override void OnRemove()
    {

    }

    /// <summary>
    /// UI界面初始化时，数据
    /// </summary>
    /// <param name="value"></param>
    public void InitCoin(int value)
    {
        m_coin = value;
        SendNotification(UPDATED_VIEW);
    }

    /// <summary>
    /// 消耗
    /// </summary>
    public void UseCoin()
    {
        m_coin -= m_need;
        SendNotification(UPDATED_VIEW);
        SettingManager.Instance.UseCoin();
        SettingManager.Instance.Save();
    }

    /// <summary>
    /// 投币
    /// </summary>
    public void AddCoin()
    {
        ++m_coin;
        SendNotification(UPDATED_VIEW);
    }

    /// <summary>
    /// 修改币率
    /// </summary>
    /// <param name="value"></param>
    public void ChangeNeed(int value)
    {
        m_need = value;
        SendNotification(UPDATED_VIEW);
    }


}
