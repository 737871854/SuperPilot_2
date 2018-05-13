/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   MediatorManager.cs
 * 
 * 简    介:    游戏中所有Mediator的管理者
 * 
 * 创建标识：   Pancake 2017/3/29 9:33:40
 * 
 * 修改描述：   2017/6/1 修复Mediator还未注册完毕，就移除Mediator导致的Bug
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class MediatorManager : Singleton<MediatorManager>
{
    private Dictionary<EnumUIType, MediatorPlug> mpDic = new Dictionary<EnumUIType, MediatorPlug>();

    /// <summary>
    /// 通过ui名注册对应的MediatorPlug
    /// </summary>
    /// <param name="panelName"></param>
    /// <param name="viewComponent"></param>
    public void RegesterMediatorPlug(EnumUIType type, UnityEngine.Object viewComponent, string mediatorClassRef)
    {
        if (!mpDic.ContainsKey(type))
        {
            if (string.IsNullOrEmpty(mediatorClassRef))
            {
                Debug.Log("mediatorClassRef is null");
                return;
            }

            MediatorPlug mp = new MediatorPlug(viewComponent, mediatorClassRef);
            mpDic.Add(type, mp);
        }
    }

    /// <summary>
    /// 通过ui名获取对应的MediatorPlug
    /// </summary>
    /// <param name="panelName"></param>
    /// <returns></returns>
    public MediatorPlug GetMediatorPlug(EnumUIType type)
    {
        if (!mpDic.ContainsKey(type))
        {
            Debug.Log(type + " is not regester, please regester first");
            return null;
        }

        return mpDic[type];
    }

    /// <summary>
    /// 通过ui名解除MediatorPlug和对应ui对象的关系
    /// </summary>
    /// <param name="type"></param>
    public void RemoveMediatorPlug(EnumUIType type)
    {
        CoroutineController.Instance.StartCoroutine(CoroutineRemove(type));
    }

    public void Clear()
    {
        mpDic.Clear();
    }

    IEnumerator CoroutineRemove(EnumUIType type)
    {
        while(!mpDic.ContainsKey(type))
        {
            yield return new WaitForEndOfFrame();
        }

        if (mpDic[type] == null)
           yield break;

        mpDic[type].Disconnect();
        mpDic.Remove(type);
    }
}
