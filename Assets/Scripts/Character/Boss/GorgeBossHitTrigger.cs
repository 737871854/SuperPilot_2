/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   GorgeBossHitTrigger.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/9/8 19:26:27
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using Need.Mx;

public class GorgeBossHitTrigger : MonoBehaviour
{
    public Boss_Gorge boss;
    private Collider collider;

    void Start()
    {
        collider = GetComponent<SphereCollider>();
        collider.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(GameTag.Weapon))
        {
            WeaponBehaviour wb = other.GetComponent<WeaponBehaviour>();

            if (wb.Owner != GameTag.Player)
                return;
            EventDispatcher.TriggerEvent(EventDefine.Event_Hit_Break, transform.position);
            wb.Trigger();
            boss.RemoveHit(this);
            DisableCollider();
        }
    }

    public void EnableCollider()
    {
        collider.enabled = true;
    }

    public void DisableCollider()
    {
        collider.enabled = false;
    }
}
