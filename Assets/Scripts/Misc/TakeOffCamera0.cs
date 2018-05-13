/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   TakeOffCamera0.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/8/10 17:18:29
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;
using Need.Mx;
using DG.Tweening;
using DG.Tweening.Core;

public class TakeOffCamera0 : MonoBehaviour
{
    public Transform TargetCameraTran;
    // Use this for initialization
    void Start()
    {
        EventDispatcher.AddEventListener(EventDefine.Event_Lift_End, OnLiftEnd);
    }

    void Destroy()
    {
        EventDispatcher.AddEventListener(EventDefine.Event_Lift_End, OnLiftEnd);
    }

    private void OnLiftEnd()
    {
        //Sequence sequence = DOTween.Sequence();
        //sequence.Append(transform.DOMove(TargetCameraTran.position, 0.5f));
        //sequence.Join(transform.DOLocalRotate(TargetCameraTran.localEulerAngles, 0.5f));
        transform.position = TargetCameraTran.position;
        transform.localEulerAngles = TargetCameraTran.localEulerAngles;
    }
}
