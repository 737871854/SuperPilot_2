/*
 * Copyright (c) 
 * 
 * 文件名称：   ioo.cs
 * 
 * 简    介:    所有管理器持有者
 * 
 * 创建标识：  Pancake 2017/4/21 8:32:44
 * 
 * 修改描述：  
 * 
 */


using UnityEngine;
using System.Collections;
using System.Text;

/// <summary>
/// Interface Manager Object 
/// </summary>
public class ioo {

    /// <summary>
    /// 游戏管理器对象
    /// </summary>
    private static GameObject _manager = null;
    public static GameObject manager {
        get { 
            if (_manager == null) 
                _manager = GameObject.FindWithTag("GameManager");
            return _manager;
        }
    }

    /// <summary>
    /// 操作控制管理器
    /// </summary>
    private static GameController _gameController = null;
    public static GameController gameController
    {
        get
        {
            if (_gameController == null)
            {
                _gameController = manager.GetComponent<GameController>();
            }
            return _gameController;
        }
    }

    /// <summary>
    /// 扩展的声音管理器
    /// </summary>
    private static AudioManager _audioManager = null;
    public static AudioManager audioManager
    {
        get
        {
            if (_audioManager == null)
            {
                _audioManager = manager.GetComponent<AudioManager>();
            }
            return _audioManager;
        }
    }

    /// <summary>
    /// 扩展的对象池管理器
    /// </summary>
    private static PoolManager _poolManager = null;
    public static PoolManager poolManager
    {
        get
        {
            if (_poolManager == null)
            {
                _poolManager = manager.GetComponent<PoolManager>();
            }
            return _poolManager;
        }
    }

    /// <summary>
    /// 场景管理
    /// </summary>
    private static ScenesManager _scenesManager = null;
    public static ScenesManager scenesManager
    {
        get
        {
            if (_scenesManager == null)
            {
                _scenesManager = manager.GetComponent<ScenesManager>();
            }
            return _scenesManager;
        }
    }

    /// <summary>
    /// 游戏状态管理
    /// </summary>
    private static GameMode _gameMode = null;
    public static GameMode gameMode
    {
        get
        {
            if (_gameMode == null)
                _gameMode = manager.GetComponent<GameMode>();
            return _gameMode;
        }
    }

    /// <summary>
    /// 加密狗
    /// </summary>
    private static SafeNet _safeNet = null;
    public static SafeNet safeNet
    {
        get
        {
            if (_safeNet == null)
                _safeNet = manager.GetComponent<SafeNet>();
            return _safeNet;
        }
    }

    private static NonStopTime _nonStopTime = null;
    public static NonStopTime nonStopTime
    {
        get
        {
            if (_nonStopTime == null)
                _nonStopTime = manager.GetComponent<NonStopTime>();
            return _nonStopTime;
        }
    }

    /// <summary>
    /// 格式化字符串
    /// </summary>
    /// <returns></returns>
    public static string f(string format, params object[] args) {
        StringBuilder sb = new StringBuilder();
        return sb.AppendFormat(format, args).ToString();
    }

    /// <summary>
    /// 字符串连接
    /// </summary>
    public static string c(params object[] args) { 
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < args.Length; i++ ) {
            sb.Append(args[i].ToString());
        }
        return sb.ToString();
    }

}
