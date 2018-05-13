/*
 * Copyright (c) 
 * 
 * 文件名称：   Main.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/3/28 16:53:35
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;
using Need.Mx;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    private float upTime;
    private float gameTime;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        
        gameObject.AddComponent<GameController>();
        gameObject.AddComponent<AudioManager>();
        gameObject.AddComponent<GameMode>();
        gameObject.AddComponent<PoolManager>();
        gameObject.AddComponent<ScenesManager>();
        gameObject.AddComponent<NonStopTime>();
    }

    // Use this for initialization
    void Start()
    {
        SettingManager.Instance.Init();

        ioo.gameController.Init(InputType.Mouse);
        ioo.gameMode.RunMode(GameState.Check);

        ioo.safeNet.InitDog();

        UIManager.Instance.OpenUI(EnumUIType.PanelCheckHard);

        LoadingManager.Instance.AddJsonFiles(Const.Props_Json_Path, PropsData.LoadHandler);
        LoadingManager.Instance.AddJsonFiles(Const.Weapon_Json_Path, WeaponData.LoadHandler);

        LoadingManager.Instance.StartLoad(InitMode);
    }

    void InitMode()
    {
        GameFacade.Instance.StartUp();

        ioo.gameMode.Init();
    }

    void Destroy()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (upTime > 0)
        {
            upTime -= Time.deltaTime;
        }
        else
        {
            upTime = 5;
            SettingManager.Instance.LogUpTime(5);
        }

        if (ioo.gameMode.State >= GameState.Play)
        {
            if (gameTime > 0)
            {
                gameTime -= Time.deltaTime;
            }
            else
            {
                gameTime = 5;
                SettingManager.Instance.LogGameTimes(5);
            }
        }
    }

    //public void OnGUI()
    //{
    //    if (null == IOManager.Instance.serialioHost)
    //    {
    //        return;
    //    }

    //    GUIStyle style = new GUIStyle();
    //    style.fontSize = 30;
    //    GUI.Label(new Rect(300, 0, 100, 100), IOManager.Instance.serialioHost.MessageStr, style);
    //}
}
