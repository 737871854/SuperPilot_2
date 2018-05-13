/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   PanelMainMediator.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/5/10 9:56:58
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using Need.Mx;
using System.Collections;

public class PanelMainMediator : PureMVC.Patterns.Mediator
{
    public const string NAME = MyMediatorName.PanelMainMediator;

    private PanelMainProxy proxy;

    private PanelMainLogic ui { get { return ((GameObject)ViewComponent).GetComponent<PanelMainLogic>(); } }

    public PanelMainMediator(string mediatorName, object viewComponent) : base(mediatorName, viewComponent) { }

    public override IList<string> ListNotificationInterests()
    {
        IList<string> list = new List<string>();
        list.Add(PanelMainProxy.UPDATED_COIN);
        return list;
    }

    public override void HandleNotification(PureMVC.Interfaces.INotification notification)
    {
        switch(notification.Name)
        {
            case PanelMainProxy.UPDATED_COIN:
                ui.UpdateCoin(proxy.Coin, proxy.Rate);
                break;
        }
    }

    public override void OnRegister()
    {
        proxy = Facade.RetrieveProxy(PanelMainProxy.NAME) as PanelMainProxy;

        ui.InitMediator(this);

        proxy.Init(SettingManager.Instance.HasCoin(0), SettingManager.Instance.GameRate);

        EventDispatcher.AddEventListener(EventDefine.Event_Update_Coin, UpdateCoin);
        EventDispatcher.AddEventListener(EventDefine.Event_Sure_Or_Missile, OnSure);
        EventDispatcher.AddEventListener(EventDefine.Event_On_TieQian_Effect, OnTieQianEffect);
        EventDispatcher.AddEventListener<bool>(EventDefine.Event_On_Trigger_JiGuang_Weapon, OnJiGuangAttack);
        EventDispatcher.AddEventListener(EventDefine.Event_OnTrigger_Wall, OnWall);
        EventDispatcher.AddEventListener<bool>(EventDefine.Event_On_Describe, OnDescribe);
        EventDispatcher.AddEventListener(EventDefine.Event_Trigger_Ice, OnTriggerIce);
        EventDispatcher.AddEventListener<bool>(EventDefine.Event_Missile_Fired_Or_Trigger, OnMissile);
        EventDispatcher.AddEventListener<int, Vector3, int>(EventDefine.Event_Tips_Type_By_Int, OnTips);
        EventDispatcher.AddEventListener(EventDefine.Event_Boss_Warning, OnBossWarning);
        EventDispatcher.AddEventListener<bool>(EventDefine.Event_Enter_Follow_Water, OnWater);
        EventDispatcher.AddEventListener(EventDefine.Event_Can_Attack_Gorge_Boss, OnAttackGorgeBoss);
        EventDispatcher.AddEventListener<Vector3>(EventDefine.Event_Lock_Pos, OnShowUI);
        EventDispatcher.AddEventListener(EventDefine.Event_Groge_Boss_Attack, OnGorgeAttack);
        EventDispatcher.AddEventListener<bool>(EventDefine.Event_Hurry_ShowOrHide, OnHurry);
        EventDispatcher.AddEventListener<List<GorgeBossHitTrigger>>(EventDefine.Event_Gorge_Boss_QTE, OnQTE);
        EventDispatcher.AddEventListener<Vector3>(EventDefine.Event_Hit_Break, OnBreak);
    }

