/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   AnimController.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/7/6 8:55:49
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using Need.Mx;

public class AnimController : MonoBehaviour
{
    private Animator Anim;
    private Player Player;

    public enum E_State
    {
        Up      = 0,
        TakeOff = 1,
        Fly     = 2,
        Land    = 3,
        Hurt    = 4,
        Down    = 5,
    }

    [System.NonSerialized]
    public E_State State = E_State.Up;

    private bool IsIdle;

    // 飞行特效
    public GameObject[] EffectIdleArray;

    private GameObject[] EffectTuoWei;

    void Start()
    {
        IsIdle = true;
        Player = GetComponent<Player>();
        Anim = Player.HelicopterBody.GetComponent<Animator>();

        EffectIdleArray = GameObject.FindGameObjectsWithTag(GameTag.EffectIdle);
        EffectTuoWei = GameObject.FindGameObjectsWithTag(GameTag.TuoWei);

        DisableEffectIdle();

    }

    void Update()
    {
        switch (State)
        {
            case E_State.Up:
                OnUp();
                break;
            case E_State.TakeOff:
                OnTakeOff();
                break;
            case E_State.Fly:
                OnFly();
                break;
            case E_State.Land:
                OnLand();
                break;
            case E_State.Down:
                OnDown();
                break;
            case E_State.Hurt:
                OnHurt();
                break;
        }
    }

    #region Private Function
    private void OnUp()
    {
        AnimatorStateInfo info = Anim.GetCurrentAnimatorStateInfo(0);
        if (!info.IsName(E_State.Up.ToString()))
        {
            Anim.SetInteger("State", (int)State);
        }
    }

    private void OnTakeOff()
    {
        AnimatorStateInfo info = Anim.GetCurrentAnimatorStateInfo(0);
        if (!info.IsName(E_State.TakeOff.ToString()))
        {
            Anim.SetInteger("State", (int)State);
        }
        else
        {
            if (info.normalizedTime >= 0.8f)
                ToFly();
        }
    }

    private void OnFly()
    {
        AnimatorStateInfo info = Anim.GetCurrentAnimatorStateInfo(0);
        if (!info.IsName(E_State.Fly.ToString()))
        {
            Anim.SetInteger("State", (int)State);
        }
    }

    private void OnLand()
    {
        AnimatorStateInfo info = Anim.GetCurrentAnimatorStateInfo(0);
        if (!info.IsName(E_State.Land.ToString()))
        {
            Anim.SetInteger("State", (int)State);
        }
        else
        {
            if (info.normalizedTime >= 1.0f)
                ToDown();
        }

        if (EffectTuoWei != null)
        {
            for (int i = 0; i < EffectTuoWei.Length;++i )
            {
                EffectTuoWei[i].SetActive(false);
            }
        }
    }

    private void OnHurt()
    {
        AnimatorStateInfo info = Anim.GetCurrentAnimatorStateInfo(0);
        if (!info.IsName(E_State.Hurt.ToString()))
        {
            Anim.SetInteger("State", (int)State);
        }
      
    }

    private void OnDown()
    {
        //AnimatorStateInfo info = Anim.GetCurrentAnimatorStateInfo(0);
        //if (!info.IsName(E_State.Down.ToString()))
        //{
        //    Anim.SetInteger("State", (int)State);
        //}
        //else
        //{
        //    //if (info.normalizedTime >= 1.0f)
        //    //    ToIdle();
        //}
    }

    private void DisableEffectIdle()
    {
        for (int i = 0; i < EffectIdleArray.Length; ++i)
        {
            EffectIdleArray[i].SetActive(false);
        }
    }

    private void ActiveEffectIdle()
    {
        for (int i = 0; i < EffectIdleArray.Length; ++i)
        {
            EffectIdleArray[i].SetActive(true);
        }
    }
    #endregion

    #region Public Function

    private string TakeOff_Music = string.Empty;
    public void ToTakeOff()
    {
        IsIdle = false;
        State = E_State.TakeOff;
        ActiveEffectIdle();

        switch (ioo.gameMode.PlayerName)
        {
            case "Player0":
                TakeOff_Music = "Music_Player0_TakeOff";
                break;
            case "Player1":
                TakeOff_Music = "Music_Player0_TakeOff";
                break;
            case "Player2":
                TakeOff_Music = "Music_Player2_TakeOff";
                break;
        }

        ioo.audioManager.PlayBackMusic(TakeOff_Music);
    }

    public void ToFly()
    {
        State = E_State.Fly;
        ioo.gameMode.PlayGameMusic();
        ioo.gameMode.PlayPlaneMusic();
        ioo.audioManager.StopBackMusic(TakeOff_Music);
    }

    public void ToLand()
    {
        State = E_State.Land;
        DisableEffectIdle();
    }

    public void ToDown()
    {
        State = E_State.Down;
        EventDispatcher.TriggerEvent(EventDefine.Event_Lift_Down);
    }

    public void OnNormalFly()
    {
        State = E_State.Fly;
    }

    public void OnHurtFly()
    {
        State = E_State.Hurt;
    }
    #endregion
}
