/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   CityTrigger.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/7/8 8:42:43
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;
using Need.Mx;

public class CityTrigger : MonoBehaviour
{
    public enum E_TriggerType
    {
        None,
        GuangGaoPai,            // 广告牌
        Friend,                 // 友军
        UFO,                    // UFO
        Enemy,                  // 敌机
        FriendEnd,              // 友机离开
        SwitchSkyBox,           // 切换天空盒
        Kill,                   // Boss必杀
        Door,                   // 隧道门
        Stone,                  // 陨石
        BreakPlane,             // 损坏敌机
        Warning,                // 警告
    }

    public E_TriggerType Type = E_TriggerType.None;

    public Transform Tran0;
    public Transform Tran1;

    public Material MT;

    public GameObject Door;

    public Animation AnimDoor;

    public bool KillBoss = false;

    #region Unity Call Back

    void OnTriggerEnter(Collider other)
    {
        if (!other.tag.Equals(GameTag.Player))
            return;

        switch(Type)
        {
            case E_TriggerType.GuangGaoPai:
                OnGuangGaoPai();
                break;
            case E_TriggerType.Enemy:
                OnEnemy();
                break;
            case E_TriggerType.Friend:
                OnFriend();
                break;
            case E_TriggerType.UFO:
                OnUFO();
                break;
            case E_TriggerType.FriendEnd:
                OnFriendEnd();
                break;
            case E_TriggerType.SwitchSkyBox:
                OnSwitchSkyBox();
                break;
            case E_TriggerType.Kill:
                OnKill();
                break;
            case E_TriggerType.Door:
                OnDoor();
                break;
            case E_TriggerType.Stone:
                OnStone();
                break;
            case E_TriggerType.BreakPlane:
                OnBreakPlane();
                break;
            case E_TriggerType.Warning:
                OnWarning();
                break;
        }
    }
   
    #endregion


    #region Private Function
    private void OnGuangGaoPai()
    {

    }

    private void OnRobot()
    {

    }

    private void OnWarning()
    {
        EventDispatcher.TriggerEvent(EventDefine.Event_Boss_Warning);
        ioo.audioManager.PlayPersonSound("Person_Sound_Warn_City");
    }

    private void OnBreakPlane()
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    private void OnStone()
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    private void OnDoor()
    {
        ioo.audioManager.PlaySoundOnPoint("SFX_Sound_Door_Boom", Door.transform.position);
        Door.SetActive(true);
        AnimDoor["Take 001"].speed = 1;
        AnimDoor.CrossFade("Take 001");
        StartCoroutine(DelayToShow());
    }

    private void OnFriend()
    {
        if (KillBoss && ioo.gameMode.Boss != null)
            ioo.audioManager.PlayPersonSound("Person_Sound_Friend_Coming");

        for (int i = 0; i < transform.childCount; ++i)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    private void OnEnemy()
    {
        for (int i = 0; i < transform.childCount;++i )
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }


    private void OnUFO()
    {
        if (ioo.gameMode.Boss != null)
            return;

        for (int i = 0; i < transform.childCount; ++i)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
        //ioo.gameMode.SpawnCityBoss();
    }


    private void OnFriendEnd()
    {
        EventDispatcher.TriggerEvent<Vector3, Vector3>(EventDefine.Event_Friend_Leave, Tran0.position, Tran1.position);
    }

    private void OnKill()
    {
        bool kill = ioo.gameMode.Boss == null ? false : true;
        EventDispatcher.TriggerEvent<bool>(EventDefine.Event_Boss_Must_Dead, kill);
    }

    private void  OnSwitchSkyBox()
    {
        if (MT == null)
            return;

        RenderSettings.skybox = MT;
    }
    #endregion

    IEnumerator DelayToShow()
    {
        yield return new WaitForSeconds(5);
        AnimDoor["Take 001"].speed = -1;
        AnimDoor.CrossFade("Take 001");
    }

}
