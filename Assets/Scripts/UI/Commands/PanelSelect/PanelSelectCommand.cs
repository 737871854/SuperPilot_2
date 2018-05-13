/*
 * Copyright (c) 
 * 
 * 文件名称：   PanelSelectCommand.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/3/29 11:38:18
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;

public class PanelSelectCommand : PureMVC.Patterns.SimpleCommand
{
    public override void Execute(PureMVC.Interfaces.INotification notification)
    {
        Facade.RegisterProxy(new PanelSelectProxy(PanelSelectProxy.NAME));
    }
}
