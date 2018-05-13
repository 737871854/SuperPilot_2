/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   GameMode.cs
 * 
 * 简    介:    玩家数据也放在这里（游戏结构简单，懒得写一个玩家管理类）
 * 
 * 创建标识：   Pancake 2017/5/9 14:41:30
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;
using Need.Mx;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameMode : MonoBehaviour
{
    private GameState _state = GameState.Coin;
    private E_Map _map = E_Map.City;
    private PilotType _type;
   
    // 无操作时间
    private float _idleTime;
    private bool _isIdle;
    // 选择的飞机名字
    private string _selectedPlayerName;

    private Player _player;

    private BossBase _boss;

    private int _times;

    private bool _canFire;

    public GameState State  { get { return _state; } }
    public E_Map Map        { get { return _map; } }
    public Player Player    { get { return _player; } }
    public BossBase Boss    { get { return _boss; } set { _boss = value; } }
    public string PlayerName{ get { return _selectedPlayerName; } }
    public int Times        { get { return _times; } }
    public bool CanFire     { get { return _canFire; } set { _canFire = value; } }

    public enum Enviroment
    {
        Normal,
        Forest,
        Volcano,
        Winter,
    }
    public Enviroment enviroment = Enviroment.Normal;

    #region Public Function
    public void Init()
    {
        _times      = 1;
        _idleTime   = 60;
        _isIdle     = false;
    }

    public void Normal()
    {
        _idleTime = 60;
        if (_isIdle)
        {
            _isIdle = false;
            SettingManager.Instance.Idle = false;
            //ioo.audioManager.Normal();
            EventDispatcher.TriggerEvent(EventDefine.Event_Idle_Movie, false);
        }
    }

    public void Reset()
    {
        Init();
        RunMode(GameState.Coin);
        ioo.scenesManager.LoadSceneDir(SceneName.Coin);
        UIManager.Instance.CloseUI(EnumUIType.PanelMain);
        UIManager.Instance.CloseUI(EnumUIType.PanelSummary);
        UIManager.Instance.OpenUI(EnumUIType.PanelCoin);
        ioo.audioManager.StopAll();
        PropsManager.Instance.End();
        PathEffect.Instance.StopPathEffect();
    }

    public void ChangeEnviroment(Enviroment envir)
    {
        enviroment = envir;
    }

    /// <summary>
    /// 场景切换完毕，在这里做一些必要的初始化工作
    /// </summary>
    /// <param name="name"></param>
    public void CurrentScene(string name)
    {
        switch (name)
        {
            case SceneName.City:
            case SceneName.Gorge:
                PathManager.Instance.InitPath();
                ResetProp();
                WeaponManager.Instance.Init();
                PathEffect.Instance.StartPathEffect();
                _player = ioo.poolManager.Spawn(_selectedPlayerName).GetOrAddComponent<Player>();
                RunMode(GameState.Play);
                break;
        }
    }

    private string GameMusic;
    /// <summary>
    /// 改变游戏装填
    /// </summary>
    /// <param name="state"></param>
    public void RunMode(GameState state)
    {
        if (_state == state)
            return;

        switch (state)
        {
            case GameState.Coin:
                UIManager.Instance.OpenUI(EnumUIType.PanelCoin);
                break;
            case GameState.Loading:
                Const.GAME_CONFIG_TIME = SettingManager.Instance.PlayTime;
                break;
            case GameState.Select:
                break;
            case GameState.Setting:
                break;
            case GameState.Play:
                if (_state == GameState.Continue)
                {
                    //++_times;
                    _player.AddLife();
                    ioo.audioManager.StopBackMusic("Music_Time_CountDown");
                    ioo.audioManager.PlaySound2D("Music_Time_CountDown_end");
                }
                break;
            case GameState.Continue:
                ioo.audioManager.PlayBackMusic("Music_Time_CountDown");
                break;
            case GameState.Back:
                _player.Land();
                TellIOBoardGameEnd();
                TimeScale(1);
                break;
            case GameState.Summary:
                break;
        }
        _state = state;
    }

    public void ResetProp()
    {
        PropsManager.Instance.Init();
    }

    public void PlayGameMusic()
    {
        ioo.audioManager.PlayBackMusic(GameMusic);
    }

    public void StopGameMusic()
    {
        ioo.audioManager.StopBackMusic(GameMusic);
    }

    public void PlayPlaneMusic()
    {
        string music = string.Empty;
        switch (_selectedPlayerName)
        {
            case "Player0":
                music = "Music_Player0";
                break;
            case "Player1":
                music = "Music_Player1";
                break;
            case "Player2":
                music = "Music_Player2";
                break;
        }
        ioo.audioManager.PlayBackMusic(music);
    }

    public void StopPlaneMusic()
    {
        string music = string.Empty;
        switch (_selectedPlayerName)
        {
            case "Player0":
                music = "Music_Player0";
                break;
            case "Player1":
                music = "Music_Player1";
                break;
            case "Player2":
                music = "Music_Player2";
                break;
        }
        ioo.audioManager.StopBackMusic(music);
    }

    /// <summary>
    /// 选择地图
    /// </summary>
    /// <param name="map"></param>
    public void MapSelect(E_Map map)
    {
        _map = map;
        switch (map)
        {
            case E_Map.City:
                GameMusic = "Music_City";
                enviroment = Enviroment.Normal;
                break;
            case E_Map.Gorge:
                GameMusic = "Music_Gorge";
                enviroment = Enviroment.Forest;
                break;
        }
    }

    /// <summary>
    /// 选择飞机
    /// </summary>
    /// <param name="name"></param>
    public void PlayerSelected(string name)
    {
        _type = PilotType.Player;
        _selectedPlayerName = name;
        ioo.poolManager.AddPreLoad(name, 1);
    }

    /// <summary>
    /// 获取飞机行驶百分百
    /// </summary>
    public float GetPilotPrecent
    {
        get
        {
            float percent = 0;
            if (_type == PilotType.Player)
                percent = _player.Percent;

            return percent;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="scale"></param>
    public void TimeScale(float scale)
    {
        Time.timeScale = scale;
    }

    ///// <summary>
    ///// Boss出生
    ///// </summary>
    //public void SpawnCityBoss()
    //{
    //    GameObject obj = ioo.poolManager.Spawn(ModelName.Boss_City);
    //    _boss = obj.GetOrAddComponent<Boss_City>();
    //}

    ///// <summary>
    ///// Boss出生
    ///// </summary>
    //public void SpawnGorgeBoss()
    //{
    //    GameObject obj = ioo.poolManager.Spawn(ModelName.Boss_City);
    //    _boss = obj.GetOrAddComponent<Boss_Gorge>();
    //}

    ///// <summary>
    /////  回收Boss
    ///// </summary>
    //public void DespawnBoss()
    //{
    //    if (_boss != null)
    //        ioo.poolManager.DeSpawn(_boss.gameObject);
    //}
    #endregion
 
    #region Unity CallBack
    void Awake()
    {
        EventDispatcher.AddEventListener(EventDefine.Event_Turn_Left, OnLeft);
        EventDispatcher.AddEventListener(EventDefine.Event_Turn_Right, OnRight);
        EventDispatcher.AddEventListener(EventDefine.Event_Turn_Up, OnUp);
        EventDispatcher.AddEventListener(EventDefine.Event_Turn_Down, OnDown);
        EventDispatcher.AddEventListener(EventDefine.Event_Sure_Or_Missile, OnMissile);
    }

    void Destroy()
    {
        EventDispatcher.RemoveEventListener(EventDefine.Event_Turn_Left, OnLeft);
        EventDispatcher.RemoveEventListener(EventDefine.Event_Turn_Right, OnRight);
        EventDispatcher.RemoveEventListener(EventDefine.Event_Turn_Left, OnUp);
        EventDispatcher.RemoveEventListener(EventDefine.Event_Turn_Right, OnDown);
        EventDispatcher.RemoveEventListener(EventDefine.Event_Sure_Or_Missile, OnMissile);
    }

    void Update()
    {
        if (State == GameState.Coin)
        {
            if (_idleTime > 0)
                _idleTime -= Time.deltaTime;
            else
            {
                if (!_isIdle)
                {
                    _isIdle = true;
                    _idleTime = 0;
                    SettingManager.Instance.Idle = true;
                    //ioo.audioManager.Idle();
                    EventDispatcher.TriggerEvent(EventDefine.Event_Idle_Movie, true);
                }
            }
        }

        if (State == GameState.Play)
        {
            PropsManager.Instance.Update();
            WeaponManager.Instance.Update();
        }

        if (_canFire)
        {
            if (State == GameState.Play && _player != null)
            {
                _player.OnFire();
            }
        }
    }
    #endregion


    #region Private Function
    private void OnLeft()
    {
        if (State == GameState.Play && _player != null)
        {
            _player.PilotController.TurnLeft();
        }
    }

    private void OnRight()
    {
        if (State == GameState.Play && _player != null)
        {
            _player.PilotController.TurnRight();
        }
    }

    private void OnUp()
    {
        if (State == GameState.Play && _player != null)
        {
            _player.PilotController.PullUp();
        }
    }

    private void OnDown()
    {
        if (State == GameState.Play && _player != null)
        {
            _player.PilotController.PullDown();
        }
    }

    private void OnMissile()
    {
        if (State == GameState.Play && _player != null)
        {
            _player.OnMissile();
        }
    }

    private void TellIOBoardGameEnd()
    {
        int ticket = 0;
        if (SettingManager.Instance.GameTicket != 0)
           ticket = ioo.gameMode.Player.Data.ResultScore / SettingManager.Instance.GameTicket;
        IOManager.Instance.SeneMessageTicket(ticket);
    }
    #endregion
   
}
