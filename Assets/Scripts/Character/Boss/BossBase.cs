/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   BossBase.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/7/14 9:26:10
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;

public class BossBase : MonoBehaviour
{
    /// <summary>
    /// 是否存活
    /// </summary>
    [System.NonSerialized]
    public bool IsDead;

    /// <summary>
    /// 射击点
    /// </summary>
    public Transform ShootPoint;

    /// <summary>
    /// Animator挂点
    /// </summary>
    [System.NonSerialized]
    public Transform Root;

    /// <summary>
    /// 发射点
    /// </summary>
    public Transform WeaponPoint;

    public Transform ViewPoint;

    /// <summary>
    /// 生命值
    /// </summary>
    protected float Life;

    /// <summary>
    /// 最大生命值
    /// </summary>
    protected float MaxLife;

    /// <summary>
    /// 动画组件
    /// </summary>
    protected Animator Anim;

    /// <summary>
    /// 杀死Boss获得积分
    /// </summary>
    protected int Worth = 2000;


    public Vector3 Postion { get { return Root.position; } }
    public float HealthProgress { get { return Life / MaxLife; } }


    public virtual void OnDamage(float value, bool isPlay = false) { }

    protected virtual void PlayBackGroundMusic()
    {
        string music = string.Empty;
        if (ioo.gameMode.Map == E_Map.City)
        {
            music = "Music_City_Boss";
        }
        else
        {
            music = "Music_Gorge_Boss";
        }
        ioo.audioManager.PlayBackMusic(music);
        ioo.gameMode.StopGameMusic();
    }

    protected virtual void StopBackGroundMusic()
    {
        string music = string.Empty;
        if (ioo.gameMode.Map == E_Map.City)
        {
            music = "Music_City_Boss";
        }
        else
        {
            music = "Music_Gorge_Boss";
        }
        ioo.audioManager.StopBackMusic(music);
        if (ioo.gameMode.State == GameState.Play)
            ioo.gameMode.PlayGameMusic();
    }
}
