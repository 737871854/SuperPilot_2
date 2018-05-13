/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   GorgeTrigger.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/7/14 8:37:45
 * 
 * 修改描述：   直接挂载场景，没必要动态产出和回收了
 * 
 */


using UnityEngine;
using System.Collections;
using Need.Mx;
using DG.Tweening;
using DG.Tweening.Core;

public class GorgeTrigger : MonoBehaviour
{
    public Transform Tran0;
    public Transform Tran1;

    public enum E_TriggerType
    {
        None,
        WaterShow,          // 水波跟随
        WaterHide,          // 水波消失
        Dolphin,            // 海豚挑起
        DustShow,           // 尘土更随
        DustHide,           // 尘土消失
        Stone,              // 巨石掉落
        EnemyPlane,         // 敌机
        Bee,                // 蜜蜂攻击
        Mine,               // 水雷爆炸
        Robot,              // 铁钳机器人
        Boss,
        Friend,             // 僚机
        Ice,                // 冰柱
        FriendEnd,          // 友机离开
        CameraClip400Start, 
        CameraClip400End,
        BossFly,
        BossEnd,
        SwitchSkyBox,           // 切换天空盒
        Dragon,
        DamagedPlane,
        Tree,
        Warning,
        Water,
        WaterEnd,
        Forest,
        Volcano,
        Winter,
        HideCity,
        ShowCity,
    }

    public E_TriggerType Type = E_TriggerType.None;

    //public GameObject BossCamera;

    public GameObject Boss;

    public Material MT;

    public Animation Anim;

    public GameObject Forest;

    public GameObject Dust;

    public bool KillBoss = false;

    public GameObject City;

    void OnTriggerEnter(Collider other)
    {
        if (!other.tag.Equals(GameTag.Player))
            return;

        switch(Type)
        {
            case E_TriggerType.WaterShow:
                OnWaterShow();
                break;
            case E_TriggerType.WaterHide:
                OnWaterHide();
                break;
            case E_TriggerType.Dolphin:
                OnDolphin();
                break;
            case E_TriggerType.DustShow:
                OnDustShow();
                break;
            case E_TriggerType.DustHide:
                OnDustHide();
                break;
            case E_TriggerType.Stone:
                OnStone();
                break;
            case E_TriggerType.EnemyPlane:
                OnEnemyPlane();
                break;
            case E_TriggerType.Bee:
                OnBee();
                break;
            case E_TriggerType.Mine:
                OnMine();
                break;
            case E_TriggerType.Robot:
                OnRobot();
                break;
            case E_TriggerType.Boss:
                OnBoss();
                break;
            case E_TriggerType.Friend:
                OnFriend();
                break;
            case E_TriggerType.Ice:
                OnIce();
                break;
            case E_TriggerType.FriendEnd:
                OnFriendEnd();
                break;
            case E_TriggerType.CameraClip400Start:
                OnCamera400Start();
                break;
            case E_TriggerType.CameraClip400End:
                OnCamera400End();
                break;
            case E_TriggerType.BossFly:
                OnBossFly();
                break;
            case E_TriggerType.BossEnd:
                OnBossEnd();
                break;
            case E_TriggerType.SwitchSkyBox:
                OnSwitchSkyBox();
                break;
            case E_TriggerType.Dragon:
                OnDragon();
                break;
            case E_TriggerType.DamagedPlane:
                OnDamagedPlane();
                break;
            case E_TriggerType.Tree:
                OnTree();
                break;
            case E_TriggerType.Warning:
                OnWarning();
                break;
            case E_TriggerType.Water:
                OnWater(true);
                break;
            case E_TriggerType.WaterEnd:
                OnWater(false);
                break;
            case E_TriggerType.Forest:
                OnForest();
                break;
            case E_TriggerType.Volcano:
                OnVolcano();
                break;
            case E_TriggerType.Winter:
                OnWinter();
                break;
            case E_TriggerType.HideCity:
                OnHideCity();
                break;
            case E_TriggerType.ShowCity:
                OnShowCity();
                break;
        }
    }

    private void OnTree()
    {
        Animator animator = transform.GetChild(0).GetComponent<Animator>();
        animator.SetBool("Down", true);
        StartCoroutine(TreeToNormal(animator));
    }

    private void OnDamagedPlane()
    {
        for (int i = 0; i < transform.childCount; ++i)
            transform.GetChild(i).gameObject.SetActive(true);
    }

    private void OnDragon()
    {
        for (int i = 0; i < transform.childCount; ++i)
            transform.GetChild(i).gameObject.SetActive(true);
    }

    private void OnWarning()
    {
        EventDispatcher.TriggerEvent(EventDefine.Event_Boss_Warning);
        ioo.audioManager.PlayPersonSound("Person_Sound_Warn_Gorge");
    }

    private void OnWater(bool value)
    {
        EventDispatcher.TriggerEvent(EventDefine.Event_Enter_Follow_Water, value);
    }

