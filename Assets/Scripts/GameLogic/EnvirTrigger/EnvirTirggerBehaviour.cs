/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   EnvirTirgger.cs
 * 
 * 简    介:    绑定到场景触发器上，去触发相应的事件
 * 
 * 创建标识：   Pancake 2017/6/1 11:44:41
 * 
 * 修改描述： 废弃 (为了方便策划随时修改触发点，废弃该方法，改为直接在场景加触发点)
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using Need.Mx;
using System.Collections;

public class EnvirTirggerBehaviour : MonoBehaviour
{
    public enum ETriggerType
    {
        None,

        /// <summary>
        ///  水波跟随
        /// </summary>
        WaterShow,
        /// <summary>
        /// 水波消失
        /// </summary>
        WaterHide,
        /// <summary>
        /// 鲸鱼翻身
        /// </summary>
        Whale,
        /// <summary>
        /// 第一人称视角
        /// </summary>
        FirstPerson,
        /// <summary>
        /// 第三让人称视角
        /// </summary>
        ThirdPerson,
        /// <summary>
        /// 杨尘显示
        /// </summary>
        DustShow,
        /// <summary>
        /// 杨尘隐藏
        /// </summary>
        DustHide,

        Bomb0,
        Bomb1,

        // 碎石
        Rock0,
        Rock1,
        Rock2,
        /// <summary>
        /// 树倒下
        /// </summary>
        TreeDown,
        /// <summary>
        /// 森林着火
        /// </summary>
        ForestFire,
        /// <summary>
        /// Boss出现
        /// </summary>
        BossShow,
        BossHide,

        /// <summary>
        /// 落石
        /// </summary>
        Stone,
      
        /// <summary>
        /// QTE
        /// </summary>
        QTE0,           // 右侧身飞过
        QTE1,           // 快速拉升
        QTE2,           // 左侧身飞过
        QTE3,           // 旋转飞过
        QTE4,           // 俯冲飞过
        QTE5,           // 躲避爆炸
    }

    public ETriggerType type = ETriggerType.None;

    public Vector3 Pos = Vector3.zero;

    public Vector3 EulerAngle = Vector3.zero;

    private OnTriggerBehaviour OTB = null;

    void Awake()
    {
        EventDispatcher.AddEventListener(EventDefine.Event_Game_Reset, InitEnvir);
        EventDispatcher.AddEventListener(EventDefine.Event_Loading_End, InitEnvir);
    }

    void Destroy()
    {
        EventDispatcher.RemoveEventListener(EventDefine.Event_Game_Reset, InitEnvir);
        EventDispatcher.RemoveEventListener(EventDefine.Event_Loading_End, InitEnvir);
    }

    void InitEnvir()
    {
        string name = string.Empty;
        switch(type)
        {
            #region 碎石
            case ETriggerType.Rock0:
                name = EffectName.Effect_Rock0;
                break;
            case ETriggerType.Rock1:
                name = EffectName.Effect_Rock1;
                break;
            case ETriggerType.Rock2:
                name = EffectName.Effect_Rock2;
                break;
            #endregion
      
            #region 树木
            case ETriggerType.TreeDown:
                //name = EffectName.Effect_TreeDown;
                break;
            #endregion
        }

        if (string.IsNullOrEmpty(name))
            return;

        GameObject obj  = ioo.poolManager.Spawn(name);
        OTB             = obj.GetOrAddComponent<OnTriggerBehaviour>();
        OTB.Name        = name;
        OTB.transform.position          = new Vector3(Pos.x, Pos.y, Pos.z);
        OTB.transform.localEulerAngles  = new Vector3(EulerAngle.x, EulerAngle.y, EulerAngle.z);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.tag.Equals(GameTag.Player))
            return;

        // 碎石， 树木
        if (OTB != null)
        {
            OTB.OnTrigger();
            return;
        }

        switch(type)
        {
            case ETriggerType.WaterShow:    // 显示水特效
                EventDispatcher.TriggerEvent(EventDefine.Event_Trigger_Effect_Water, true);
                break;
            case ETriggerType.WaterHide:    // 回收水特效
                EventDispatcher.TriggerEvent(EventDefine.Event_Trigger_Effect_Water, false);
                break;
            case ETriggerType.DustShow:     // 显示杨尘
                EventDispatcher.TriggerEvent(EventDefine.Event_Trigger_Effect_Dust, true);
                break;
            case ETriggerType.DustHide:     // 隐藏杨尘
                EventDispatcher.TriggerEvent(EventDefine.Event_Trigger_Effect_Dust, false);
                break;
            case ETriggerType.Stone:        // 落石
                StartCoroutine(Stone());
                break;
            case ETriggerType.Bomb0:        // 炸弹 0
                StartCoroutine(Bomb(EffectName.Effect_Bomb0));
                break;
            case ETriggerType.Bomb1:        // 炸弹 1
                StartCoroutine(Bomb(EffectName.Effect_Bomb1));
                break;
            case ETriggerType.BossShow:     // Boss出现
                //ioo.gameMode.SpawnGorgeBoss();
                break;
            case ETriggerType.BossHide:     // Boss回收
                //ioo.gameMode.DespawnBoss();
                break;
            case ETriggerType.ForestFire:   // 森林着火
                break;

        }
    }

    // 落石
    IEnumerator Stone()
    {
        GameObject stone            = ioo.poolManager.Spawn(EffectName.Effect_Stone);
        stone.transform.position    = Pos;
        stone.transform.localEulerAngles = EulerAngle;
        yield return new WaitForSeconds(3);
        ioo.poolManager.DeSpawn(stone);
    }

    // 炸弹
    IEnumerator Bomb(string name)
    {
        GameObject bomb         = ioo.poolManager.Spawn(name);
        bomb.transform.position = Pos;
        bomb.transform.localEulerAngles = EulerAngle;    
        yield return new WaitForSeconds(3);
        ioo.poolManager.DeSpawn(bomb);
    }
}
