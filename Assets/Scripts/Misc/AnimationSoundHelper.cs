/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   AnimationSoundHelper.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/8/15 9:22:17
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;

public class AnimationSoundHelper : MonoBehaviour
{

    public void PlaySABCSound()
    {
        ioo.audioManager.PlaySound2D("SFX_SABC_Sound");
    }
}
