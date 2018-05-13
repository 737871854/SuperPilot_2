/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   BossTrigger.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/5/31 9:44:12
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;

public class BossTrigger : MonoBehaviour
{
    public BossBase Boss;

    #region Unity CallBack
    void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(GameTag.Weapon))
        {
            WeaponBehaviour wb = other.gameObject.GetComponent<WeaponBehaviour>();
            if (wb.Owner == GameTag.Boss)
                return;

            if (wb.Owner == GameTag.Friend)
            {
                if (wb.Type == WeaponType.Missle || wb.Type == WeaponType.Missle2)
                    Boss.OnDamage(-1000); // 必杀
                else
                    Boss.OnDamage(wb.DamageValue, false);
            }else
            {
                Boss.OnDamage(wb.DamageValue, true);
            }

            wb.Trigger();
            WeaponManager.Instance.AddDespawnWeapon(wb);
        }
    }
    #endregion
}
