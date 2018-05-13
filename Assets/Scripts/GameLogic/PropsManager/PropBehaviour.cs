/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   PropBehaviour.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/5/26 15:03:02
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using System;
using Need.Mx;

public class PropBehaviour : MonoBehaviour
{
    // 拥有哲
    private string onwer;

    // 道具类型
    private PropType type;

    // 是否可被武器攻击
    private bool assaultable;

    // 音效名字
    private string bornVolumeName;
    private string dieVolumeName;

    // 回收时播放的特效
    private string dieEffect;
    // 出生时特效
    private string bornEffect;

    private float damageValue;

    private float damagePlane;

    private float halfSize = 2.5f;

    private Transform root;

    [NonSerialized]
    public Transform ShootPoint;

    // 右边的点
    private Vector3 rightPos;
    // 上边的点
    private Vector3 upPos;

    // 移动目标对象
    private Transform toTarget;
    // 移动目标点
    private Vector3 toPos;
    // 移动方向
    private Vector3 toDir;
    // 移动速度
    private float moveSpeed;

    private Vector3 oldDir;

    // 道具位于路径百分比
    private float Percent;

    private int score;

    public float LocalPercent   { get { return Percent; }       set { Percent   = value; } }
    public PropType Type        { get { return type; }          set { type      = value; } }
    public bool Assaultable     { get { return assaultable; }   set { assaultable = value; } }
    public float DamageValue    { get { return damageValue; }   set { damageValue = value; } }
    public float DamagePlane    { get { return damagePlane; }   set { damagePlane = value; } }
    public string BornVolumeName{ set { bornVolumeName = value; } }
    public string DieVolumeName { set { dieVolumeName = value; } }
    public string BornEffect    { set { bornEffect  = value; } }
    public string DieEffect     { set { dieEffect   = value; } }
    public string Owner         { get { return onwer; } set { onwer = value; } }
    public int Score            { get { return score; } set { score = value; } }
    public bool IsLocked        { get; set; }
    public Transform Root       { get { return root; } }
    public WeaponBehaviour LockedBy { get; set; }

    
    #region Unity CallBack
    void Awake()
    {
        root = transform.Find("Root").transform;
        ShootPoint = root.Find("ShootPoint").transform;
    }

    void OnDisable()
    {
        toPos       = Vector3.zero;
        toTarget    = null;
        toDir       = Vector3.zero;
        oldDir      = Vector3.zero;
        damageValue = 0;
        damagePlane = 0;
        assaultable = false;
        IsLocked        = false;
        bornVolumeName  = string.Empty;
        dieVolumeName   = string.Empty;
        bornEffect      = string.Empty;
        dieEffect       = string.Empty;

        if (CanRotation())
        {
            root.localEulerAngles = Vector3.zero;
        }
        PlayBornSound();
    }
   
    void Update()
    {
        if (LockedBy == null || !LockedBy.gameObject.activeSelf)
        {
            IsLocked = false;
        }

        #region -----------------------------向目标移动-----------------------------------
        Vector3 targetpos = Vector3.zero;
        if (toTarget != null)
            targetpos = toTarget.position;

        if (toPos != Vector3.zero)
            targetpos = toPos;

        if (targetpos != Vector3.zero)
        {
            Vector3 dir = targetpos - transform.position;
            dir.Normalize();
            if (Vector3.Distance(targetpos, transform.position) < 0.1f || Vector3.Angle(dir, oldDir) > 120)
            {
                // 到达目标
                toTarget = null;
                toPos = Vector3.zero;
            }
            else
            {
                transform.position += dir * Time.deltaTime * moveSpeed;
                oldDir = dir;
            }
        }

        if (toDir != Vector3.zero)
        {
            toDir.Normalize();
            transform.position += toDir * Time.deltaTime * moveSpeed;
        }

        #endregion

        #region ------------------------自转--------------------------------
        if (!CanRotation())
            return;

        root.localEulerAngles += Vector3.up * Time.deltaTime * 150;
        #endregion
    }
    #endregion

    

