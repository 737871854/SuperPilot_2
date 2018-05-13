/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   PanelSettingProxy.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/5/17 8:25:02
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;

public class PanelSettingProxy : PureMVC.Patterns.Proxy
{
    public const string NAME = MyProxyName.PanelSettingProxy;
    public const string UPDATED = MyProxyUpdated.PanelSetting_Update;
    public const string UPDATED_RATE = MyProxyUpdated.PanelSetting_UpdateRate;
    public const string UPDATED_LANGUAGE = MyProxyUpdated.PanelSetting_UpdateLanguage;
    public const string UPDATED_VOLUME = MyProxyUpdated.PanelSetting_UpdateVolume;
    public const string UPDATED_MONTH = MyProxyUpdated.PanelSetting_UpdateMonth;
    public const string UPDATED_TATAL = MyProxyUpdated.PanelSetting_UpdateTotal;
    public const string UPDATED_TICKET = MyProxyUpdated.PanelSetting_UpdateTicket;
    public const string UPDATAED_TIME = MyProxyUpdated.Panelsetting_UpdateTime;

    public PanelSettingProxy(string proxyName) : base(proxyName, null) { }

    private int MaxRate = 40;

    /// <summary>
    /// 币率
    /// </summary>
    private int m_rate;

    /// <summary>
    /// 音量
    /// </summary>
    private int m_volume;

    private int m_ticket;

    private int m_time;

    private int m_timeIndex;

    /// <summary>
    /// 语言
    /// </summary>
    private int m_language;

    private List<float[]> m_monthData = new List<float[]>();
    private float[] m_total;

    public int Rate { get { return m_rate; } }
    public int Language { get { return m_language; } }
    public int Volume { get { return m_volume; } }
    public int Ticket { get { return m_ticket; } }
    public int Time { get { return m_time; } }
    public int TimeIndex { get { return m_timeIndex; } }

    public List<float[]> MonthData { get { return m_monthData; } }
    public float[] Total { get { return m_total; } }

    public override void OnRegister()
    {
        base.OnRegister();
    }

    public override void OnRemove()
    {
        base.OnRemove();
    }
    #region 提供Mediator调用
    public void InitInfo(int rate, int language, int volume, int ticket, int time)
    {
        m_rate      = rate;
        m_language  = language;
        m_volume    = volume;
        m_ticket    = ticket;
        m_time      = time;

        for (int i = 0; i < SettingConfig.PlayTime.Length; ++i )
        {
            if (m_time == SettingConfig.PlayTime[i])
            {
                m_timeIndex = i;
                break;
            }
        }

        SendNotification(UPDATED);
    }

    /// <summary>
    /// 修改币率
    /// </summary>
    public void ChangeRate(int rate)
    {
        m_rate = rate;

        SettingManager.Instance.GameRate = m_rate;
        SettingManager.Instance.Save();

        SendNotification(UPDATED_RATE);
    }

    /// <summary>
    /// 修改语言
    /// </summary>
    public void ChangeLaguage(int language)
    {
        m_language = language;

        SettingManager.Instance.GameLanguage = m_language;
        SettingManager.Instance.Save();

        SendNotification(UPDATED_LANGUAGE);
    }

    /// <summary>
    /// 修改音量
    /// </summary>
    public void ChangeVolume(int volume)
    {
        m_volume = volume;

        SettingManager.Instance.GameVolume = m_volume;
        SettingManager.Instance.Save();

        SendNotification(UPDATED_VOLUME);
    }

    /// <summary>
    /// 修改游戏初始时间
    /// </summary>
    /// <param name="index"></param>
    public void ChangeTime(int index)
    {
        m_time = SettingConfig.PlayTime[index];
        SettingManager.Instance.PlayTime = m_time;
        SettingManager.Instance.Save();

        SendNotification(UPDATAED_TIME);        
    }
    
    /// <summary>
    /// 清除投币
    /// </summary>
    /// <param name="clear"></param>
    public void ClearCoin(int clear)
    {
        if (clear != 0)
            return;

        SettingManager.Instance.ClearCoin();
        SettingManager.Instance.Save();
    }

