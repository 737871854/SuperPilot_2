/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   WelCome.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/6/12 11:19:45
 * 
 * 修改描述：
 * 
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class WelCome : MonoBehaviour
{
    public MovieTexture Movie;
    public RawImage RawImage;

    void Start()
    {
        Cursor.visible = false;
        Screen.SetResolution(1440, 900, true);
        Screen.fullScreen = true;
        QualitySettings.SetQualityLevel((int)QualityLevel.Simple);

        RawImage.texture = Movie;

        AudioSource audioSource = GetComponent<AudioSource>();

        audioSource.clip = Movie.audioClip;

        Movie.Play();

        audioSource.Play();

        StartCoroutine(OpenMainScene());
    }

    IEnumerator OpenMainScene()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(SceneName.CheckGUID);
    }
}
