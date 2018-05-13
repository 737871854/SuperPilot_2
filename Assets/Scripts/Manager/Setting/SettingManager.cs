/*
 * Copyright (c) 
 * 
 * 文件名称：   SettingManager.cs
 * 
 * 简    介:    SettingManager数据管理
 * 
 * 创建标识：   Pancake 2016/7/28 17:54:50
 * 
 * 修改描述：   2016.9.3 PlayerPrefs转Json
 * 
 */

using System;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Need.Mx;
using JsonFx.Json;

public class SettingConfigData
{
    // 校验ID
    public string CheckId { get; set; }

    // 0,1,2,3,4,5,6币率
    public int GameRate { get; set; }

    // 0代表中文，1代表英文。
    public int GameLanguage { get; set; }

    // 0 代表模式1,1 代表模式2
    public int Ticket { get; set; }

    // 当前音量，分为10个等级
    public int GameVolume { get; set; }

    // 游戏初始时间
    public int PlayTime { get; set; }

    // 月份信息
    public List<float[]> MonthList = new List<float[]>();

    // 总记录 游戏次数，游戏时间，开机时间，总币数，总出票数
    public float[] TotalRecord { get; set; }

    // 玩家剩余币数
    public int[] Coin { get; set; }

    public SettingConfigData()
    {
        this.CheckId = string.Empty;
        this.GameRate = 3;
        this.GameLanguage = 0;
        this.Ticket = 200;
        this.GameVolume = 5;
        this.PlayTime = 30;
        for (int i = 0; i < 3; i++)
        {
            this.MonthList.Add(new float[4]);
        }

        DateTime now = DateTime.Now;
        int month = now.Month;
        this.MonthList[0][0] = month;


        if (month - 1 <= 0)
        {
            this.MonthList[1][0] = 11 + month;
            this.MonthList[2][0] = 10 + month;
        }
        else if (month - 2 <= 0)
        {
            this.MonthList[1][0] = 10 + month;
            this.MonthList[2][0] = 9 + month;
        }
        else
        {
            this.MonthList[1][0] = month - 1;
            this.MonthList[2][0] = month - 2;
        }


        this.TotalRecord = new float[4];
        this.Coin = new int[3];
    }

}

public class SettingManager
{
    private static readonly object _obj = new object();
    private static SettingManager _instance;
    public static SettingManager Instance
    {
        get
        {
            if (null == _instance)
            {
                lock (_obj)
                {
                    if (null == _instance)
                    {
                        _instance = new SettingManager();
                    }
                }
            }
            return _instance;
        }
    }


    private SettingConfigData po;

    private string _checkID         = string.Empty;
    private int _gameRate           = 3;                                // 游戏币率
    private int _gameVolume         = 10;                               // 游戏音量
    private int _gameLanguage       = 0;                                // 游戏语言
    private int _ticket             = 0;                                // 出票模式
    private int _playTime           = 30;                               // 游戏时间

    private int[] _hasCoin;                                             // 玩家剩余币数

    private List<float[]> _monthList = new List<float[]>();             // 月份信息

    private float[] _totalRecord;                                       // 总记录

    private int _gameTime;  // 游戏时间
    private int _upTime;    // 开机时间
    private int _gameCount; // 游戏次数
    private int _coin;      // 耗币
    private int _addCoin;   // 投币

    private bool _isIdle;   // 待机


    public void Init()
    {
        // 取出后台存储所有数据
        this.po = HelperTool.LoadJson(Application.streamingAssetsPath + "/SettingConfig.json");
        if (null == this.po)
        {
            this.po = new SettingConfigData();
        }
        
        // CheckID
        this._checkID = po.CheckId;
    
        // 游戏币率
        this._gameRate = po.GameRate;

        // 游戏语言版本 0中文 1英文
        this._gameLanguage = po.GameLanguage;

        // 检查点模式
        this._ticket = po.Ticket;

        // 游戏初始时间
        this._playTime = po.PlayTime;

        // 游戏音量
        this._gameVolume = po.GameVolume;

        // 当前剩余币数
        this._hasCoin = po.Coin;

        // 月份信息
        this._monthList = po.MonthList;

        // 总记录
        this._totalRecord = po.TotalRecord;

        CheckIsNewMonth();

        //GameConfig.ParsingGameConfig();
    }
      
   
    #region  -----------------后台提供借口-----------------------begin------------------
  
    // 待机
    public bool Idle
    {
        set { _isIdle = value; }
    }

    // 校验ID
    public string CheckID
    {
        get { return _checkID; }
        set { _checkID = value; }
    }

