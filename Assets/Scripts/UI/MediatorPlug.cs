/*
 * Copyright (c) 
 * 
 * 文件名称：   MediatorPlug.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/3/29 10:26:19
 * 
 * 修改描述：
 * 
 */

using UnityEngine;
using System.Collections;

public interface IMediatorPlug
{
    void Connect();
    void Disconnect();
    string GetName();
    string GetClassRef();
    UnityEngine.Object GetView();
}

public class MediatorPlug : IMediatorPlug
{
    [SerializeField]
    private string mediatorName;
    [SerializeField]
    private string mediatorClassRef;
    // Use this for initialization

    private UnityEngine.Object viewComponent;

    public MediatorPlug(UnityEngine.Object obj, string mpref)
    {
        viewComponent       = obj;
        mediatorClassRef    = mpref;
        Connect();
    }

    public void Connect()
    {
        GameFacade.Instance.ConnectMediator(this);
    }

    public void Disconnect()
    {
        GameFacade.Instance.DisconnectMediator(mediatorName);
    }


    public string GetName()
    {
        if (string.IsNullOrEmpty(mediatorName))
        {
            mediatorName = System.Guid.NewGuid().ToString();
        }
        return mediatorName;
    }

    public string GetClassRef()
    {
        return mediatorClassRef;
    }

    public UnityEngine.Object GetView()
    {
        return viewComponent;
    }
}
