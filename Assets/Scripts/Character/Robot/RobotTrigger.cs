/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   RobotTrigger.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/7/8 16:27:13
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using Need.Mx;

public class RobotTrigger : MonoBehaviour
{
    public RobotBehaviour Robot;

    [System.NonSerialized]
    public bool GetTarget;

    private bool HasAttack;

    void OnEnable()
    {
        HasAttack = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(GameTag.Player) && !HasAttack)
        {
            if (ioo.gameMode.Player.ShieldLife == 0)
            {
                Robot.Hide();
                GetTarget = true;
                EventDispatcher.TriggerEvent(EventDefine.Event_Camera_State_FirstOrThird, Player.E_CameraState.FirstView);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals(GameTag.Player))
        {
            GetTarget = false;
        }
    }
}
