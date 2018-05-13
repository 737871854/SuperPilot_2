/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   PanelCoinMediator.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/5/9 10:58:27
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using Need.Mx;
using System.Collections;

public class PanelCoinMediator : PureMVC.Patterns.Mediator
{
    public const string NAME = MyMediatorName.PanelCoinMediator;

    private PanelCoinProxy proxy;

    private PanelCoinLogic ui { get { return ((GameObject)ViewComponent).GetComponent<PanelCoinLogic>(); } }

    public PanelCoinMediator(string mediatorName, object viewComponent) : base(mediatorName, viewComponent) { }

    public override IList<string> ListNotificationInterests()
    {
        IList<string> list = new List<string>();
        list.Add(PanelCoinProxy.UPDATED_VIEW);
        return list;
    }

    public override void HandleNotification(PureMVC.Interfaces.INotification notification)
    {
        switch (notification.Name)
        {
            case PanelCoinProxy.UPDATED_VIEW:
                ui.UpdateView(proxy.Coins, proxy.Need);
                break;
        }
    }

    public override void OnRegister()
    {
        EnterGame = false;

        proxy = Facade.RetrieveProxy(PanelCoinProxy.NAME) as PanelCoinProxy;

        ui.InitMediator(this);

        EventDispatcher.AddEventListener(EventDefine.Event_Sure_Or_Missile, CanPlay);
        EventDispatcher.AddEventListener(EventDefine.Event_Update_Coin, UpdateCoin);
        EventDispatcher.AddEventListener(EventDefine.Event_Button_A, OpenSetting);
        EventDispatcher.AddEventListener<bool>(EventDefine.Event_Idle_Movie, OnIdleMovie);

        InitInfo(SettingManager.Instance.HasCoin(0), SettingManager.Instance.GameRate);

        ioo.audioManager.PlayBackMusic("Music_Panel_Coin");
    }

    public override void OnRemove()
    {
        EventDispatcher.RemoveEventListener(EventDefine.Event_Sure_Or_Missile, CanPlay);
        EventDispatcher.RemoveEventListener(EventDefine.Event_Update_Coin, UpdateCoin);
        EventDispatcher.RemoveEventListener(EventDefine.Event_Button_A, OpenSetting);
        EventDispatcher.RemoveEventListener<bool>(EventDefine.Event_Idle_Movie, OnIdleMovie);

        ioo.audioManager.StopBackMusic("Music_Panel_Coin");
    }

    public bool EnterGame;

    private void CanPlay()
    {
        if (proxy.Coins >= proxy.Need && !EnterGame)
        {
            EnterGame = true;
            ioo.audioManager.PlaySound2D("SFX_Sound_Sure");
            proxy.UseCoin();
            ui.ShowEffectSure();
            ioo.gameMode.RunMode(GameState.Waitting);
            CoroutineController.Instance.StartCoroutine(DelayToOpen());
        }
    }

    private void OpenSetting()
    {
        if (ioo.gameMode.State != GameState.Coin)
            return;

        UIManager.Instance.OpenUI(EnumUIType.PanelSetting);
        UIManager.Instance.CloseUI(EnumUIType.PanelCoin);
        ioo.audioManager.StopAll();
        //IOManager.Instance.SendMessageEnterSetting();
    }

    private void OnIdleMovie(bool value)
    {
        if (value)
            PlayIdleMovie();
        else
            StopIdleMovie();
    }


    IEnumerator DelayToOpen()
    {
        yield return new WaitForSeconds(1);
        ioo.gameMode.RunMode(GameState.Select);
        UIManager.Instance.CloseUI(EnumUIType.PanelCoin);
        UIManager.Instance.OpenUI(EnumUIType.PanelSelect);
    }

    /// <summary>
    /// 打开投币界面初始化币率和币数
    /// </summary>
    /// <param name="coin"></param>
    /// <param name="rate"></param>
    private void InitInfo(int coin, int rate)
    {
        proxy.InitCoin(coin);
        proxy.ChangeNeed(rate);
    }

    /// <summary>
    /// 更新投币
    /// </summary>
    /// <param name="value"></param>
    private void UpdateCoin()
    {
        proxy.AddCoin();
    }

    /// <summary>
    /// 更新币率
    /// </summary>
    /// <param name="value"></param>
    private void UpdateNeed(int value)
    {
        proxy.ChangeNeed(value);
    }

    private void PlayIdleMovie()
    {
        ioo.audioManager.StopBackMusic("Music_Panel_Coin");

        MovieTexture mt = proxy.MovieTex;
        if (mt == null)
        {
            mt = Resources.Load(Const.Path_Config_Idle_Movie_Path) as MovieTexture;
            proxy.MovieTex = mt;
        }

        if (mt != null)
            ui.PlayIdleMovie(mt);
    }

    private void StopIdleMovie()
    {
        ioo.audioManager.PlayBackMusic("Music_Panel_Coin");
        ui.StopIdleMovie();
    }
}
