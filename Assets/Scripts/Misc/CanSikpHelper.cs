/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   CanSikpHelper.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/9/1 10:49:15
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using Need.Mx;

public class CanSikpHelper : MonoBehaviour
{
    private bool CanSkip;
    private Animator Anim;
    private float Timer;
    private bool HasBegine;
    private GameObject Effect_Please;

    void Start()
    {
        Anim = GetComponent<Animator>();
        Effect_Please = transform.Find("Press_Please/Effect_Press_Please").gameObject;
        EventDispatcher.AddEventListener(EventDefine.Event_Skip_Describe, Skip);
    }

    /// <summary>
    /// 跳过游戏说明
    /// </summary>
    public void CanSkipDescribe()
    {
        CanSkip = true;
    }

    /// <summary>
    /// 游戏正式开始
    /// </summary>
    public void CanBeginGame()
    {
        HasBegine = true;
        ioo.gameMode.Player.GameBegine();
        EventDispatcher.RemoveEventListener(EventDefine.Event_Skip_Describe, Skip);
    }

    private void Skip()
    {
        if (!CanSkip)
            return;

        Anim.SetBool("Flag", true);
        Effect_Please.SetActive(true);
    }

    void Update()
    {
        if (HasBegine)
            return;

        Timer += Time.deltaTime;
        if (Timer > 8)
        {
            HasBegine = true;
            CanSkipDescribe();
            Skip();
        }
    }
}
