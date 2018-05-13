/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   CheckGUID.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/5/12 17:35:21
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CheckGUID : MonoBehaviour
{
    public GameObject text;
    void Start()
    {
        //string guid = PlayerPrefs.GetString("CitySUVGUID");
        //if (guid.Equals(SystemInfo.deviceUniqueIdentifier))
        //{
        //    text.SetActive(false);
        //    SceneManager.LoadScene(SceneName.Main);
        //}
        //else
        //{
        //    text.SetActive(true);
        //}

        SceneManager.LoadScene(SceneName.Main);
    }
}
