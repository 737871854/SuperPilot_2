/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   SelectBK.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/8/23 16:49:17
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;

public class SelectBK : MonoBehaviour
{
    public GameObject CH;
    public GameObject EN;
    // Use this for initialization

    void FixedUpdate()
    {
        if (SettingManager.Instance.GameLanguage == 0)
        {
            CH.SetActive(true);
            EN.SetActive(false);
        }
        else
        {
            CH.SetActive(false);
            EN.SetActive(true);
        }
    }
}
