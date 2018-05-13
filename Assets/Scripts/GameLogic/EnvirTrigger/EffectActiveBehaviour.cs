/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   EffectActiveBehaviour.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/7/4 8:39:29
 * 
 * 修改描述：废弃 (为了方便策划随时修改触发点，废弃该方法，改为直接在场景加触发点)
 * 
 */


using UnityEngine;
using System.Collections;

public class EffectActiveBehaviour : MonoBehaviour
{
    public bool Active = false;
    // Use this for initialization
   
    void OnTriggerEnter(Collider other)
    {
        if (!other.tag.Equals(GameTag.Player))
            return;

        if (Active)
        {
            PathEffect.Instance.StartPathEffect();
            return;
        }

        PathEffect.Instance.StopPathEffect();
    }
}
