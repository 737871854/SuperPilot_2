/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   BulletBehaviour.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/5/31 16:44:04
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using System;

public enum WeaponType
{
    Bullet  = 1,
    Missle  = 2,
    Mine    = 3,
    Stone   = 4,
    Laser   = 5,
    FireBal = 6,
    Missle2 = 7,
}

public class WeaponBehaviour : MonoBehaviour
{
    // 拥有哲
    private string onwer;

    // 道具类型
    private WeaponType type;

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

    private bool hasTarget;

    private Vector3 BornDir;
    // 移动目标对象
    private Transform toTarget;
    // 移动目标点
    private Vector3 toPos;
    // 移动方向
    private Vector3 toDir;

    private float moveSpeed;

    private Vector3 oldDir;

    // 道具位于路径百分比
    private float Percent;

    public float LocalPercent   { get { return Percent; } set { Percent = value; } }
    public WeaponType Type      { get { return type; } set { type = value; } }
    public bool Assaultable     { get { return assaultable; } set { assaultable = value; } }
    public float DamageValue    { get { return damageValue; } set { damageValue = value; } }
    public float DamagePlane    { get { return damagePlane; } set { damagePlane = value; } }
    public string BornVolumeName{ set { bornVolumeName = value; } }
    public string DieVolumeName { set { dieVolumeName = value; } }
    public string BornEffect    { set { bornEffect = value; } }
    public string DieEffect     { set { dieEffect = value; } }
    public string Owner         { get { return onwer; } set { onwer = value; } }
    public Transform Root       { get { return root; } }

    private float Life;

    #region Unity CallBack
    void Awake()
    {
        root = transform.Find("Root").transform;
        ShootPoint = root.Find("ShootPoint").transform;
        BornDir = ioo.gameMode.Player.FirePoint.forward* 0.5f;
    }

    void OnDisable()
    {
        toPos       = Vector3.zero;
        toTarget    = null;
        hasTarget   = false;
        toDir       = Vector3.zero;
        oldDir      = Vector3.zero;
        damageValue = 0;
        assaultable = false;
        bornVolumeName  = string.Empty;
        dieVolumeName   = string.Empty;
        bornEffect      = string.Empty;
        dieEffect       = string.Empty;
    }

    void OnTriggerEnter(Collider other)
    {
        // Mini怪物发射的
        if (Owner == GameTag.Enemy)
            return;

        if (Owner == GameTag.Boss)
            return;

        // 武器打到道具
        if (other.tag.Equals(GameTag.Props) && type != WeaponType.Stone)
        {
            PropBehaviour pb = other.transform.parent.GetComponent<PropBehaviour>();
            if (pb == null)
                pb = other.transform.parent.parent.GetComponent<PropBehaviour>();
            if(
               //pb.Type == PropType.Bat || 
               //pb.Type == PropType.FeiQi || 
               pb.Type == PropType.Mine)
                {
                    Trigger();
                    if (Owner == GameTag.Player)
                    {
                        pb.Trigger(true);
                    }
                    WeaponManager.Instance.AddDespawnWeapon(this);
                    pb.PlayDestroyEffect();
                    PropsManager.Instance.AddDespawnNormal(pb);
                    return;
                }
        }

        // 打到武器道具
        if (other.tag.Equals(GameTag.Weapon))
        {
            WeaponBehaviour wb = other.gameObject.GetComponent<WeaponBehaviour>();
            if (wb.Assaultable)
            {
                Trigger();
                WeaponManager.Instance.AddDespawnWeapon(this);
                WeaponManager.Instance.AddDespawnWeapon(wb);
            }
        }

        if (other.tag.Equals(GameTag.Bug))
        {
            Trigger();
            BugAttack ba = other.GetComponent<BugAttack>();
            ba.OnHurt(damageValue);
        }
    }

    void Update()
    {
        // 丢失目标
        if (hasTarget && toTarget == null)
        {
            WeaponManager.Instance.AddDespawnWeapon(this);
            return;
        }
        #region -----------------------------向目标移动-----------------------------------

        Vector3 targetpos = Vector3.zero;
        if (toTarget != null)
        {
            if (!toTarget.gameObject.activeSelf)
            {
                WeaponManager.Instance.AddDespawnWeapon(this);
                return;
            }
            else
            {
                targetpos = toTarget.position;
            }
            
        }

        if (toPos != Vector3.zero)
            targetpos = toPos;

        if (targetpos != Vector3.zero)
        {
            Vector3 dir = targetpos - transform.position;
            dir.Normalize();
            if (Vector3.Distance(targetpos, transform.position) < 0.05f || Vector3.Angle(dir, oldDir) > 120)
            {
                // 到达目标
                toTarget = null;
                toPos = Vector3.zero;
            }
            else
            {
                if (type == WeaponType.Missle || type == WeaponType.Missle2)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(dir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, ioo.nonStopTime.deltaTime * 100);
                }
                transform.position += (dir + BornDir) * ioo.nonStopTime.deltaTime * moveSpeed;
                oldDir = dir;
                if (BornDir.magnitude > 0.1f)
                    BornDir -= BornDir * ioo.nonStopTime.deltaTime;
                else
                    BornDir = Vector3.zero;
            }
        }

        if (toDir != Vector3.zero)
        {
            toDir.Normalize();
            transform.position += toDir * ioo.nonStopTime.deltaTime * moveSpeed;
            Quaternion toRotation = Quaternion.LookRotation(toDir);
            transform.rotation = toRotation;
        }

        #endregion
    }
    #endregion


    #region Private Function
    // 初始化
    public void Spawn(bool bornsound = true)
    {
        PlayBornEffect();
        if (bornsound)
            PlayBornSound();

        //if (type == WeaponType.FireBal)
        //{
        //    Life = 3;
        //}
    }

    // 碰撞回收时播放特效
    private void PlayDeSpawnEffect()
    {
        if (string.IsNullOrEmpty(dieEffect))
            return;
        EffectManager.Instance.Spawn(dieEffect, transform.position);
    }

    // 播放出生特效
    private void PlayBornEffect()
    {
        if (string.IsNullOrEmpty(bornEffect))
            return;
        EffectManager.Instance.Spawn(bornEffect, transform);
    }

    // 出生音效
    private void PlayBornSound()
    {
        if (string.IsNullOrEmpty(bornVolumeName))
            return;
        ioo.audioManager.PlaySound2D(bornVolumeName);
        //ioo.audioManager.PlaySoundOnObj(bornVolumeName, gameObject);
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
  
    // 目标对象
    public void SetMoveToTargetTransform(Transform trans, float speed = 100)
    {
        if (toTarget != null)
            return;
        hasTarget = true;
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

    public void Trigger()
    {
        PlayDeSpawnEffect();
        PlayTriggerSound();
    }

    // 回收
    public void DeSpawn()
    {
        ioo.poolManager.DeSpawn(gameObject);
    }
    #endregion
  
}
