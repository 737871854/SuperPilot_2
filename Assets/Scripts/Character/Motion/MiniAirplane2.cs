/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   MiniAirplane2.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/7/22 13:43:55
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;

public class MiniAirplane2 : MonoBehaviour
{
    public Transform FirePoint;

    public int Index = 0; // 0:上，1：左

    public Transform HitPoint;

    private float Life;

    private float deltaTime = 0.2f;

    public float PercentOffset = 0.004f;
    public float Speed = 100;
    private Vector3 Direction;
    private Vector3 TargetPos;

    private Vector3 StartPos;
    private Vector3 UpDir;

    private float LifeTime;

    void Awake()
    {
        StartPos = transform.position;
    }

    void OnEnable()
    {
        Life = 1.0f;

        LifeTime = 3;

        transform.position = StartPos;

        // 指定
        float percent   = ioo.gameMode.Player.Percent + PercentOffset; 
        TargetPos       = PathManager.Instance.PathInfo1.GetPos(percent);
        UpDir           = PathManager.Instance.PathInfo1.GetUpPos(percent) - TargetPos;
        UpDir.Normalize();
        Vector3 targetRight = PathManager.Instance.PathInfo1.GetRightPos(percent);

        Vector3 offset = ioo.gameMode.Player.Offset;
        TargetPos += offset;
        targetRight += offset;

        Vector3 rightDir = targetRight - TargetPos;
        rightDir.Normalize();
        Direction = TargetPos - transform.position;
        Direction.Normalize();
        transform.LookAt(TargetPos);
    }

    void OnDisable()
    {
        EffectManager.Instance.Spawn(EffectName.Effect_Enemy_2_baoz, transform.position);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.tag.Equals(GameTag.Weapon))
            return;

        WeaponBehaviour wb = other.GetComponent<WeaponBehaviour>();

        if (wb.Owner != GameTag.Player)
            return;

        OnDamage(wb.DamageValue);
        WeaponManager.Instance.AddDespawnWeapon(wb);
    }

    private void OnDamage(float damage)
    {
        Life += damage;
        if (Life <= 0)
        {
            Life = 0;
            Death();
        }
    }

    private void Death()
    {
        gameObject.SetActive(false);
        ioo.audioManager.PlaySoundOnPoint("SFX_Sound_Enemy_Plane_Destroy", transform.position);
        EffectManager.Instance.Spawn(EffectName.Effect_diji_Plane_baoz, transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (deltaTime > 0)
            deltaTime -= Time.deltaTime;
        else
        {
            deltaTime = 0.2f;
            Fire();
        }

        transform.position += Direction * Time.deltaTime * Speed;

        if (LifeTime > 0)
        {
            LifeTime -= Time.deltaTime;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
  
    private void Fire()
    {
        WeaponBehaviour wb = WeaponManager.Instance.CreateWeapon(ModelName.WBullet);
        wb.transform.position = FirePoint.position;
        wb.Owner = GameTag.Friend;
        wb.SetMoveToDir(FirePoint.forward);
        WeaponManager.Instance.MiniPlanWeapon(wb);
    }
}
