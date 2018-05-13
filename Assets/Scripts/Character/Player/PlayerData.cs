/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   PlayerData.cs
 * 
 * 简    介:    玩家临时数据
 * 
 * 创建标识：   Pancake 2017/5/27 10:54:29
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using Need.Mx;

public class PlayerData : ActorBase
{
    public PlayerData()
    {
        _life = Const.GAME_CONFIG_TIME;
        _condition = 100;
        _gather = 0;
    }

    // 玩家获得金币个数
    private int _coin;
    // 玩家获得魔方个数
    private int _fix;
    // 玩家获得油桶个数
    private int _fuel;
    // 玩家获得钻石个数
    private int _diamond;
    // 玩家击落蝙蝠个数
    private int _bat;
    // 玩家击落FeiQi个数
    private int _feiQi;
    // 玩家获得护盾个数
    private int _shield;
    // 玩家获得磁铁个数
    private int _magnet;
    private int _wall;

    private float _totalTime;

    private float _hurry;

    private float _hot;

    // 机身完好程度（100完好无损）
    private float _condition;

    private float _gather;

    public int Coin { get { return _coin; } }
    public int Fix { get { return _fix; } }
    public int Fuel { get { return _fuel; } }
    public int Diamond { get { return _diamond; } }
    public int Bat { get { return _bat; } }
    public int FeiQi { get { return _feiQi; } }
    public int Shield { get { return _shield; } }
    public int Magnet { get { return _magnet; } }
    public float LifeTime { get { return _life; } set { _life = value; } }
    public int Score { get { return _score; } }
    public int ResultScore { get { return _resulScore = _score + _bossScore - _damageScore; } }
    public int BossScore { get { return _bossScore; } }
    public int DamageScore { get { return _damageScore; } }
    public float Gather { get { return _gather; } set { _gather = value; } }
    public float TotalTime { get { return _totalTime; } }
    public float ConditionRate 
    { 
        get 
        {
            float ret = 0;
            if (_condition >= 75)
                ret = 1;
            else if (_condition < 75 && _condition >= 25)
                ret = 0.85f;
            else
                ret = 0.7f;

            return ret;
        } 
    }
    public float Hot 
    {
        get { return _hot; } 
        set 
        { 
            _hot = value;
            _hot = _hot < 0 ? 0 : _hot;
            _hot = _hot > 30 ? 30 : _hot;
        }
    }

    public float Hurry 
    { 
        get { return _hurry; } 
        set 
        { 
            _hurry = value; 
            _hurry = _hurry < 0 ? 0 : _hurry; 
            _hurry = _hurry > 20 ? 20 : _hurry; 
        } 
    }

    public void AddCoin(int value)
    {
        ++_coin;
        _score += value;
    }

    public void AddFix(int value)
    {
        ++_fix;
        _score += value;
        FixPlane();
    }

    public void AddFuel(int value)
    {
        ++_feiQi;
        _score += value;
    }

    public void AddDiamond(int value)
    {
        ++_diamond;
        _score += value;
    }

    public void AddBat(int value)
    {
        ++_bat;
        //_score += value;
    }

    public void AddFeiQi(int value)
    {
        ++_feiQi;
        //_score += value;
    }

    public void AddWall(int value)
    {
        ++_wall;
        //_score += value;
    }

    public void AddShield(int value)
    {
        ++_shield;
        _score += value;
    }

    public void AddMagnet(int value)
    {
        ++_magnet;
        _score += value;
    }

    public void AddBossScore(int value)
    {
        _bossScore += value;
    }

    // 简单处理 damage + 增加时间  - 减少时间
    public void OnDamage(float damage0, float damage1 = 0)
    {
        if (ioo.gameMode.State != GameState.Play)
            return;

        _life += damage0;
        _totalTime += damage0;

        if (_life < 0)
            _life = 0;

        if (damage0 < 0)
            DamagePlane(damage1);
    }

    public void AddLife(float value)
    {
        _life += value;
        _totalTime += value;
    }

    public void Summary()
    {
        _condition = 100;
    }

    #region 机身受损以及修复
    private void DamagePlane(float value)
    {
        float temp = _condition;
        if (value < 0)
        {
            if (_condition > 80)
            {
                if (temp + value <= 80)
                    ioo.audioManager.PlayPersonSound("Person_Sound_Injured");
            }else if (_condition > 0)
            {
                if (temp + value <= 0)
                    ioo.audioManager.PlayPersonSound("Person_Sound_Injured");
            }
        }

        _damageScore += (int)Mathf.Abs(value);
        _condition += value;
        if (_condition < 0)
            _condition = 0;
    }

    private void FixPlane()
    {
        _condition = 100;
        //_condition += value;
        //if (_condition > 100)
        //    _condition = 100;
    }
    #endregion
}
