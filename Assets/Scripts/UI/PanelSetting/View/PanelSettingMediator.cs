/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   PanelSettingMediator.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/5/17 8:25:35
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using Need.Mx;

public class PanelSettingMediator : PureMVC.Patterns.Mediator
{
    public const string NAME = MyMediatorName.PanelSettingMediator;

    private PanelSettingProxy proxy;

    private PanelSettingLogic ui { get { return ((GameObject)ViewComponent).GetComponent<PanelSettingLogic>(); } }

    public PanelSettingMediator(string mediaName, object viewComponent) : base(mediaName, viewComponent) { }

    public override IList<string> ListNotificationInterests()
    {
        IList<string> list = new List<string>();
        list.Add(PanelSettingProxy.UPDATED);
        list.Add(PanelSettingProxy.UPDATED_RATE);
        list.Add(PanelSettingProxy.UPDATED_VOLUME);
        list.Add(PanelSettingProxy.UPDATED_LANGUAGE);
        list.Add(PanelSettingProxy.UPDATED_MONTH);
        list.Add(PanelSettingProxy.UPDATED_TATAL);
        list.Add(PanelSettingProxy.UPDATED_TICKET);
        return list;
    }

    public override void HandleNotification(PureMVC.Interfaces.INotification notification)
    {
        switch(notification.Name)
        {
            case PanelSettingProxy.UPDATED:
            case PanelSettingProxy.UPDATED_LANGUAGE:
                ui.SetLanguage(proxy.Language);
                break;
            case PanelSettingProxy.UPDATED_RATE:
                break;
            case PanelSettingProxy.UPDATED_VOLUME:
                break;
            case PanelSettingProxy.UPDATED_TICKET:
                break;
            case PanelSettingProxy.UPDATAED_TIME:
                break;
            case PanelSettingProxy.UPDATED_MONTH:
                 curPage = SettingConfig.Page.BusinessRecord_Page;
                 ui.AccountToBusiness();
                 ui.SetBusinessPageInfo(proxy.Language, proxy.MonthData);
                 _timeIndex = proxy.TimeIndex;
                break;
            case PanelSettingProxy.UPDATED_TATAL:
                curPage = SettingConfig.Page.TotalRecord_Page;
                ui.AccountToTotal();
                ui.SetTotalRecordInfo(proxy.Language, proxy.Total);
                break;
        }
    }

    private GameObject SelectOBJ;

    public override void OnRegister()
    {
        proxy = Facade.RetrieveProxy(MyProxyName.PanelSettingProxy) as PanelSettingProxy;

        EventDispatcher.AddEventListener(EventDefine.Event_Button_A, OnButtonA);
        EventDispatcher.AddEventListener(EventDefine.Event_Button_B, OnButtionB);

        _rate       = proxy.Rate;
        _language   = proxy.Language;
        _volume     = proxy.Volume;
        _clear      = 0;
        _ticket     = 0;
        proxy.InitInfo( SettingManager.Instance.GameRate, SettingManager.Instance.GameLanguage, 
                        SettingManager.Instance.GameVolume, SettingManager.Instance.GameTicket, SettingManager.Instance.PlayTime);

        SelectOBJ = GameObject.Find("SelectBK").gameObject;
        SelectOBJ.SetActive(false);

        for (int i = 0; i < SettingConfig.Tickets.Length; ++i )
        {
            if (SettingManager.Instance.GameTicket == SettingConfig.Tickets[i])
            {
                _ticket = i;
                break;
            }
        }
    }

    public override void OnRemove()
    {
        EventDispatcher.RemoveEventListener(EventDefine.Event_Button_A, OnButtonA);
        EventDispatcher.RemoveEventListener(EventDefine.Event_Button_B, OnButtionB);

        SelectOBJ.SetActive(true);
    }

    private int comdSwitch;      //指示按钮A和按钮B功能切换
    private int _rate;           //设置每次消耗游戏币数
    private int _volume;         //设置音量
    private int _language;       //设置语言
    private int _clear;
    private int _ticket;
    private int _timeIndex;

    private SettingConfig.ComdOfButA curButA;

    private SettingConfig.Page curPage; //当前选中页
    private SettingConfig.HomePageItem curHomeItem;//

    #region Private Function
    /// <summary>
    /// 处理后台A按钮操作
    /// </summary>
    private void OnButtonA()
    {
        switch (curPage)
        {
            case SettingConfig.Page.HomePage:
                OPHomePage_A();
                break;
            case SettingConfig.Page.SettingPage:
                OPSetPage_A();
                break;
            case SettingConfig.Page.AccountPage:
                OPAccountPage_A();
                break;
            case SettingConfig.Page.BusinessRecord_Page:
                curPage = SettingConfig.Page.AccountPage;
                ui.BusinessToAccount();
                break;
            case SettingConfig.Page.DataReset_Page:
                SettingConfig.DataResetItem dataresetitem = ui.FindDataResetItem();
                if (dataresetitem == SettingConfig.DataResetItem.Yes)
                    proxy.ClearAccount();
                curPage = SettingConfig.Page.AccountPage;
                ui.DataResetToAccount();
                break;
            case SettingConfig.Page.TotalRecord_Page:
                curPage = SettingConfig.Page.AccountPage;
                ui.TotalRecordToAccount();
                break;
        }
    }