    //获取游戏币率
    public int GameRate
    {
        get { return _gameRate; }
        set { _gameRate = value;}
    }
    //获取游戏音量
    public int GameVolume
    {
        get 
        {
            if (!_isIdle)
                return _gameVolume;
            else
                return Mathf.FloorToInt(_gameVolume * 0.5f);
        }
        set { _gameVolume = value; }
    }
 
    //获取游戏语言
    public int GameLanguage
    {
        get { return _gameLanguage; }
        set { _gameLanguage = value; }
    }


    //获取1票需要多少分或出票模式
    public int GameTicket
    {
        get { return _ticket; }
        set { _ticket = value; }
    }

    // 游戏初始时间
    public int PlayTime
    {
        get { return _playTime; }
        set { _playTime = value; }
    }
    
    // 总记录
    public float[] TotalRecord()
    {
        return this._totalRecord;
    }

    #endregion         -----------------后台提供借口-----------------end------------------------

    public int HasCoin(int index)
    {
        return _hasCoin[index];
    }

    // 修改po的Coin值
    public void ClearCoin()
    {
        for (int i = 0; i < 1; ++i )
        {
            this._hasCoin[i] = 0;
        }
    }

    // 清除月份信息
    public void ClearMonthInfo()
    {
        for (int i = 0; i < this._monthList.Count; i++ )
        {
            for (int j = 1; j < this._monthList[i].Length; j++ )
            {
                this._monthList[i][j] = 0;
            }
        }
    }

    // 清除总记录
    public void ClearTotalRecord()
    {
        for (int i = 0; i < this._totalRecord.Length; i++ )
        {
            this._totalRecord[i] = 0;
        }
    }


    public List<float[]> GetMonthData()
    {
        return this._monthList;
    }

    // 获取指定月份信息
    public float[] GetMonthData(int index)
    {
        return this._monthList[index];
    }

    // 游戏时间
    public void LogGameTimes(int value)
    {
        this._gameTime += value;
    }

    // 开机时间
    public void LogUpTime(int value)
    {
        this._upTime += value;
    }

    // 耗币
    public void LogCoins(int value)
    {
        this._coin += value;
    }

    // 增加游戏次数
    public void LogNumberOfGame(int value)
    {
        this._gameCount += value;
    }

    // 投币
    public void AddCoin()
    {
        if (this._hasCoin[0] >= 999)
            return;
        this._addCoin += 1;
        this._hasCoin[0] += 1;
        LogCoins(1);
        ioo.audioManager.PlaySound2D("SFX_Sound_Insert_Coin");
    }

    public void UseCoin()
    {
        this._hasCoin[0] -= _gameRate;
        LogGameCount();
    }

    // 游戏次数
    public void LogGameCount()
    {
        this._gameCount += 1;
    }

   //----------------------------------------------

    // 保存后台信息
    public void Save()
    {
        CopyToPo();
        HelperTool.SaveJson(this.po, Application.streamingAssetsPath + "/SettingConfig.json");
    }

    public void CopyToPo()
    {
        // 修改当期那月份信息
        {
            this._monthList[0][1] += this._coin;
            this._monthList[0][2] += this._gameTime;
            this._monthList[0][3] += this._upTime;
        }

        // 修改总记录
        {
            this._totalRecord[0] += this._gameCount;
            this._totalRecord[1] += this._gameTime;
            this._totalRecord[2] += this._upTime;
            this._totalRecord[3] += this._addCoin;
        }

        // 临时数据清除
        {
            this._gameCount = 0;
            this._gameTime = 0;
            this._upTime = 0;
            this._coin = 0;
            this._addCoin = 0;
        }

        // 自改po的值
        {
            this.po.CheckId         = this._checkID;
            this.po.GameRate        = this._gameRate;
            this.po.GameLanguage    = this._gameLanguage;
            this.po.Ticket          = this._ticket;
            this.po.GameVolume      = this._gameVolume;
            this.po.PlayTime        = this._playTime;
            this.po.MonthList       = this._monthList;
            this.po.Coin            = this._hasCoin;

            this.po.TotalRecord = this._totalRecord;
        }
    }

    private void CheckIsNewMonth()
    {
        DateTime now = DateTime.Now;
        int month = now.Month;
        bool isNewMonth = true;
        for (int i = 0; i < this._monthList.Count; i++ )
        {
            if (month == this._monthList[i][0])
            {
                isNewMonth = false;
                break;
            }
        }

        if (!isNewMonth)
        {
            return;
        }

        for (int i = 0; i < this._monthList[1].Length; i++)
        {
            this._monthList[2][i] = this._monthList[1][i];
        }

        for (int i = 0; i < this._monthList[1].Length; i++)
        {
            this._monthList[1][i] = this._monthList[0][i];
        }
        
        for (int i = 0; i < this._monthList[0].Length; i++ )
        {
            this._monthList[0][i] = 0;
        }
        this._monthList[0][0] = month;
    }
  
}
