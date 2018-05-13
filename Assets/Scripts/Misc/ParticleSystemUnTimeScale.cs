/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   ParticleSystemUnTimeScale.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/9/9 15:56:38
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleSystemUnTimeScale : MonoBehaviour
{
    private ParticleSystem ps;
    public Animator animator;

    void Start()
    {
        ps = transform.GetComponent<ParticleSystem>();
        if (animator != null)
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    // Update is called once per frame
    void Update()
    {
        ps.Simulate(Time.unscaledDeltaTime, true, false);
    }
}