    public void ChangeTicket(int ticket)
    {
        m_ticket = ticket;

        SettingManager.Instance.GameTicket = m_ticket;
        SettingManager.Instance.Save();

        SendNotification(UPDATED_TICKET);
    }

    /// <summary>
    /// 更新月份信息
    /// </summary>
    /// <param name="data"></param>
    public void UpdateMonthData(List<float[]> data)
    {
        m_monthData = data;

        SendNotification(UPDATED_MONTH);
    }

    /// <summary>
    /// 更新总记录信息
    /// </summary>
    /// <param name="data"></param>
    public void UpdateTotalData(float[] data)
    {
        m_total = new float[data.Length];
        m_total = data;

        SendNotification(UPDATED_TATAL);
    }

    /// <summary>
    /// 清除查账数据
    /// </summary>
    public void ClearAccount()
    {
        SettingManager.Instance.ClearMonthInfo();
        SettingManager.Instance.ClearTotalRecord();
    }

    /// <summary>
    /// 矫正币率
    /// </summary>
    /// <returns></returns>
    public int CorrectRate(int value)
    {
        return value <= MaxRate ? value : 0;
    }

    /// <summary>
    /// 矫正音量
    /// </summary>
    /// <returns></returns>
    public int CorretVolume(int value)
    {
        return value <= 10 ? value : 0;
    }

    public int CorretTimeIndex(int value)
    {
        m_timeIndex = value % SettingConfig.PlayTime.Length; ;
        m_time = SettingConfig.PlayTime[m_timeIndex];
        return m_timeIndex;
    }

    public int CorrectTicket(int value)
    {
        if (value >= SettingConfig.Tickets.Length)
            value = 0;

        return value;
    }

    /// <summary>
    /// 矫正语言
    /// </summary>
    /// <returns></returns>
    public int CorrectLanguage(int value)
    {
        return value == 1 ? 0 : 1;
    }

    public int CorrectClear (int value)
    {
        return value == 1 ? 0 : 1;
    }
    #endregion
   
}


public class SettingConfig
{
    #region PageCard
    public enum Page
    {
        HomePage,
        SettingPage,
        AccountPage,
        BusinessRecord_Page,
        TotalRecord_Page,
        DataReset_Page,
    };

    public enum HomePageItem
    {
        Set = 0,
        Account = 1,
        Exit = 2
    };

    public enum SettingPageItem
    {
        Money       = 0,
        Volume      = 1,
        Language    = 2,
        ClearCoin   = 3,
        Ticket      = 4,
        Time        = 5,
        Exit        = 6,
    };

    public enum AccountPageItem
    {
        BusinessRecords = 0,
        TotalRecords = 1,
        DataReset = 2,
        Exit = 3
    }

    public enum DataResetItem
    {
        Yes = 0,
        No = 1
    }

    public enum ComdOfButA
    {
        Sure = 0,
        Enter = 1,
    };


    public struct MonthData
    {
        public int month;
        public int coins_month;
        public int tickets_month;
        public int gameTimes_month;
        public int upTimes_month;
    };

    #endregion

