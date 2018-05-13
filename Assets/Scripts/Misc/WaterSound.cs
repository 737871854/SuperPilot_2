/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   SpraySound.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/7/28 16:39:45
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;

public class WaterSound : MonoBehaviour
{
    private bool IsPlaying;

    public enum Type
    {
        Spray,
        WaterFull,
    }
    public Type type = Type.Spray;

    public float Radius = 100;

    void Destroy()
    {
        StopSound();
    }

    // Update is called once per frame
    void Update()
    {
        if (ioo.gameMode.Player == null)
            return;

        float distance = Vector3.Distance(transform.position, ioo.gameMode.Player.transform.position);
        if (distance < Radius)
        {
            PlaySound();
        }
        else
        {
            StopSound();
        }
    }

    private void PlaySound()
    {
        if (IsPlaying)
            return;

        IsPlaying = true;
        if (type == Type.Spray)
            ioo.audioManager.PlayMusicOnPoint("Music_Spray_Sound", transform.position);
        if (type == Type.WaterFull)
            ioo.audioManager.PlayMusicOnPoint("Music_WaterFull_Sound", transform.position);
    }

    private void StopSound()
    {
        if (!IsPlaying)
            return;

        IsPlaying = false;
        if (type == Type.Spray)
            ioo.audioManager.StopBackMusic("Music_Spray_Sound");
        if (type == Type.WaterFull)
            ioo.audioManager.StopBackMusic("Music_WaterFull_Sound");
    }
}