    #region Private Function
    private bool CanRotation()
    {
        switch (type)
        {
            case PropType.Coin:
            case PropType.Diamond:
            case PropType.Fuel:
            case PropType.Magnet:
            case PropType.Mine:
            case PropType.Shield:
            case PropType.Fix:
            case PropType.Engine:
            case PropType.Missile:
                return true;
        }
        return false;
    }

    // 碰撞回收时播放特效
    private void PlayDeSpawnEffect()
    {
        if (string.IsNullOrEmpty(dieEffect))
            return;

        // 配表比较合适，懒得改了
        if (type == PropType.Mine)
            EffectManager.Instance.Spawn(dieEffect, Root);
        else
            EffectManager.Instance.SpawnEffectInPlayer(dieEffect);
    }

    // 播放出生特效
    private void PlayBornEffect()
    {
        if (string.IsNullOrEmpty(bornEffect))
            return;
        EffectManager.Instance.Spawn(bornEffect, transform.position);
    }

    // 出生音效
    private void PlayBornSound()
    {
        if (string.IsNullOrEmpty(bornVolumeName))
            return;
        ioo.audioManager.PlaySound2D(bornVolumeName);
    }

    // 播放触发时音效
    private void PlayTriggerSound()
    {
        if (string.IsNullOrEmpty(dieVolumeName))
            return;
        ioo.audioManager.PlaySound2D(dieVolumeName);
    }
  
    #endregion
 
    #region Public Function
    // 初始化
    public void Spawn(Vector3 pos0, Vector3 pos1,int id = 0)
    {
        PlayBornEffect();

        rightPos    = pos0;
        upPos       = pos1;

        Vector3 upDir = upPos - transform.position;

        if (type == PropType.Wall)
        {
            int rand = UnityEngine.Random.Range(0, 50);
            if (rand % 2 == 0)
            {
                Root.localEulerAngles = Vector3.forward * 0;
                Root.localPosition = Vector3.up * 2.0f;
            }
            else
            {
                Root.localEulerAngles = Vector3.forward * 90;
                Root.localPosition = Vector3.up * 5.5f;
            }
        }

        if (!CanRotation())
            return;
        
        root.localEulerAngles += Vector3.up * id * 90;
    }

    // 出生偏左
    public void LocateLeft()
    {
        Vector3 dir = rightPos - transform.position;
        dir.Normalize();
        transform.position += halfSize * dir;
    }

    // 出生偏右
    public void LocateRight()
    {
        Vector3 dir = rightPos - transform.position;
        dir.Normalize();
        transform.position -= halfSize * dir;
    }

    // 目标对象
    public void SetMoveToTargetTransform(Transform trans, float speed = 100)
    {
        if (toTarget != null)
            return;
        toTarget = trans;
        moveSpeed = speed;
    }

    // 目标点
    public void SetMoveToTragetPosition(Vector3 pos, float speed = 100)
    {
        if (toPos != Vector3.zero)
            return;
        toPos = pos;
        moveSpeed = speed;
    }

    // 朝给定方向移动
    public void SetMoveToDir(Vector3 dir, float speed = 150)
    {
        if (toDir != Vector3.zero)
            return;
        toDir = dir;
        moveSpeed = speed;
    }

    public void Trigger(bool BeAttacked = false)
    {
        PlayDeSpawnEffect();
        PlayTriggerSound();
        if (BeAttacked)
        {
            PropsManager.Instance.SpawnPropInPos(PropType.Coin, transform.position, Percent);
        }
    }

    // 回收
    public void DeSpawn()
    {      
        ioo.poolManager.DeSpawn(gameObject);
    }

    public void PlayDestroyEffect()
    {
        if (string.IsNullOrEmpty(dieEffect))
            return;
        EffectManager.Instance.Spawn(dieEffect, transform.position);
    }
    #endregion
  
}