    public override void OnRemove()
    {
        EventDispatcher.RemoveEventListener(EventDefine.Event_Update_Coin, UpdateCoin);
        EventDispatcher.RemoveEventListener(EventDefine.Event_Sure_Or_Missile, OnSure);
        EventDispatcher.RemoveEventListener(EventDefine.Event_On_TieQian_Effect, OnTieQianEffect);
        EventDispatcher.RemoveEventListener<bool>(EventDefine.Event_On_Trigger_JiGuang_Weapon, OnJiGuangAttack);
        EventDispatcher.RemoveEventListener(EventDefine.Event_OnTrigger_Wall, OnWall);
        EventDispatcher.RemoveEventListener<bool>(EventDefine.Event_On_Describe, OnDescribe);
        EventDispatcher.RemoveEventListener(EventDefine.Event_Trigger_Ice, OnTriggerIce);
        EventDispatcher.RemoveEventListener<bool>(EventDefine.Event_Missile_Fired_Or_Trigger, OnMissile);
        EventDispatcher.RemoveEventListener<int, Vector3, int>(EventDefine.Event_Tips_Type_By_Int, OnTips);
        EventDispatcher.RemoveEventListener(EventDefine.Event_Boss_Warning, OnBossWarning);
        EventDispatcher.RemoveEventListener<bool>(EventDefine.Event_Enter_Follow_Water, OnWater);
        EventDispatcher.RemoveEventListener(EventDefine.Event_Can_Attack_Gorge_Boss, OnAttackGorgeBoss);
        EventDispatcher.RemoveEventListener<Vector3>(EventDefine.Event_Lock_Pos, OnShowUI);
        EventDispatcher.RemoveEventListener(EventDefine.Event_Groge_Boss_Attack, OnGorgeAttack);
        EventDispatcher.RemoveEventListener<bool>(EventDefine.Event_Hurry_ShowOrHide, OnHurry);
        EventDispatcher.RemoveEventListener<List<GorgeBossHitTrigger>>(EventDefine.Event_Gorge_Boss_QTE, OnQTE);
        EventDispatcher.RemoveEventListener<Vector3>(EventDefine.Event_Hit_Break, OnBreak);
    }

    #region Private Fucntion

    /// <summary>
    /// 更新投币
    /// </summary>
    /// <param name="value"></param>
    private void UpdateCoin()
    {
        proxy.AddCoin();
    }

    /// <summary>
    /// 续币
    /// </summary>
    private void OnSure()
    {
        if (!proxy.CanPlay() || ioo.gameMode.State != GameState.Continue)
            return;

        proxy.ChangePlay();
        ioo.gameMode.RunMode(GameState.Play);
    }

    /// <summary>
    /// 被铁钳怪抓住
    /// </summary>
    private void OnTieQianEffect()
    {
        ui.OnTieQianEffect();
    }

    private void OnJiGuangAttack(bool value)
    {
        ui.OnJiGuangAttack(value);
    }

    private void OnWall()
    {
        ui.OnWall();
    }

    /// <summary>
    /// 道具介绍说明
    /// </summary>
    /// <param name="value"></param>
    private void OnDescribe(bool value)
    {
        ui.OnDescribe(value);
    }

    private void OnTriggerIce()
    {
        ui.OnTriggerIce();
    }

    private void OnMissile(bool value)
    {
        ui.MissileUsed(value);
    }

    public void OnTips(int type, Vector3 pos, int value)
    {
        ui.OnTips(type, pos, value);
    }

    public bool CoinIsEnough()
    {
        if (proxy.Coin >= proxy.Rate)
            return true;
        return false;
    }

    private void OnBossWarning()
    {
        ui.OnBossWarning();
    }

    private void OnAttackGorgeBoss()
    {
        ui.Lock();
    }

    private void OnWater(bool value)
    {
        ui.OnWater(value);
    }

    private void OnShowUI(Vector3 pos)
    {
        ui.OnShowUI(pos);
    }

    private void OnGorgeAttack()
    {
        ui.OnGorgeAttack();
    }

    private void OnHurry(bool value)
    {
        if (value)
        {
            ui.ShowHurryPush();
        }
        else
        {
            ui.HideHurryPush();
        }
    }

    private void OnQTE(List<GorgeBossHitTrigger> list)
    {
        ui.OnQTE(list);
    }

    private void OnBreak(Vector3 pos)
    {
        ui.OnBreak(pos);
    }
    #endregion

    #region Public Function
    public string CointinueNumber()
    {
        string text = (proxy.Coin - proxy.Rate) >= 0 ? "0" : (proxy.Rate - proxy.Coin).ToString();
        return text;
    }

    /// <summary>
    /// 返回机仓
    /// </summary>
    public void ToBack()
    {
        ioo.audioManager.StopBackMusic("SFX_Sound_Boss_Warning");
        ioo.gameMode.RunMode(GameState.Back);
        ioo.audioManager.StopAll();
        ioo.audioManager.PlayPersonSound("Person_Soune_Mission_Complete");
        EventDispatcher.TriggerEvent(EventDefine.Event_Enter_Follow_Water, false);
        EventDispatcher.TriggerEvent(EventDefine.Event_Trigger_Effect_Dust, false);
    }

    #endregion
}
