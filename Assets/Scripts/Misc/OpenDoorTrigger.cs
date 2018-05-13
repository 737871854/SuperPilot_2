/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   OpenDoorTrigger.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/6/30 14:04:16
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using System.Collections;
using Need.Mx;

public class OpenDoorTrigger : MonoBehaviour
{
    public Transform LeftDoor;
    public Transform RightDoor;

    public Vector3 LeftTarget;
    public Vector3 RightTarget;

    public GameObject Camera0;
    public GameObject Camera1;

    public GameObject Effect;

    public Material MT;

    private Vector3 LeftOrg;
    private Vector3 RightOrg;

    void Start()
    {
        LeftOrg = LeftDoor.localPosition;
        RightOrg = RightDoor.localPosition;

        EventDispatcher.AddEventListener<bool>(EventDefine.Event_On_Describe, OnGameStart);
    }

    void Destroy()
    {
        EventDispatcher.RemoveEventListener<bool>(EventDefine.Event_On_Describe, OnGameStart);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.tag.Equals(GameTag.Player))
            return;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(LeftDoor.transform.DOLocalMove(LeftTarget, 1));
        sequence.Join(RightDoor.transform.DOLocalMove(RightTarget, 1));
        DOVirtual.DelayedCall(0.2f, delegate 
        {
            if (ioo.gameMode.State == GameState.Back)
                return;
            Camera0.SetActive(false);
            Camera1.SetActive(true);
            ioo.gameMode.Player.CameraObj.SetActive(false);
        });

        Effect.SetActive(false);

        ioo.audioManager.PlaySound2D("SFX_Sound_Open_Door");
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.tag.Equals(GameTag.Player) || ioo.gameMode.State == GameState.Back)
            return;

        StartCoroutine(CloseDoor());
    }

    IEnumerator CloseDoor()
    {
        yield return new WaitForSeconds(1);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(LeftDoor.transform.DOLocalMove(LeftOrg, 1));
        sequence.Join(RightDoor.transform.DOLocalMove(RightOrg, 1));
    }

    private void OnGameStart(bool value)
    {
        if (!value)
        {
            Camera1.SetActive(false);
        }
    }

    void Update()
    {
        if (MT == null)
            return;

        if (ioo.gameMode.State == GameState.Back)
        {
            if (RenderSettings.skybox != MT)
                RenderSettings.skybox = MT;
        }
    }
}
