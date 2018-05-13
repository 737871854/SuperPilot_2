/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   PanelCheckHardProxy.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/5/12 18:03:48
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;

public class PanelCheckHardProxy : PureMVC.Patterns.Proxy
{
    public const string NAME = MyProxyName.PanelCheckHardProxy;

    public PanelCheckHardProxy(string proxyName) : base(proxyName, null) { }

    public override void OnRegister()
    {
        base.OnRegister();
    }

    public override void OnRemove()
    {
        base.OnRemove();
    }
}
