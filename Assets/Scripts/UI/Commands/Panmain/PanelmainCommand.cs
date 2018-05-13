/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   Panmain.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/5/10 13:53:59
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;

public class PanelmainCommand : PureMVC.Patterns.SimpleCommand
{
    public override void Execute(PureMVC.Interfaces.INotification notification)
    {
        Facade.RegisterProxy(new PanelMainProxy(PanelMainProxy.NAME));
    }
}
