/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   OnTrigger.cs
 * 
 * 简    介:    响应场景中的触发事件
 * 
 * 创建标识：   Pancake 2017/6/1 17:15:28
 * 
 * 修改描述： 废弃 (为了方便策划随时修改触发点，废弃该方法，改为直接在场景加触发点)
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class OnTriggerBehaviour : MonoBehaviour
{
    [System.NonSerialized]
    public string Name = string.Empty;

    /// <summary>
    /// 响应场景触发事件
    /// </summary>
    public void OnTrigger()
    {
        switch (Name)
        {
            case EffectName.Effect_Rock0:
            case EffectName.Effect_Rock1:
            case EffectName.Effect_Rock2:
                StartCoroutine(RockAnimation());
                break;
            //case EffectName.Effect_TreeDown:
            //    StartCoroutine(TreeDown());
            //    break;
        }

        if (Name.Equals(EffectName.Effect_Rock1))
        {
            EffectManager.Instance.Spawn(EffectName.Effect_Bomb0, new Vector3(14.1f, -6.55f, -852.1f));
            EffectManager.Instance.Spawn(EffectName.Effect_Bomb0, new Vector3(35.5f, -13.55f, -874.2f));
            EffectManager.Instance.Spawn(EffectName.Effect_Bomb1, new Vector3(68.8f, -10.65f, -859.9f));
        }
    }

    // 触发碎石特效
    IEnumerator RockAnimation()
    {
        Animation anim = transform.Find("Root").GetComponent<Animation>();
        anim.CrossFade("Take 001");
        float last = anim["Take 001"].length;
        yield return new WaitForSeconds(last);
        RemoveTrigger();
    }

    // 树木倒塌
    IEnumerator TreeDown()
    {
        Animator anim = GetComponent<Animator>();
        anim.SetBool("Down", true);
        yield return new WaitForSeconds(8);
        RemoveTrigger();
    }

    // 回收
    private void RemoveTrigger()
    {
        ioo.poolManager.DeSpawn(gameObject);
    }
}
