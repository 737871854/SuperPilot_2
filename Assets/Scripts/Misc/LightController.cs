/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   LightController.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/7/10 14:32:49
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using Need.Mx;

public class LightController : MonoBehaviour
{

    public GameObject JiKuLight;
    public GameObject GameLight;

    void Start()
    {
        EventDispatcher.AddEventListener<bool>(EventDefine.Event_JiKu_To_Game_Light, OnSwitchLight);
        EventDispatcher.AddEventListener(EventDefine.Event_Lift_Up, OnLight);
    }

    void Destroy()
    {
        EventDispatcher.RemoveEventListener<bool>(EventDefine.Event_JiKu_To_Game_Light, OnSwitchLight);
        EventDispatcher.AddEventListener(EventDefine.Event_Lift_Up, OnLight);
    }

    private void OnSwitchLight(bool yes)
    {
        if (yes)
        {
            JiKuLight.SetActive(false);
            GameLight.SetActive(true);
        }
        else
        {
            JiKuLight.SetActive(true);
            GameLight.SetActive(false);
        }
        
    }

    private void OnLight()
    {
        JiKuLight.SetActive(true);
    }
}
