﻿/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   Effect.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/7/18 14:52:34
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;

public class SingleEffect : MonoBehaviour
{
    private ParticleSystem[] Particles;

    private float Duration;

    private float PlayTime;

    #region Unity CallBack
    void OnDisable()
    {
        PlayTime = 0;
        Duration = 0;

        if (gameObject.tag.Equals(GameTag.JiGuang))
        {
            ioo.audioManager.StopBackMusic("Music_Laser");
        }
    }

    void OnEnable()
    {
        Particles = transform.GetComponentsInChildren<ParticleSystem>();
        for (int i = 0; i < Particles.Length; ++i)
        {
            if (Particles[i].duration > Duration)
                Duration = Particles[i].duration;
        }

        if (gameObject.tag.Equals(GameTag.JiGuang))
        {
            ioo.audioManager.PlayBackMusic("Music_Laser");
        }
    }

    void Update()
    {
        if (PlayTime < Duration)
            PlayTime += ioo.nonStopTime.deltaTime;
        else
            DeSpawn();
    }
    #endregion

    #region Private Function

    private void DeSpawn()
    {
        gameObject.SetActive(false);
    }
    #endregion
}