    private void OnForest()
    {
        ioo.gameMode.ChangeEnviroment(GameMode.Enviroment.Forest);
    }

    private void OnVolcano()
    {
        ioo.gameMode.ChangeEnviroment(GameMode.Enviroment.Volcano);
    }

    private void OnWinter()
    {
        ioo.gameMode.ChangeEnviroment(GameMode.Enviroment.Winter);
    }

    private void OnHideCity()
    {
        City.SetActive(false);
    }

    private void OnShowCity()
    {
        City.SetActive(true);
    }

    private void OnSwitchSkyBox()
    {
        if (MT == null)
            return;

        RenderSettings.skybox = MT;
    }

    private void OnBossEnd()
    {
        Forest.SetActive(true);
        if (ioo.gameMode.Boss != null && !ioo.gameMode.Boss.IsDead)
        {
            EventDispatcher.TriggerEvent(EventDefine.Event_Gorge_Boss_Animation, false);
            Boss.SetActive(false);
            ioo.gameMode.Boss = null;
        }
    }

    private void OnBossFly()
    {
        EventDispatcher.TriggerEvent(EventDefine.Event_Gorge_Boss_Animation, true);

        Forest.SetActive(false);
    }

    private void OnCamera400End()
    {
        Camera.main.farClipPlane = 800;
    }

    private void OnCamera400Start()
    {
        Camera.main.farClipPlane = 250;
    }

    private void OnFriendEnd()
    {
        EventDispatcher.TriggerEvent<Vector3, Vector3>(EventDefine.Event_Friend_Leave, Tran0.position, Tran1.position);
    }

    /// <summary>
    /// 显示水花特效
    /// </summary>
    private void OnWaterShow()
    {
        EventDispatcher.TriggerEvent(EventDefine.Event_Trigger_Effect_Water, true);
    }

    /// <summary>
    /// 回收水花特效
    /// </summary>
    private void OnWaterHide()
    {
        EventDispatcher.TriggerEvent(EventDefine.Event_Trigger_Effect_Water, false);
    }

    /// <summary>
    /// 海豚跳
    /// </summary>
    private void OnDolphin()
    {
        for (int i = 0; i < transform.childCount; ++i)
            transform.GetChild(i).gameObject.SetActive(true);

        DOVirtual.DelayedCall(4, delegate 
        {
            for (int i = 0; i < transform.childCount; ++i)
                transform.GetChild(i).gameObject.SetActive(false);
        });
    }

    /// <summary>
    /// 扬尘显示
    /// </summary>
    private void OnDustHide()
    {
        EventDispatcher.TriggerEvent(EventDefine.Event_Trigger_Effect_Dust, false);
    }

    /// <summary>
    /// 扬尘隐藏
    /// </summary>
    private void OnDustShow()
    {
        EventDispatcher.TriggerEvent(EventDefine.Event_Trigger_Effect_Dust, true);
    }

    /// <summary>
    ///  碎石特效
    /// </summary>
    private void OnStone()
    {
        Anim["Take 001"].speed = 1;
        Anim.CrossFade("Take 001");
        float last = Anim["Take 001"].length;
        Dust.SetActive(true);
        StartCoroutine(DisableObj(last));
        ioo.audioManager.PlaySoundOnPoint("SFX_Sound_Stone_Crush", Anim.transform.position);
    }

    private void OnIce()
    {
        ioo.audioManager.PlaySoundOnPoint("SFX_Sound_Ice", transform.position);
        Anim["Take 001"].speed = 1;
        Anim.CrossFade("Take 001");
        float last = Anim["Take 001"].length;
        StartCoroutine(DisableObj(last));
        EventDispatcher.TriggerEvent(EventDefine.Event_Trigger_Ice);
    }

    private void OnFriend()
    {
        if (KillBoss && ioo.gameMode.Boss != null)
            ioo.audioManager.PlayPersonSound("Person_Sound_Friend_Coming");

        for (int i = 0; i < transform.childCount; ++i)
            transform.GetChild(i).gameObject.SetActive(true);
    }

    private void OnBoss()
    {
        Boss.SetActive(true);
    }

    private void OnRobot()
    {
    }

    private void OnMine()
    {
        OnCamera400Start();
        for (int i = 0; i < transform.childCount; ++i)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    private void OnBee()
    {
    }

    private void OnEnemyPlane()
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    //IEnumerator CameraEnd()
    //{
    //    yield return new WaitForSeconds(2);
    //    BossCamera.SetActive(false);
    //}

    IEnumerator DisableObj(float time)
    {
        yield return new WaitForSeconds(time * 5);

        Anim["Take 001"].speed = -1;
        Anim.CrossFade("Take 001");
    }

    IEnumerator TreeToNormal(Animator animator)
    {
        yield return new WaitForSeconds(1);
        ioo.gameMode.Player.ShackCameraByPosition(0.3f);
        yield return new WaitForSeconds(10);
        animator.SetBool("Down", false);
    }

}
