/*
 * Copyright (c) 
 * 
 * 文件名称：   SettingLogic.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2016/9/4 10:47:18
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Need.Mx;

public class PanelSettingLogic : BaseUI
{
    public override EnumUIType GetUIType()
    {
        return EnumUIType.PanelSetting;
    }

    public override void OnRegesterMediatorPlug()
    {
        MediatorManager.Instance.RegesterMediatorPlug(GetUIType(), gameObject, MyMediatorName.PanelSettingMediator);
    }

    protected PanelSettingView _View;
    private SettingConfig.HomePageItem curHomeItem;//
    private SettingConfig.SettingPageItem curSettingItem;
    private SettingConfig.AccountPageItem curAccountItem;
    private SettingConfig.DataResetItem curDataResetItem;

    //  Home_Page主页
    private int total_home_item;
    private int cur_home_item;
    // Setting_Page设置页
    private int total_set_item;
    private int cur_set_item;

    //Account_Page 查账页
    private int total_account_item;
    private int cur_account_item;
    
    //DataReset_Page数据清零页
    private int total_rest_item;
    private int cur_rest_item;


    protected override void OnStart()
    {
        _View = new PanelSettingView();
        _View.Init(transform);
        total_home_item     = _View.homePage.homeToggleList.Count;
        total_set_item      = _View.settingPage.settingToggleList.Count;
        total_account_item  = _View.accountPage.accountToggleList.Count;
        total_rest_item     = _View.dataRestPage.datarestToggleList.Count;

    }

    protected override void OnRelease()
    {
        MediatorManager.Instance.RemoveMediatorPlug(GetUIType());
    }


    #region Public Function
    /// <summary>
    /// 设置后台语言
    /// </summary>
    /// <param name="language"></param>
    public void SetLanguage(int language)
    {
        //Home_Page
        {
            for (int i = 0; i < _View.homePage.homeTextList.Count; i++)
                _View.homePage.homeTextList[i].text = SettingConfig.homeName[language, i];
        }
        //Setting_Page
        {
            for (int i = 0; i < _View.settingPage.settingTextList.Count; i++)
                _View.settingPage.settingTextList[i].text = SettingConfig.setName[language, i];
            _View.settingPage.Text_title.text = SettingConfig.setName[language, _View.settingPage.settingTextList.Count];
            if (language == 0)
            {
                for (int i = 0; i < _View.settingPage.Text_clear.Length; i++)
                    _View.settingPage.Text_clear[i].text = SettingConfig.clearcoin[0, i];
            }
            else
            {
                for (int i = 0; i < _View.settingPage.Text_clear.Length; i++)
                    _View.settingPage.Text_clear[i].text = SettingConfig.clearcoin[1, i];
            }
        }
        //Account_Page
        {
            for (int i = 0; i < _View.accountPage.accountTextList.Count; i++)
                _View.accountPage.accountTextList[i].text = SettingConfig.accountName[language, i];
            _View.accountPage.Text_title.text = SettingConfig.accountName[language, _View.accountPage.accountTextList.Count];
        }
        //Business_Page
        {
            for (int i = 0; i < _View.businnessPage.titleList.Count; i++)
                _View.businnessPage.titleList[i].text = SettingConfig.businessRecordName[language, i];
        }
        //DataRest_Page
        {
            for (int i = 0; i < _View.dataRestPage.datarestTextList.Count; i++)
                _View.dataRestPage.datarestTextList[i].text = SettingConfig.dataResetName[language, i];
            _View.dataRestPage.Text_title.text = SettingConfig.dataResetName[language, _View.dataRestPage.datarestTextList.Count];
        }
        //Total_Page
        {
            for (int i = 0; i < _View.totalPage.titleList.Count; i++)
                _View.totalPage.titleList[i].text = SettingConfig.totalRecordName[language, i];
        }
    }

    public SettingConfig.HomePageItem FindHomeItem()
    {
        return curHomeItem;
    }

    public SettingConfig.SettingPageItem FindSettingItem()
    {
        return curSettingItem;
    }

    public SettingConfig.AccountPageItem FindAccountItem()
    {
        return curAccountItem;
    }

    public SettingConfig.DataResetItem FindDataResetItem()
    {
        return curDataResetItem;
    }

    

    #region--------------------------页面切换------------------------
    /// <summary>
    /// 切换到设置页
    /// </summary>
    public void HomeToSet()
    {
        _View.Home_Page.SetActive(false);
        _View.Setting_Page.SetActive(true);
    }

    /// <summary>
    /// 切换到查账页
    /// </summary>
    public void HomeToAccount()
    {
        _View.Home_Page.SetActive(false);
        _View.Account_Page.SetActive(true);
    }

    /// <summary>
    /// 查账主页面到营业记录
    /// </summary>
    public void AccountToBusiness()
    {
        _View.BusinessRecords_Page.SetActive(true);
        _View.Account_Page.SetActive(false);
    }

    /// <summary>
    /// 查账主页面到总记录
    /// </summary>
    public void AccountToTotal()
    {
        _View.TotalRecord_Page.SetActive(true);
        _View.Account_Page.SetActive(false);
    }

    /// <summary>
    /// 由设置页到主页面
    /// </summary>
    public void SetToHome()
    {
        _View.Setting_Page.SetActive(false);
        _View.Home_Page.SetActive(true);
    }


    /// <summary>
    /// 数据重置到查账主页面
    /// </summary>
    public void AccountToDataReset()
    {
        _View.Account_Page.SetActive(false);
        _View.DataReset_Page.SetActive(true);
    }

    /// <summary>
    /// 查账页面到后台主页面
    /// </summary>
    public void AccountToHome()
    {
        _View.Account_Page.SetActive(false);
        _View.Home_Page.SetActive(true);
    }

    /// <summary>
    /// 营业记录页面到查账页面
    /// </summary>
    public void BusinessToAccount()
    {
        _View.BusinessRecords_Page.SetActive(false);
        _View.Account_Page.SetActive(true);
    }

    /// <summary>
    /// 总记录页面到查账页面
    /// </summary>
    public void TotalRecordToAccount()
    {
        _View.TotalRecord_Page.SetActive(false);
        _View.Account_Page.SetActive(true);
    }

    /// <summary>
    /// 数据重置到查账
    /// </summary>
    public void DataResetToAccount()
    {
        _View.DataReset_Page.SetActive(false);
        _View.Account_Page.SetActive(true);
    }

    #endregion
    /// <summary>
    /// 后台设置页面初始化
    /// </summary>
    /// <param name="rate"></param>
    /// <param name="language"></param>
    /// <param name="volume"></param>
    public void SetSetInfo(int rate, int language, int volume, int ticket, int time)
    {
        SetRateSelect(rate);
        SetLanguageSelect(language);
        SetVolumeSelect(volume);
        SetTicketSelect(ticket);
        SetTimeSelect(time);
    }

    /// <summary>
    /// 设置当前选中币率
    /// </summary>
    /// <param name="rate"></param>
    public void SetRateSelect(int rate)
    {
        _View.settingPage.Text_Rate.text = rate.ToString();
    }

    /// <summary>
    /// 设置当前选中的出票分数
    /// </summary>
    /// <param name="ticket"></param>
    public void SetTicketSelect(int ticket)
    {
        _View.settingPage.Text_Ticket.text = ticket.ToString();
    }

    /// <summary>
    /// 设置当前的开始游戏时间
    /// </summary>
    /// <param name="time"></param>
    public void SetTimeSelect(int time)
    {
        _View.settingPage.Text_Time.text = time.ToString();
    }

    /// <summary>
    /// 设置当前选中语言
    /// </summary>
    /// <param name="language"></param>
    public void SetLanguageSelect(int language)
    {
        for (int i = 0; i < _View.settingPage.image_language.Length; i++)
        {
            if (language == i)
                _View.settingPage.image_language[i].color = new Color(1, 0, 0);
            else
                _View.settingPage.image_language[i].color = new Color(1, 1, 1);
        }
    }

    /// <summary>
    ///  设置当前选中音量
    /// </summary>
    public void SetVolumeSelect(int volume)
    {
        _View.settingPage.slider_volume.value = (float)volume / SettingConfig.volume.Length;
        _View.settingPage.Text_volume.text = volume.ToString();
    }

    /// <summary>
    /// 设置查账信息
    /// </summary>
    public void SetBusinessPageInfo(int language, List<float[]> monthData)
    {
        for (int i = 0; i < monthData.Count; i++)
        {
            _View.businnessPage.monthList[i].text = monthData[i][0].ToString();
            _View.businnessPage.coinsmonthList[i].text = monthData[i][1].ToString();
            _View.businnessPage.gametimemonthList[i].text = HelperTool.DoTimeFormat(language, (int)monthData[i][2]);
            _View.businnessPage.uptimemonthList[i].text = HelperTool.DoTimeFormat(language, (int)monthData[i][3]);
        }

        for (int i = 0; i < _View.businnessPage.titleList.Count; i++)
            _View.businnessPage.titleList[i].text = SettingConfig.businessRecordName[language, i];
    }

    /// <summary>
    /// 设置总记录信息
    /// </summary>
    public void SetTotalRecordInfo(int language, float[] total)
    {
        for (int i = 0; i < total.Length; i++)
        {
            if (i == 1 || i == 2)
                _View.totalPage.valueList[i].text = HelperTool.DoTimeFormat(language, (int)SettingManager.Instance.TotalRecord()[i]);
            else
                _View.totalPage.valueList[i].text = SettingManager.Instance.TotalRecord()[i].ToString();
        }

        for (int i = 0; i < _View.totalPage.titleList.Count; i++)
            _View.totalPage.titleList[i].text = SettingConfig.totalRecordName[language, i].ToString();
    }

   
    /// <summary>
    /// 设置页调整数据操作
    /// </summary>
    public void OPSet(int cmd, int index)
    {
        if (cmd == 1)
            _View.settingPage.settingImageList[index].color = new Color(1, 0, 0);
        else
            _View.settingPage.settingImageList[index].color = new Color(1, 1, 1);
    }
  
    /// <summary>
    /// 设置也清除投币操作
    /// </summary>
    /// <param name="cmd"></param>
    public void OpClearCoin(int cmd)
    {
        if (cmd == 1)
            _View.settingPage.Text_clear[cmd].color = new Color(1, 0, 0);
        else
            _View.settingPage.Text_clear[cmd].color = new Color(1, 1, 1);
    }

    #region OPB
    /// <summary>
    /// 后台主页面选项切换
    /// </summary>
    public void SwitchHomeItem()
    {
        ++cur_home_item;
        cur_home_item %= total_home_item;
        int temp = 0;
        for (int i = 0; i < _View.homePage.homeToggleList.Count; i++)
        {
            if (cur_home_item == temp)
            {
                _View.homePage.homeToggleList[i].isOn = true;
                _View.homePage.homeTextList[i].color = new Color(1, 0, 0);
                curHomeItem = (SettingConfig.HomePageItem)cur_home_item;
            }
            else
            {
                _View.homePage.homeToggleList[i].isOn = false;
                _View.homePage.homeTextList[i].color = new Color(1, 1, 1);
            }
            ++temp;
        }
    }

    /// <summary>
    /// 切换Set选项
    /// </summary>
    public void SwitchSetItem()
    {
        ++cur_set_item;
        cur_set_item %= total_set_item;
        int temp = 0;

        for (int i = 0; i < _View.settingPage.settingToggleList.Count; i++)
        {
            if (cur_set_item == temp)
            {
                _View.settingPage.settingToggleList[i].isOn = true;
                _View.settingPage.settingTextList[i].color = new Color(1, 0, 0);
                curSettingItem = (SettingConfig.SettingPageItem)cur_set_item;
            }
            else
            {
                _View.settingPage.settingToggleList[i].isOn = false;
                _View.settingPage.settingTextList[i].color = new Color(1, 1, 1);
            }
            ++temp;
        }
    }


    /// <summary>
    /// 选择币率
    /// </summary>
    /// <param name="rate"></param>
    public void SwitchRate(int rate)
    {
        _View.settingPage.Text_Rate.text = rate.ToString();
    }

    /// <summary>
    /// 选择音量
    /// </summary>
    /// <param name="volume"></param>
    public void SwitchVolume(int volume)
    {
        _View.settingPage.Text_volume.text = volume.ToString();
        _View.settingPage.slider_volume.value = (float)volume / SettingConfig.volume.Length;
    }

    public void SwitchTime(int time)
    {
        _View.settingPage.Text_Time.text = time.ToString();
    }

    /// <summary>
    /// 修改出票分数
    /// </summary>
    /// <param name="ticket"></param>
    public void SwitchTicket(int ticket)
    {
        _View.settingPage.Text_Ticket.text = ticket.ToString();
    }

    /// <summary>
    /// 选择语言
    /// </summary>
    /// <param name="language"></param>
    public void SwitchLanguage(int language)
    {
        int temp = 0;
        for (int i = 0; i < _View.settingPage.image_language.Length; i++)
        {
            if (language == temp)
                _View.settingPage.image_language[i].color = new Color(1, 0, 0);
            else
                _View.settingPage.image_language[i].color = new Color(1, 1, 1);
            ++temp;
        }
    }

    /// <summary>
    /// 选择清除投币
    /// </summary>
    /// <param name="clear"></param>
    public void SwitchClear(int clear)
    {
        int temp = 0;
        for (int i = 0; i < _View.settingPage.Text_clear.Length; i++)
        {
            if (clear == temp)
                _View.settingPage.Text_clear[i].color = new Color(1, 0, 0);
            else
                _View.settingPage.Text_clear[i].color = new Color(1, 1, 1);
            ++temp;
        }
    }

   
    /// <summary>
    /// 查账主页面选项切换
    /// </summary>
    public void SwitchAccountItem()
    {
        ++cur_account_item;
        cur_account_item %= total_account_item;
        int temp = 0;
        foreach (Toggle tg in _View.accountPage.accountToggleList)
        {
            if (cur_account_item == temp)
            {
                tg.isOn = true;
                tg.transform.Find("Label").GetComponent<Text>().color = new Color(1, 0, 0);
                curAccountItem = (SettingConfig.AccountPageItem)cur_account_item;
            }
            else
            {
                tg.isOn = false;
                tg.transform.Find("Label").GetComponent<Text>().color = new Color(1, 1, 1);
            }
            ++temp;
        }
    }

    /// <summary>
    /// 查账数据清零选项切换
    /// </summary>
    public void SwitchDataRestItem()
    {
        ++cur_rest_item;
        cur_rest_item %= total_rest_item;
        int temp = 0;
        foreach (Toggle tg in _View.dataRestPage.datarestToggleList)
        {
            if (cur_rest_item == temp)
            {
                tg.isOn = true;
                tg.transform.Find("Label").GetComponent<Text>().color = new Color(1, 0, 0);
                curDataResetItem = (SettingConfig.DataResetItem)cur_rest_item;
            }
            else
            {
                tg.isOn = false;
                tg.transform.Find("Label").GetComponent<Text>().color = new Color(1, 1, 1);
            }
            ++temp;
        }
    }

    #endregion

    #endregion
}