    #region ----------------------------------------------------------各个页面信息-------------------------------------------------------------
    public static int confitshootingcalibreation = 128;
    public static string[] screenes = { "GAME_CONFIG_SCREEN_WIDTH", "GAME_CONFIG_SCREEN_HEIGHT" };
    public static string isfirsetopengame = "GAME_CONFIG_IS_FIRST_OPEN";
    public static string numberofgame = "GAME_CONFIG_NUMBER_OF_GAME";
    public static string[] monthInfo = { "GAME_CONFIG_THIS_MONTH", "GAME_CONFIG_ONE_MONTH_AGO", "GAME_CONFIG_TWO_MONTH_AGO" };
    public static string[] settingpageInfo = { "GAME_CONFIG_PER_USE_COIN", "GAME_CONFIG_VOLUME", "GAME_CONFIG_DIFFICULTY",
                                                      "GAME_CONFIG_LANGUAGE","GAME_CONFIG_PER_TICKET_SCORE","GAME_CONFIG_WATER_SHOW" };
    public static string[] coins_month = { "GAME_CONFIG_THIS_MONTH_COINS", "GAME_CONFIG_ONE_MONTH_AGO_COINS", "GAME_CONFIG_TWO_AGO_COINS" };
    public static string[] tickets_month = { "GAME_CONFIG_THIS_MONTH_TICKETS", "GAME_CONFIG_ONE_MONTH_AGO_TICKETS", "GAME_CONFIG_TWO_MONTH_AGO_TICKETS" };
    public static string[] gametimes_month = { "GAME_CONFIG_THIS_MONTH_GAME_TIMES", "GAME_CONFIG_ONE_MONTH_AGO_GAME_TIMES", "GAME_CONFIG_TWO_MONTH_AGO_GAME_TIMES" };
    public static string[] uptimes_month = { "GAME_CONFIG_THIS_MONTH_UP_TIMES", "GAME_CONFIG_ONE_MONTH_AGO_UP_TIMES", "GAME_CONFIG_TOW_MONTH_AGO_UP_TIMES" };
    public static string[] englishmonthes = { "January", "February", "march", "April", "May", "June", "July", 
                                                       "August", "September", "October", "November", "December" };
    public static int[] money = { 1, 2, 3, 4, 5, 6, 0 };
    public static int[] level = { 1, 2, 3 };
    public static int[] volume = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
    public static string[] language = { "中文", "英文" };
    public static string[,] gameDifficulty = { { "简单", "普通", "困难" }, { "Easy", "Common", "Hard" } };
    public static string[,] clearcoin = { { "是", "否" }, { "Yes", "No" } };
    public static string[] watershow = { "NO", "OFF" };
    public static string[,] homeName = { { "    设定", "    查账", "    退出" }, { "  Set", "  Account", "  Exit" } };
    public static string[,] setName = { {"    游戏币率","    游戏音量","    游戏语言","	币数清零","    出票分值","    游戏时间","    退出","设定"},
                                                      {"    Coin","    Volume","    Language","    Clear Coin","    Ticket","    Game Time","    Exit","Set"} };
    public static string[,] calibrationName = { { "    玩家 1" ,  "    玩家 2" ,"    玩家 3","    退出","设定校准"},
                                                        {"    Player 1","    Player 2","    Player 3","    Exit","Shooting Calibration"}};
    public static string[,] accountName = { { "    营业记录", "    总记录", "    数据清零", "    退出", "    查账" },
                                                      { "    Business Records", "    Total Record", "    Data Rest", "    Exit", "    Account" } };
    public static string[,] businessRecordName = { {"营业记录", "最近三个月记录", "投币数", "游戏时间", "开机时间","月记录", "月记录", "月记录"},
                                                        {"Business Record","Records The Last Three Months","Coins","Game Time","Uptime","","",""}};
    public static string[,] totalRecordName = { { "总记录", "游戏次数：", "总游戏时间：", "总开机时间：", "总投币数："},
                                                        { "Total Records", "Game Times:", "Total Game Times:", "Uptime:", "Total Coins:"} };
    public static string[,] dataResetName = { { "    是", "    否", "数据清零？" }, { "    Yes", "    No", "Data Rest?" } };

    public static string[] playerCoin = { "GAME_COINFIG_PLEYER0_COIN", "GAME_COINFIG_PLEYER1_COIN", "GAME_COINFIG_PLEYER2_COIN" };

    public static int[,] CheckPoint = { { 2500, 4800, 5800 }, { 2800, 5100, 6100 }, { 3200, 5300, 6350 } };

    public static int[] PlayTime = { 30, 60, 90, 120, 150, 180 };

    public static int[] Tickets = { 200, 400, 600, 800, 1000, 0};
    #endregion  --------------------------------------------------------------------------------------------------------------------
}
