/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   PanelLoadingMediator.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/5/23 15:14:07
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using Need.Mx;

public class PanelLoadingMediator : PureMVC.Patterns.Mediator
{
    public const string NAME = MyMediatorName.PanelLoadingMediator;

    private PanelLoadingProxy proxy;

    private PanelLoadingLogic ui { get { return ((GameObject)ViewComponent).GetComponent<PanelLoadingLogic>(); } }

    public PanelLoadingMediator(string mediatorName, object viewComponent) : base(mediatorName, viewComponent) { }

    public override IList<string> ListNotificationInterests()
    {
        IList<string> list = new List<string>();
        list.Add(PanelLoadingProxy.UPDATED_PROGRESS);
        list.Add(PanelLoadingProxy.UPDATE_COIN);
        return list;
    }

    public override void HandleNotification(PureMVC.Interfaces.INotification notification)
    {
        switch(notification.Name)
        {
            case PanelLoadingProxy.UPDATED_PROGRESS:
                ui.SetProgress(proxy.ToProgress);
                break;
            case PanelLoadingProxy.UPDATE_COIN:
                ui.UpdateCoin(proxy.Coin, proxy.Rate);
                break;
        }
    }

    public override void OnRegister()
    {
        proxy = Facade.RetrieveProxy(PanelLoadingProxy.NAME) as PanelLoadingProxy;

        ui.InitMediator(this);

        proxy.Init(SettingManager.Instance.HasCoin(0), SettingManager.Instance.GameRate);

        EventDispatcher.AddEventListener(EventDefine.Event_Update_Coin, OnCoin);
        EventDispatcher.AddEventListener<int>(EventDefine.Event_Loading_Progress, OnProgress);
        EventDispatcher.AddEventListener(EventDefine.Event_Sure_Or_Missile, OnSkip);
    }

    public override void OnRemove()
    {
        EventDispatcher.RemoveEventListener(EventDefine.Event_Update_Coin, OnCoin);
        EventDispatcher.RemoveEventListener(EventDefine.Event_Sure_Or_Missile, OnSkip);
        EventDispatcher.RemoveEventListener<int>(EventDefine.Event_Loading_Progress, OnProgress);
    }

    private void OnCoin()
    {
        proxy.AddCoin();
    }

    private void OnSkip()
    {
        ui.OnSkip();
    }

    private void OnProgress(int toProgress)
    {
        proxy.Progress(toProgress);
    }

    public void LoadingEnd()
    {
        ioo.audioManager.StopPersonMusic("Person_Sound_Guided");
        EventDispatcher.TriggerEvent(EventDefine.Event_Loading_End);
    }
}
