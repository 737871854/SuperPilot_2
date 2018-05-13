/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   FirstCameraView.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/7/10 10:12:36
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Need.Mx;
using DG.Tweening;
using DG.Tweening.Core;

public class FirstCameraView : MonoBehaviour
{
    public GameObject Robot;
    public GameObject Effect;
    public Transform HurtTran;

    private Vector3 OrgPos;
    private bool ToAttack;
    private float Timer;

    void Start()
    {
        OrgPos = Robot.transform.localPosition;
    }

    public void ActiveRobot()
    {
        Robot.SetActive(true);
        ToAttack    = true;
        Timer       = 0;
        Step        = 0;
        HasLeave    = false;
        //StartCoroutine(ToExplode());
        ioo.audioManager.PlayBackMusic("SFX_Sound_Robot_Attack", false);
    }

    //IEnumerator ToExplode()
    //{
    //    yield return new WaitForSeconds(0.3f);
    //    EventDispatcher.TriggerEvent(EventDefine.Event_Tips_Type_By_Int, 0, HurtTran.position, 1);
    //    yield return new WaitForSeconds(0.3f);
    //    EventDispatcher.TriggerEvent(EventDefine.Event_Tips_Type_By_Int, 0, HurtTran.position, 1);
    //    yield return new WaitForSeconds(0.3f);
    //    EventDispatcher.TriggerEvent(EventDefine.Event_Tips_Type_By_Int, 0, HurtTran.position, 1);
    //    yield return new WaitForSeconds(1.1f);

    //    if (ioo.gameMode.Player.Data.Hurry <= 10)
    //    {
    //        ioo.gameMode.Player.ShackCameraByPosition(1);
    //        EventDispatcher.TriggerEvent(EventDefine.Event_Tips_Type_By_Int, 0, HurtTran.position, 2);
    //        Effect.SetActive(true);
    //        Robot.SetActive(false);
    //        StartCoroutine(ToNormal());
    //        EventDispatcher.TriggerEvent(EventDefine.Event_Hurry_ShowOrHide, false);
    //    }
    //    else
    //    {
    //        Robot.transform.DOLocalMoveY(-0.8f, 0.8f).OnComplete(OnMoveEnd);
    //        StartCoroutine(ToNormal());
    //    }
    //}

    IEnumerator ToNormal()
    {
        yield return new WaitForSeconds(2);
        Effect.SetActive(false);
        EventDispatcher.TriggerEvent(EventDefine.Event_Camera_State_FirstOrThird, Player.E_CameraState.ThirdView);
    }

    private void OnMoveEnd()
    {
        Robot.SetActive(false);
        Robot.transform.localPosition = OrgPos;
        EventDispatcher.TriggerEvent(EventDefine.Event_Hurry_ShowOrHide, false);
    }

    private int Step;
    private bool HasLeave;
    void Update()
    {
        if (ioo.gameMode.Player.Data.Hurry == 20 && !HasLeave)
        {
            ToAttack = false;
            HasLeave = true;
            Robot.transform.DOLocalMoveY(-0.8f, 0.8f).OnComplete(OnMoveEnd);
            StartCoroutine(ToNormal());
            ioo.audioManager.PlaySound2D("SFX_Sound_Success");
            ioo.audioManager.StopBackMusic("SFX_Sound_Robot_Attack");
        }

        if (!ToAttack)
            return;

        Timer += Time.deltaTime;
        if (Timer >= 0.3f && Timer < 0.6f && Step == 0)
        {
            ++Step;
            ioo.gameMode.Player.OnDamage(-1);
            EventDispatcher.TriggerEvent(EventDefine.Event_Tips_Type_By_Int, 0, HurtTran.position, 1);
        }else if (Timer >= 0.6f && Timer < 0.9f && Step == 1)
        {
            ++Step;
            ioo.gameMode.Player.OnDamage(-1);
            EventDispatcher.TriggerEvent(EventDefine.Event_Tips_Type_By_Int, 0, HurtTran.position, 1);
        }else if (Timer >= 0.9f && Timer < 2.0f && Step == 2)
        {
            ++Step;
            ioo.gameMode.Player.OnDamage(-1);
            EventDispatcher.TriggerEvent(EventDefine.Event_Tips_Type_By_Int, 0, HurtTran.position, 1);
        }
        else if (Timer > 2.0f && Step == 3)
        {
             ++Step;
             ToAttack = false;
             if (ioo.gameMode.Player.Data.Hurry <= 10)
             {
                ioo.gameMode.Player.ShackCameraByPosition(1);
                ioo.gameMode.Player.OnDamage(-2);
                EventDispatcher.TriggerEvent(EventDefine.Event_Tips_Type_By_Int, 0, HurtTran.position, 2);
                Effect.SetActive(true);
                Robot.SetActive(false);
                StartCoroutine(ToNormal());
                EventDispatcher.TriggerEvent(EventDefine.Event_Hurry_ShowOrHide, false);
            }
            else
            {
                HasLeave = true;
                Robot.transform.DOLocalMoveY(-0.8f, 0.8f).OnComplete(OnMoveEnd);
                StartCoroutine(ToNormal());
                ioo.audioManager.StopBackMusic("SFX_Sound_Robot_Attack");
            }
        }
    }
}
