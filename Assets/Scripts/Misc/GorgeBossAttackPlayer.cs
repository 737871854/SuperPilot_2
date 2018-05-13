/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   GorgeBossAttackPlayer.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/8/11 17:07:17
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;
using Need.Mx;

public class GorgeBossAttackPlayer : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    void Destroy()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.tag.Equals(GameTag.Weapon))
            return;

        WeaponBehaviour wb = other.GetComponent<WeaponBehaviour>();
        if (wb.Type == WeaponType.FireBal)
        {
            wb.Trigger();
            EventDispatcher.TriggerEvent(EventDefine.Event_Groge_Boss_Attack);
            if (ioo.gameMode.Player.ShieldLife - 3 < 0)
            {
                ioo.gameMode.Player.OnDamage(ioo.gameMode.Player.ShieldLife - 3);
                EventDispatcher.TriggerEvent(EventDefine.Event_Tips_Type_By_Int, 0, wb.transform.position, (int)(3 - ioo.gameMode.Player.ShieldLife));
            }
            WeaponManager.Instance.AddDespawnWeapon(wb);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
