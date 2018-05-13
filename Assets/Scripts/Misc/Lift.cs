/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   Lift.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/7/6 17:03:47
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;
using Need.Mx;

public class Lift : MonoBehaviour
{
    public Transform FllowTran;

    private enum E_State
    {
        None,
        Up,
        Down,
    }

    private E_State State = E_State.None;

    private Animator Anim;

    private bool Hold;
    private bool CanUpdate;

    // Use this for initialization
    void Start()
    {
        Anim = GetComponent<Animator>();

        EventDispatcher.AddEventListener(EventDefine.Event_Lift_Up,OnUp);
        EventDispatcher.AddEventListener(EventDefine.Event_Lift_Down, OnDown);
    }

    private void OnDown()
    {
        Hold = true;
        CanUpdate = true;
        State = E_State.Down;
    }

    private void OnUp()
    {
        Hold = true;
        CanUpdate = true;
        State = E_State.Up;
        ioo.audioManager.PlayBackMusic("Music_Player_Lift");
    }

    void Destroy()
    {
        EventDispatcher.RemoveEventListener(EventDefine.Event_Lift_Up, OnUp);
        EventDispatcher.RemoveEventListener(EventDefine.Event_Lift_Down, OnDown);
    }


    void Update()
    {
        if (!CanUpdate)
            return;

        AnimatorStateInfo info = Anim.GetCurrentAnimatorStateInfo(0);
        switch (State)
        {
            case E_State.None:
                break;
            case E_State.Up:
                if (!info.IsName("Up"))
                {
                    Anim.SetInteger("State", 0);
                }
                else
                {
                    if (info.normalizedTime >= 1.0f)
                    {
                        Hold = false;
                        CanUpdate = false;
                        EventDispatcher.TriggerEvent(EventDefine.Event_Lift_End);
                        ioo.audioManager.StopBackMusic("Music_Player_Lift");
                    }
                }
                break;
            case E_State.Down:
                if (!info.IsName("Down"))
                {
                    Anim.SetInteger("State", 1);
                }
                else
                {
                    if (info.normalizedTime >= 1.0f)
                    {
                        Hold = false;
                        CanUpdate = false;
                        //EventDispatcher.TriggerEvent(EventDefine.Event_Lift_End);
                        ToSummary();
                    }
                }
                break;
        }

        if (Hold)
        {
            ioo.gameMode.Player.transform.position = FllowTran.position;
        }
    }

    /// <summary>
    /// 跳转结算
    /// </summary>
    private void ToSummary()
    {
        ioo.gameMode.RunMode(GameState.Summary);
        UIManager.Instance.OpenUI(EnumUIType.PanelSummary);
        ioo.audioManager.StopAll();
    }
}
