/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   QiTaEffect.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/8/7 19:27:11
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;

public class QiPaEffect : MonoBehaviour
{

    public Animator Anim;
    public int LoopTimes = 1;
    // Update is called once per frame
    void Update()
    {
        AnimatorStateInfo info = Anim.GetCurrentAnimatorStateInfo(0);
        if (info.normalizedTime >= LoopTimes)
        {
            gameObject.SetActive(false);
        }
    }
}
