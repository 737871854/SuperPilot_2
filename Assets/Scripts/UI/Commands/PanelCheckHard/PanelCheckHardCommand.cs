﻿/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   PanelCheckHardCommand.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/5/12 18:04:14
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;

public class PanelCheckHardCommand : PureMVC.Patterns.SimpleCommand
{
    public override void Execute(PureMVC.Interfaces.INotification notification)
    {
        Facade.RegisterProxy(new PanelCheckHardProxy(PanelCheckHardProxy.NAME));
    }
}