    /// <summary>
    /// 处理后台B按钮
    /// </summary>
    private void OnButtionB()
    {

        if (curButA == SettingConfig.ComdOfButA.Sure)
        {
            switch (curPage)
            {
                case SettingConfig.Page.HomePage:
                    ui.SwitchHomeItem();
                    break;
                case SettingConfig.Page.SettingPage:
                    ui.SwitchSetItem();                    
                    break;
                case SettingConfig.Page.AccountPage:
                    ui.SwitchAccountItem();
                    break;
                case SettingConfig.Page.DataReset_Page:
                    ui.SwitchDataRestItem();
                    break;
            }
            return;
        }

        if (curPage == SettingConfig.Page.SettingPage)
        {
            SettingConfig.SettingPageItem setitem = ui.FindSettingItem();
            switch (setitem)
            {
                case SettingConfig.SettingPageItem.Money:
                    _rate = proxy.CorrectRate(++_rate);
                    ui.SwitchRate(_rate);
                    break;
                case SettingConfig.SettingPageItem.Volume:
                    _volume = proxy.CorretVolume(++_volume);
                    ui.SwitchVolume(_volume);
                    break;
                case SettingConfig.SettingPageItem.Language:
                    _language = proxy.CorrectLanguage(_language);
                    ui.SwitchLanguage(_language);
                    break;
                case SettingConfig.SettingPageItem.ClearCoin:
                    _clear = proxy.CorrectClear(_clear);
                    ui.SwitchClear(_clear);
                    break;
                case SettingConfig.SettingPageItem.Time:
                    _timeIndex = proxy.CorretTimeIndex(++_timeIndex);
                    ui.SwitchTime(proxy.Time);
                    break;
                case SettingConfig.SettingPageItem.Ticket:
                    _ticket = proxy.CorrectTicket(++_ticket);
                    ui.SwitchTicket(SettingConfig.Tickets[_ticket]);
                    break;
            }
        }
    }

    /// <summary>
    /// HomePage A 擦做
    /// </summary>
    private void OPHomePage_A()
    {
        SettingConfig.HomePageItem homeitem = ui.FindHomeItem();
        switch (homeitem)
        {
            case SettingConfig.HomePageItem.Set:
                comdSwitch = 0;
                _clear = 1;
                curPage = SettingConfig.Page.SettingPage;
                ui.HomeToSet();
                curButA = SettingConfig.ComdOfButA.Sure;
                ui.SetSetInfo(proxy.Rate, proxy.Language, proxy.Volume, proxy.Ticket, proxy.Time);
                break;
            case SettingConfig.HomePageItem.Account:
                curPage = SettingConfig.Page.AccountPage;
                ui.HomeToAccount();
                break;
            case SettingConfig.HomePageItem.Exit:
                SettingManager.Instance.Save();
                UIManager.Instance.CloseUI(EnumUIType.PanelSetting);
                UIManager.Instance.OpenUI(EnumUIType.PanelCoin);
                //IOManager.Instance.SendMessageExitSetting();
                break;
        }
    }

    /// <summary>
    /// SetPage A操作
    /// </summary>
    private void OPSetPage_A()
    {
        ++comdSwitch;
        comdSwitch %= 2;
        SettingConfig.SettingPageItem setitem = ui.FindSettingItem();
        switch (setitem)
        {
            case SettingConfig.SettingPageItem.Money:
            case SettingConfig.SettingPageItem.Volume:
            case SettingConfig.SettingPageItem.Language:
            case SettingConfig.SettingPageItem.Ticket:
            case SettingConfig.SettingPageItem.Time:
                curButA = comdSwitch == 1 ? SettingConfig.ComdOfButA.Enter : SettingConfig.ComdOfButA.Sure;
                ui.OPSet(comdSwitch, (int)setitem);
                break;
            case SettingConfig.SettingPageItem.ClearCoin:
                curButA = comdSwitch == 1 ? SettingConfig.ComdOfButA.Enter : SettingConfig.ComdOfButA.Sure;
                ui.OPSet(comdSwitch, (int)setitem);
                ui.OpClearCoin(comdSwitch);
                break;
            case SettingConfig.SettingPageItem.Exit:
                curPage = SettingConfig.Page.HomePage;
                ui.SetToHome();
                break;
        }

        if (curButA == SettingConfig.ComdOfButA.Sure)
        {
            switch (setitem)
            {
                case SettingConfig.SettingPageItem.Money:
                    proxy.ChangeRate(_rate);
                    break;
                case SettingConfig.SettingPageItem.Volume:
                    proxy.ChangeVolume(_volume);
                    break;
                case SettingConfig.SettingPageItem.Language:
                    proxy.ChangeLaguage(_language);
                    break;
                case SettingConfig.SettingPageItem.ClearCoin:
                    proxy.ClearCoin(_clear);
                    break;
                case SettingConfig.SettingPageItem.Time:
                    proxy.ChangeTime(_timeIndex);
                    break;
                case SettingConfig.SettingPageItem.Ticket:
                    proxy.ChangeTicket(SettingConfig.Tickets[_ticket]);
                    break;
            }
        }
    }

    /// <summary>
    /// 查账页面 A操作
    /// </summary>
    private void OPAccountPage_A()
    {
        SettingConfig.AccountPageItem accountitem = ui.FindAccountItem();
        switch (accountitem)
        {
            case SettingConfig.AccountPageItem.BusinessRecords:
                proxy.UpdateMonthData(SettingManager.Instance.GetMonthData());
                break;
            case SettingConfig.AccountPageItem.TotalRecords:
                proxy.UpdateTotalData(SettingManager.Instance.TotalRecord());
                break;
            case SettingConfig.AccountPageItem.DataReset:
                curPage = SettingConfig.Page.DataReset_Page;
                ui.AccountToDataReset();
                break;
            case SettingConfig.AccountPageItem.Exit:
                curPage = SettingConfig.Page.HomePage;
                ui.AccountToHome();
                break;
        }
    }

    #endregion


}
