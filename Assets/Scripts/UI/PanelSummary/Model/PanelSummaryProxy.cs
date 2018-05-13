/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   PanelSummaryProxy.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/5/11 8:43:08
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;

public class PanelSummaryProxy : PureMVC.Patterns.Proxy
{
    public const string NAME = MyProxyName.PanelSummaryProxy;
    public const string UPDATED = MyProxyUpdated.PanelSummary_Update;

    public PanelSummaryProxy(string proxyName) : base(proxyName, null) { }

    public override void OnRegister()
    {
        base.OnRegister();
    }

    public override void OnRemove()
    {
        base.OnRemove();
    }
}
