/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   CityDoor.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/9/6 15:44:12
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CityDoor : MonoBehaviour
{
    public Animation Anim;
    public GameObject Effect;
    private float Life;

    void Start()
    {
        Life = 5;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(GameTag.Weapon) && Life > 0)
        {
            WeaponBehaviour wb = other.GetComponent<WeaponBehaviour>();
            Life += wb.DamageValue;
            wb.Trigger();
            if (Life <= 0)
            {
                Effect.SetActive(true);
                ioo.audioManager.PlaySoundOnPoint("SFX_Sound_Door_Boom", Anim.transform.position);
                Anim["Take 001"].speed = 1;
                Anim.CrossFade("Take 001");
                StartCoroutine(DelayToShow());
            }
        }
    }

    IEnumerator DelayToShow()
    {
        yield return new WaitForSeconds(15);
        Life = 5;
        Anim["Take 001"].speed = -1;
        Anim.CrossFade("Take 001");
    }
}
