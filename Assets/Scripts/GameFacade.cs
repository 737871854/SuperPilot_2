/*
 * Copyright (c) 
 * 
 * 文件名称：   GameFacade.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/3/28 16:54:34
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using PureMVC.Interfaces;
using System;

public class GameFacade : PureMVC.Patterns.Facade
{
    /// <summary>
    /// Singleton
    /// </summary>
    public static GameFacade Instance
    {
        get
        {
            if (null == m_instance)
                m_instance = new GameFacade();
            return m_instance as GameFacade;
        }
    }

    /// <summary>
    /// InitializeController
    /// </summary>
    protected override void InitializeController()
    {
        base.InitializeController();
        RegisterCommand(NotificationName.PanelCoin, typeof(PanelCoinCommand));
        RegisterCommand(NotificationName.PanelSelect, typeof(PanelSelectCommand));
        RegisterCommand(NotificationName.PanelMain, typeof(PanelmainCommand));
        RegisterCommand(NotificationName.PanelSummary, typeof(PanelSummaryCommand));
        RegisterCommand(NotificationName.PanelCheckHard, typeof(PanelCheckHardCommand));
        RegisterCommand(NotificationName.PanelSetting, typeof(PanelSettingCommand));
        RegisterCommand(NotificationName.PanelLoading, typeof(PanelLoadingCommand));
    }

    /// <summary>
    /// Start Game
    /// </summary>
    public void StartUp()
    {
        SendNotification(NotificationName.PanelCoin);
        SendNotification(NotificationName.PanelSelect);
        SendNotification(NotificationName.PanelMain);
        SendNotification(NotificationName.PanelSummary);
        SendNotification(NotificationName.PanelCheckHard);
        SendNotification(NotificationName.PanelSetting);
        SendNotification(NotificationName.PanelLoading);
    }

    //Handle IMediatorPlug connection
    public void ConnectMediator(IMediatorPlug item)
    {
        Type mediatorType = Type.GetType(item.GetClassRef());
        if (mediatorType != null)
        {
            IMediator mediatorPlug = (IMediator)Activator.CreateInstance(mediatorType, item.GetName(), item.GetView());
            RegisterMediator(mediatorPlug);
        }
    }

    public void DisconnectMediator(string mediatorName)
    {
        RemoveMediator(mediatorName);
    }
}
