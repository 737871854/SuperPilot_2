/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   PanelSummaryMediator.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/5/11 8:44:14
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;

public class PanelSummaryMediator : PureMVC.Patterns.Mediator
{
    public const string NAME = MyMediatorName.PanelSummaryMediator;

    private PanelSummaryProxy proxy;

    private PanelSummaryLogic ui { get { return ((GameObject)ViewComponent).GetComponent<PanelSummaryLogic>(); } }

    public PanelSummaryMediator(string mediatorName, object viewComponent) : base(mediatorName, viewComponent) { }

    public override IList<string> ListNotificationInterests()
    {
        IList<string> list = new List<string>();
        list.Add(PanelSummaryProxy.UPDATED);
        return list;
    }

    public override void HandleNotification(PureMVC.Interfaces.INotification notification)
    {
        switch(notification.Name)
        {
            case PanelSummaryProxy.UPDATED:
                break;
        }
    }

    public override void OnRegister()
    {
        ui.InitMediator(this);
        ioo.audioManager.PlayBackMusic("Music_Summary");
        ioo.safeNet.CheckOutDog();
    }

    public override void OnRemove()
    {
        ioo.audioManager.StopBackMusic("Music_Summary");
    }

    public void ToStart()
    {
        ioo.gameMode.Reset();
    }

}
