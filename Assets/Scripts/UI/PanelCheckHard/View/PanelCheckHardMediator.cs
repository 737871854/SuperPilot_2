/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   PanelCheckHardMediator.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/5/12 18:03:10
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;

public class PanelCheckHardMediator : PureMVC.Patterns.Mediator
{
    public const string NAME = MyMediatorName.PanelCheckHardMediator;

    private PanelCheckHardProxy proxy;

    private PanelCheckHardLogic ui { get { return ((GameObject)ViewComponent).GetComponent<PanelCheckHardLogic>(); } }

    public PanelCheckHardMediator(string mediatorName, object viewComponent) : base(mediatorName, viewComponent) { }

    public override IList<string> ListNotificationInterests()
    {
        return base.ListNotificationInterests();
    }

    public override void HandleNotification(PureMVC.Interfaces.INotification notification)
    {
        base.HandleNotification(notification);
    }

    public override void OnRegister()
    {
        proxy = Facade.RetrieveProxy(PanelCheckHardProxy.NAME) as PanelCheckHardProxy;
    }

    public override void OnRemove()
    {
        base.OnRemove();
    }
}
