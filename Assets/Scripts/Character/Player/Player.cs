/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   Player.cs
 * 
 * 简    介:    借鉴了一代飞机部分代码，负责飞机坐标点的控制
 * 
 * 创建标识：   Pancake 2017/5/25 14:21:46
 * 
 * 修改描述：   沿用了一代飞机驱动 (特效应该分开设计的，算了)
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Need.Mx;
using DG.Tweening;
using DG.Tweening.Core;

[RequireComponent(typeof(AnimController))]

public class Player : MonoBehaviour
{

    public enum E_Path
    {
        Ready,
        Game,
        Land,
    }
    
    public enum E_CameraState
    {
        ThirdView,
        FirstView,
    }

    public E_CameraState CameraState = E_CameraState.ThirdView;

    #region 定义变量
    // 速度控制脚本
    [NonSerialized]
    public PilotController PilotController;

    [NonSerialized]
    public AnimController AnimController;

    //机身
    [NonSerialized]
    public Transform HelicopterBody;

    // 吃道具特效挂点
    [NonSerialized]
    public Transform EffectPoint;

    // 水特效挂点
    [NonSerialized]
    public Transform WaterEffect;

    // 杨尘特效挂点
    [NonSerialized]
    public Transform DustEffect;

    [NonSerialized]
    public Transform BossPoint;
    [NonSerialized]
    public List<Transform> BossPathList = new List<Transform>();

    // 火龙Boss阶段
    //[NonSerialized]
    public bool BossStep;

    public PlayerData Data;

    public float ShieldLife;

    //private bool IsGamePath;
    public bool Pause;

    public GameObject CameraObj;
    private Camera PCamera;

    [NonSerialized]
    public GameObject BossCameraObj;
    private Transform ShowUITran;
    public Transform BossAttackPoint;
    public Vector3 ShowUIMoveDir;

    public E_Path PathType = E_Path.Ready;

    private bool CamerCanRotation;

    #region 射击
    [NonSerialized]
    public Transform FirePoint;
    public bool IsPress;
    private bool CanFire;
    private float FireTime;
    private float FireHotCoolTime;
    private bool CanMissile;
    private float MissileTime;
    #endregion

    #region 飞机驱动相关
    [NonSerialized]
    public PathInfo pathInfo;
    //当前路径点
    [NonSerialized]
    public Vector3 curPoint;
    //下一个路径点
    [NonSerialized]
    public Vector3 nextPoint;
    [NonSerialized]
    public Vector3 upPoint;
    //运行时间 
    private float timer;
    //步伐大小
    private const float Step = 0.0001f;
    //飞机运行到整个路径的百分比
    public float Percent;
    //跑完整个路径的时间
    [NonSerialized]
    public float OverTime = 10;

    /// <summary>
    ///  使用聚气
    /// </summary>
    private bool UsingGather;
    private bool CanUseGather;
    private bool ActiveGather;
    #endregion

    public FirstCameraView FirstCameraView;
    private Transform ThirdCameraView;

    // 特效单独用一个脚本管理，后期优化
    // 护盾特效
    private GameObject Effect_Shield;
    // 磁铁特效
    private GameObject Effect_Magent;
    // 飞行特效
    public GameObject[] EffectHurtArray;
    // 导弹挂点
    private GameObject[] MissilePoints;
    private GameObject RedMissile;
    private GameObject YellowMissile;
    private List<WeaponBehaviour> Missile = new List<WeaponBehaviour>();

    private GameObject Effect_Warning;

    // 加速特效
    public GameObject EffectSpeedUp;

    // 受损修复
    public GameObject EffectFixed;

    private float HurryTime;

    //速度数据，存储当前位置点速度信息
    public class SpeedData 
    {
        public float Distance;
        public float Time;

        public SpeedData(float distance, float time)
        {
            Distance = distance;
            Time = time;
        }
    }
    private List<SpeedData> distance = new List<SpeedData>();

    #endregion

    #region Unity CallBack
    void Awake()
    {
        HelicopterBody  = transform.Find("HeilcopterBody").transform;
        EffectPoint     = HelicopterBody.Find("EffectPoint").transform;
        WaterEffect     = transform.Find("WaterEffect").transform;
        DustEffect      = transform.Find("DustEffect").transform;
        BossPoint       = transform.Find("BossLocalPath").transform;
        FirePoint       = HelicopterBody.Find("FirePoint").transform;
        CameraObj       = transform.Find("Main Camera").gameObject;
        PCamera         = CameraObj.GetComponent<Camera>();
        PilotController = HelicopterBody.gameObject.GetOrAddComponent<PilotController>();
        AnimController  = GetComponent<AnimController>();

        BossCameraObj   = transform.Find("BossCamera").gameObject;
        FirstCameraView = transform.Find("FirstView").GetComponent<FirstCameraView>();
        ThirdCameraView = transform.Find("ThirdView").transform;
        ShowUITran      = BossCameraObj.transform.Find("ShowUI").transform;
        BossAttackPoint = BossCameraObj.transform.Find("BossAttackPoint").transform;

        Effect_Shield   = EffectPoint.transform.Find("Effect_Shield").gameObject;
        Effect_Magent   = EffectPoint.transform.Find("Effect_Magent").gameObject;
        EffectSpeedUp   = EffectPoint.transform.Find("Effect_SpeedUp").gameObject;
        Effect_Warning  = EffectPoint.transform.Find("Effect_Warning").gameObject;
        EffectFixed     = EffectPoint.transform.Find("Effect_Fixed").gameObject;

        EventDispatcher.AddEventListener<bool>(EventDefine.Event_Trigger_Effect_Water, OnWater);
        EventDispatcher.AddEventListener<bool>(EventDefine.Event_Trigger_Effect_Dust, OnDust);
        EventDispatcher.AddEventListener(EventDefine.Event_Lift_End, OnLiftEnd);
        EventDispatcher.AddEventListener<E_CameraState>(EventDefine.Event_Camera_State_FirstOrThird, ChangeCameraState);
        EventDispatcher.AddEventListener(EventDefine.Event_Player_Gather, UseGather);
        EventDispatcher.AddEventListener<bool>(EventDefine.Event_Gorge_Boss_Animation, OnGorgeBoss);
        EventDispatcher.AddEventListener(EventDefine.Event_Boss_Warning, OnWarning);
        EventDispatcher.AddEventListener(EventDefine.Event_Groge_Boss_Attack, OnGorgeAttack);
    }

    void Destroy()
    {
        EventDispatcher.RemoveEventListener<bool>(EventDefine.Event_Trigger_Effect_Water, OnWater);
        EventDispatcher.RemoveEventListener<bool>(EventDefine.Event_Trigger_Effect_Dust, OnDust);
        EventDispatcher.RemoveEventListener(EventDefine.Event_Lift_End, OnLiftEnd);
        EventDispatcher.RemoveEventListener<E_CameraState>(EventDefine.Event_Camera_State_FirstOrThird, ChangeCameraState);
        EventDispatcher.RemoveEventListener(EventDefine.Event_Player_Gather, UseGather);
        EventDispatcher.RemoveEventListener<bool>(EventDefine.Event_Gorge_Boss_Animation, OnGorgeBoss);
        EventDispatcher.RemoveEventListener(EventDefine.Event_Boss_Warning, OnWarning);
        EventDispatcher.RemoveEventListener(EventDefine.Event_Groge_Boss_Attack, OnGorgeAttack);
    }

    private float SpeedFactor;
    private float CameraToBody;
    private Vector3 StartPos = new Vector3(-0.08f, 6334.795f, -16.749f);
    void Start()
    {
        MissileTime         = 5;
        CamerCanRotation    = true;
        Data                = new PlayerData();
        for (int i = 0; i < BossPoint.transform.childCount; ++i)
        {
            BossPathList.Add(BossPoint.transform.GetChild(i).transform);
        }

        transform.position = StartPos;
        CameraToBody = CameraObj.transform.localPosition.y - HelicopterBody.localPosition.y;

        SpeedFactor = ioo.gameMode.Map == E_Map.City ? 1 : 0.7f;

        EventDispatcher.TriggerEvent(EventDefine.Event_Lift_Up);

        EffectHurtArray = GameObject.FindGameObjectsWithTag(GameTag.EffectHurt);
        MissilePoints = GameObject.FindGameObjectsWithTag(GameTag.MissilePoint);
        RedMissile = GameObject.FindGameObjectWithTag("RedMissile").gameObject;
        YellowMissile = GameObject.FindGameObjectWithTag("YellowMissile").gameObject;
        for (int i = 0; i < MissilePoints.Length; ++i )
        {
            MissilePoints[i].SetActive(false);
        }
        YellowMissile.SetActive(false);
        DisableEffectHurt();
    }

    // Update is called once per frame
    void Update()
    {
        #region Life
        if (PathType == E_Path.Game)
        {
            if (Data.LifeTime > 0)
            {
                Data.LifeTime -= ioo.nonStopTime.deltaTime;
            }
            else
            {
                Data.LifeTime = 0;
                ioo.gameMode.RunMode(GameState.Continue);
                PilotController.PlayToContinue();
            }

            #region Fire

            if (!IsPress)
            {
                FireHotCoolTime += ioo.nonStopTime.deltaTime;

                if (Data.Hot < 10)
                {
                    if (FireHotCoolTime >= 0.12f)
                    {
                        --Data.Hot;
                        FireHotCoolTime = 0;
                    }
                }
                else
                {
                    if (FireHotCoolTime >= 0.08f)
                    {
                        --Data.Hot;
                        FireHotCoolTime = 0;
                    }
                }
            }

            if (FireTime > 0)
                FireTime -= ioo.nonStopTime.deltaTime;
            else
            {
                CanFire = true;
                FireTime = 0.15f;
            }

            if (MissileTime > 0 && !CanMissile)
            {
                MissileTime -= ioo.nonStopTime.deltaTime;
            }
            else
            {
                if (!PilotController._AbilityMissile)
                {
                    MissilePoints[0].SetActive(true);
                    CanMissile = true;
                    MissileTime = 5;
                    RedMissile.SetActive(true);
                }
            }
            #endregion
        }

        #endregion      
    
        #region BossCamera
        if (BossStep)
        {
            if (ioo.gameMode.Boss != null)
            {
                //ShowUITran.localPosition += ShowUIMoveDir * Time.deltaTime * 2;
                float factor = 0;
                if (Time.timeScale != 1)
                    factor = ioo.nonStopTime.deltaTime;
                else
                    factor = Time.deltaTime;

                if (Pause)
                    ShowUITran.localPosition += ShowUIMoveDir * factor * 4;
                else
                    ShowUITran.localPosition += ShowUIMoveDir * factor * 4;
                EventDispatcher.TriggerEvent(EventDefine.Event_Lock_Pos, ShowUITran.position);
                ShowUIMoveDir = Vector3.zero;

                if (ShowUITran.localPosition.x > 2.8f)
                {
                    ShowUITran.localPosition = new Vector3(2.5f, ShowUITran.localPosition.y, ShowUITran.localPosition.z);
                }

                if (ShowUITran.localPosition.x < -2.8f)
                {
                    ShowUITran.localPosition = new Vector3(-2.5f, ShowUITran.localPosition.y, ShowUITran.localPosition.z);
                }

                if (ShowUITran.localPosition.y > 2)
                {
                    ShowUITran.localPosition = new Vector3(ShowUITran.localPosition.x, 2, ShowUITran.localPosition.z);
                }

                if (ShowUITran.localPosition.y < -2)
                {
                    ShowUITran.localPosition = new Vector3(ShowUITran.localPosition.x, -2, ShowUITran.localPosition.z);
                }
            }
        }
        else
            ShowUITran.transform.localPosition = Vector3.forward * 4;
        #endregion

        if (!CameraIsMoving && CameraState == E_CameraState.ThirdView)
        {
            if (CameraSpeedUp && !CameraSpeedDown)
            {
                if (PCamera.fieldOfView > 90)
                {
                    CameraSpeedUp = false;
                    PCamera.fieldOfView = 90;
                }
                else
                    PCamera.fieldOfView += ioo.nonStopTime.deltaTime * 30;
                //if (CameraObj.transform.localPosition.z <= -18)
                //{
                //    CameraSpeedUp = false;
                //    CameraObj.transform.localPosition = new Vector3(CameraObj.transform.localPosition.x, CameraObj.transform.localPosition.y, -18);
                //}
                //else
                //    CameraObj.transform.localPosition -= Vector3.forward * ioo.nonStopTime.deltaTime * 20;
            }
            else if (!CameraSpeedUp && CameraSpeedDown)
            {
                if (PCamera.fieldOfView < 45)
                {
                    CameraSpeedDown = false;
                    PCamera.fieldOfView = 45;
                }
                else
                    PCamera.fieldOfView -= ioo.nonStopTime.deltaTime * 30;

                //if (CameraObj.transform.localPosition.z >= -10.42f)
                //{
                //    CameraSpeedDown = false;
                //    CameraObj.transform.localPosition = new Vector3(CameraObj.transform.localPosition.x, CameraObj.transform.localPosition.y, -10.42f);
                //}
                //else
                //    CameraObj.transform.localPosition += Vector3.forward * ioo.nonStopTime.deltaTime * 10;
            }
        }

        if (Pause)
            return;

        if (AnimController.State == global::AnimController.E_State.Up || 
            AnimController.State == global::AnimController.E_State.Land || 
            AnimController.State == global::AnimController.E_State.Down)
        {
            return;
        }

        #region 飞机驱动
        aircraftEngine();
        #endregion

        if (ioo.gameMode.State != GameState.Play)
            return;

       
        #region 聚气
        if (ActiveGather)
        {
            if (UsingGather)
            {
                if (Data.Gather > 0)
                    Data.Gather -= ioo.nonStopTime.deltaTime * 0.2f;
                else
                {
                    Data.Gather = 0;
                    UsingGather = false;
                }
            }
            else
            {
                Data.Gather += ioo.nonStopTime.deltaTime * 0.05f;
               
                if (Data.Gather >= 0.2f)
                {
                    CanUseGather = true;
                    if (Data.Gather >= 1)
                        Data.Gather = 1;
                }
               
            }
        }
        #endregion


        // 水的特效
        WaterEffect.transform.position = new Vector3(HelicopterBody.position.x, 2, HelicopterBody.position.z) + HelicopterBody.forward * 6;

        #region 挣扎
        if (HurryTime < 0.2f)
        {
            HurryTime += ioo.nonStopTime.deltaTime;
        }
        else
        {
            HurryTime = 0;
            Data.Hurry -= 1;
        }
        #endregion
    }

    #endregion

    #region Private Function
    private GameObject water;
    /// <summary>
    /// 水波特效处理
    /// </summary>
    /// <param name="show"></param>
    private void OnWater(bool show)
    {
        if (show)
        {
            water = ioo.poolManager.Spawn(EffectName.Effect_Water);
            water.transform.SetParent(ioo.gameMode.Player.WaterEffect);
            water.transform.localPosition = Vector3.zero;
            water.transform.localEulerAngles = Vector3.zero;
        }
        else
        {
            ioo.poolManager.DeSpawn(water);
            water = null;
        }
    }

    private GameObject dust;
    /// <summary>
    /// 杨尘特效处理
    /// </summary>
    /// <param name="show"></param>
    private void OnDust(bool show)
    {
        if (show)
        {
            dust = ioo.poolManager.Spawn(EffectName.Effect_Dust);
            dust.transform.SetParent(ioo.gameMode.Player.DustEffect);
            dust.transform.localPosition = Vector3.zero;
            dust.transform.localEulerAngles = Vector3.zero;
        }
        else
        {
            ioo.poolManager.DeSpawn(dust);
            dust = null;
        }
    }

    //void OnGUI()
    //{
    //    GUI.Label(new Rect(200, 500, 100, 100), " 耐久度: " +Data.ConditionRate.ToString());
    //    GUI.Label(new Rect(200, 600, 100, 100), " 速度因子: " + (PilotController._CurFactor * SpeedFactor * Data.ConditionRate).ToString());
    //}

    private float Test = 1;
    private bool HasEnd;
    /// <summary>
    /// 飞机驱动
    /// </summary>
    private void aircraftEngine()
    {
        // 没有初始化路径
        if (pathInfo == null)
            return;

        timer += ioo.nonStopTime.deltaTime * PilotController._CurFactor * SpeedFactor * Data.ConditionRate * Test * PilotController._LandFactor;

        Percent = timer / OverTime;

        if (ioo.gameMode.State == GameState.Play || ioo.gameMode.State == GameState.Continue)
        {
            if (Percent <= 1f)
            {
                curPoint = pathInfo.GetPos(Percent);
                upPoint = pathInfo.GetUpPos(Percent);
                nextPoint = pathInfo.GetPos(Mathf.Repeat(Percent + Step, 1f));
                transform.position = curPoint;

                Quaternion toRotatiojn = Quaternion.LookRotation(nextPoint - curPoint);
                transform.rotation = Quaternion.Lerp(transform.rotation, toRotatiojn, ioo.nonStopTime.deltaTime * 10);
            }
            else
            {
                // 起飞完成，进入游戏场景路线
                if (PathType == E_Path.Ready)
                {
                    GamePath();
                }

                if (PathType == E_Path.Game)
                {
                    EventDispatcher.TriggerEvent(EventDefine.Event_Game_Reset);
                    ioo.gameMode.ResetProp();
                }
                //清零计时器，重新开始 
                timer = 0;
            }
        }

        if (ioo.gameMode.State == GameState.Back)
        {
            UpdateBack();
        }
    }

    private void UpdateBack()
    {
        if (Percent <= 1)
        {
            curPoint = pathInfo.GetPos(Percent);
            upPoint = pathInfo.GetUpPos(Percent);
            nextPoint = pathInfo.GetPos(Mathf.Repeat(Percent + Step, 1f));
            transform.position = curPoint;

            Quaternion toRotatiojn = Quaternion.LookRotation(nextPoint - curPoint);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotatiojn, ioo.nonStopTime.deltaTime * 10);

            HelicopterBody.localRotation = Quaternion.identity;
            HelicopterBody.localPosition = Vector3.zero;

            if (!HasEnd)
            {
                Debug.Log("ioo.gameMode.State == GameState.Back");
                OnGorgeBoss(false);
                PCamera.fieldOfView = 45;
                CameraObj.transform.SetParent(null);
                CameraObj.transform.position = new Vector3(0, 6339.2f, 104.0f);
                HasEnd = true;
            }

            if (!PilotController.SlowDown)
            {
                float z = transform.position.z - CameraObj.transform.position.z;
                float speed = z < -2.5f ? Mathf.Abs(z) : 1;
                CameraObj.transform.position -= Vector3.forward * ioo.nonStopTime.deltaTime * speed;
            }
            else
            {

            }
        }
        else
        {
            AnimController.ToLand();
        }
    }

    /// <summary>
    /// 搜索射击范围最近目标
    /// </summary>
    /// <returns></returns>
    private Transform SearchTarget(WeaponBehaviour weapon)
    {
        float dis = Const.GAME_CONFIG_ATTACK_RADIUS ;
        float dis0 = Const.GAME_CONFIG_ATTACK_RADIUS ;
        float dis1 = Const.GAME_CONFIG_ATTACK_RADIUS ;
        float dis2 = Const.GAME_CONFIG_ATTACK_RADIUS ;
        Transform trans = null;
        Transform trans0 = null;
        Transform trans1 = null;
        Transform trans2 = null;

        trans0 = PropsManager.Instance.NearestEnemyToPlayer();
        if (trans0 != null)
            dis0 = Vector3.Distance(trans0.position, FirePoint.position);

        trans1 = WeaponManager.Instance.NearestEnemyToPlayer();
        if (trans1 != null)
            dis1 = Vector3.Distance(trans1.position, FirePoint.position);

        trans2 = EnemyManager.Instance.NearestEnemyToPlayer();
        if (trans2 != null)
            dis2 = Vector3.Distance(trans2.position, FirePoint.position);

        trans = dis0 > dis1 ? trans1 : trans0;
        dis = trans == trans0 ? dis0 : dis1;

        trans = dis > dis2 ? trans2 : trans;
        dis = trans == trans2 ? dis2 : dis;

        if(ioo.gameMode.Boss != null && !ioo.gameMode.Boss.IsDead)
        {
            float temp = Vector3.Distance(FirePoint.position, ioo.gameMode.Boss.Postion);
            if (dis > temp)
            {
                dis = temp;
                trans = ioo.gameMode.Boss.ShootPoint;
            }
        }

        if (dis > Const.GAME_CONFIG_ATTACK_RADIUS)
            trans = null;

        if (trans != null && trans == trans0)
        {
            trans.parent.parent.GetComponent<PropBehaviour>().IsLocked = true;
            trans.parent.parent.GetComponent<PropBehaviour>().LockedBy = weapon;
        }

        return trans;
    }

    /// <summary>
    /// 出仓库
    /// </summary>
    private void Readypath()
    {
        PathType = E_Path.Ready;
        OverTime = Const.GAME_CONFIT_READY_TIME;
        pathInfo = PathManager.Instance.PathInfo0;
        PilotController.ReadyPath();
        AnimController.ToTakeOff();
    }

    /// <summary>
    /// 游戏
    /// </summary>
    private void GamePath()
    {
        Pause = true;
        ActiveGather = true;
        OverTime = 180;
        pathInfo = PathManager.Instance.PathInfo1;
        PilotController.GamePath();
        EventDispatcher.TriggerEvent(EventDefine.Event_JiKu_To_Game_Light, true);
        EventDispatcher.TriggerEvent(EventDefine.Event_On_Describe, true);
        //DOVirtual.DelayedCall(8, delegate 
        //{
        //    Pause = false;
        //    PathType = E_Path.Game;
        //    CameraObj.SetActive(true);
        //    PilotController.CanControlled = true;
        //    PropsManager.Instance.Begine();
        //    EventDispatcher.TriggerEvent(EventDefine.Event_On_Describe, false);
        //    EventDispatcher.TriggerEvent(EventDefine.Event_Missile_Fired_Or_Trigger, true);
        //    IOManager.Instance.SendMessageGameBegine();
        //});
    }

    private void OnLiftEnd()
    {
        Readypath();
    }

    private bool CameraIsMoving;
    private void ChangeCameraState(E_CameraState state)
    {
        if (CameraState == state)
            return;

        switch (state)
        {
            case E_CameraState.FirstView:
                if (ShieldLife > 0)
                {
                    DestroyShield();
                    return;
                }

                if(ioo.gameMode.State == GameState.Continue)
                {
                    return;
                }

                Data.Hurry = 10;
                CamerCanRotation = false;
                EventDispatcher.TriggerEvent(EventDefine.Event_On_TieQian_Effect);
                Camera.main.nearClipPlane = 0.01f;
                CameraIsMoving = true;
                Sequence sequence = DOTween.Sequence();
                sequence.Append(CameraObj.transform.DOLocalMove(FirstCameraView.transform.localPosition, 0.5f));
                sequence.Join(CameraObj.transform.DOLocalRotate(Vector3.zero, 0.5f));
                sequence.Join(PCamera.DOFieldOfView(45, 0.5f));
                sequence.OnComplete(delegate 
                {
                    CameraIsMoving = false;
                    FirstCameraView.ActiveRobot();
                    EventDispatcher.TriggerEvent(EventDefine.Event_Hurry_ShowOrHide, true);
                });
                break;
            case E_CameraState.ThirdView:
                Camera.main.nearClipPlane = 0.3f;
                CameraIsMoving = true;
                sequence = DOTween.Sequence();
                sequence.Append(CameraObj.transform.DOLocalMove(ThirdCameraView.transform.localPosition, 0.5f));
                sequence.Join(CameraObj.transform.DOLocalRotate(new Vector3(14.49f, 0, 0), 0.5f));
                sequence.OnComplete(delegate
                {
                    CameraIsMoving = false;
                    CamerCanRotation = true;
                });
                break;
        }
        CameraState = state;
    }

    private void OnGorgeBoss(bool value)
    {
        DestroyShield();
        BossStep = value;
        if (BossStep)
        {
            SpeedFactor = 0.2f;
            PilotController.NormalSpeed();
            CameraObj.SetActive(false);
            BossCameraObj.SetActive(true);
        }
        else
        {
            SpeedFactor = 0.7f;
            CameraObj.SetActive(true);
            BossCameraObj.SetActive(false);
            CameraObj.transform.localPosition = ThirdCameraView.localPosition;
        }
    }

    private void OnWarning()
    {
        Effect_Warning.SetActive(true);
        DOVirtual.DelayedCall(3.1f, delegate { Effect_Warning.SetActive(false); });
    }

    private void OnGorgeAttack()
    {
        OnDamage(-2);
        ShackCameraByPosition(1.5f);
    }

    private void OnGlass()
    {
        DestroyShield();
    }

    private void DisableEffectHurt()
    {
        for (int i = 0; i < EffectHurtArray.Length; ++i )
        {
            EffectHurtArray[i].SetActive(false);
        }
    }

    private void ActiveEffectHurt(float percent)
    {
        if (percent == 0.5f)
        {
            for (int i = 0; i < EffectHurtArray.Length; ++i)
            {
                EffectHurtArray[i].SetActive(true);
            }
        }
        else if (percent < 0.8f)
        {
            AnimController.OnHurtFly();
            for (int i = 0; i < EffectHurtArray.Length - 1; ++i)
            {
                EffectHurtArray[i].SetActive(true);
            }
        }else if (percent >= 0.8f)
        {
            AnimController.OnNormalFly();
            DisableEffectHurt();
        }
    }

    private void OnShakeEnd0()
    {
        IsShaking0 = false;
    }

    private void OnShakeEnd1()
    {
        IsShaking1 = false;
    }
    #endregion

    #region Public Function
    private bool IsShaking0;
    public void ShackCameraByPosition(float last, float strength = 2)
    {
        if (IsShaking0 || CameraIsMoving)
            return;
        IsShaking0 = true;
        DG.Tweening.ShortcutExtensions.DOShakePosition(CameraObj.transform, last, Vector3.one * strength).OnComplete(OnShakeEnd0);
    }

    private bool IsShaking1;
    public void ShackCameraByRotation(float last, float strength = 2)
    {
        if (IsShaking1 || CameraIsMoving)
            return;
        IsShaking1 = true;
        DG.Tweening.ShortcutExtensions.DOShakeRotation(CameraObj.transform, last, Vector3.one * strength).OnComplete(OnShakeEnd1);
    }

    public bool CanRotation()
    {
        return CamerCanRotation;
    }

    /// <summary>
    /// 主场景启动
    /// </summary>
    public void GameBegine()
    {
        Pause = false;
        PathType = E_Path.Game;
        CameraObj.SetActive(true);
        PilotController.CanControlled = true;
        PropsManager.Instance.Begine();
        EventDispatcher.TriggerEvent(EventDefine.Event_On_Describe, false);
        EventDispatcher.TriggerEvent(EventDefine.Event_Missile_Fired_Or_Trigger, true);
        IOManager.Instance.SendMessageGameBegine();
    }

    /// <summary>
    /// 着陆
    /// </summary>
    public void Land()
    {
        PathType = E_Path.Land;

        for (int i = 0; i < MissilePoints.Length; ++i)
            MissilePoints[i].SetActive(false);

        CameraObj.transform.localEulerAngles = Vector3.zero;
        CameraObj.transform.localPosition = new Vector3(0.5f, 3.0f, -17.4f);
        timer = 0; 
        OverTime = Const.GAME_CONFIT_LAND_TIME;
        //PathManager.Instance.ReversePath(PathManager.Instance.PathInfo0);
        //pathInfo = PathManager.Instance.PathInfo0;
        pathInfo = PathManager.Instance.PathInfo2;

        PilotController.LandPath();
        PilotController.NormalSpeed();
        Data.Summary();
        ActiveEffectHurt(Data.ConditionRate);
        EffectPoint.gameObject.SetActive(false);

        Vector3 dir = pathInfo.GetPos(Step) - pathInfo.GetPos(0);
        Quaternion toRotation = Quaternion.LookRotation(dir);
        transform.rotation = toRotation;

        EventDispatcher.TriggerEvent(EventDefine.Event_JiKu_To_Game_Light, false);
        EventDispatcher.TriggerEvent(EventDefine.Event_Trigger_Effect_Water, false);
        EventDispatcher.TriggerEvent(EventDefine.Event_Trigger_Effect_Dust, false);
        EventDispatcher.TriggerEvent(EventDefine.Event_Engine_Speed_Up, false);
        //EventDispatcher.TriggerEvent(EventDefine.Event_Gorge_Boss_Animation, false);
    }
      
    public Vector3 Offset
    {
        get
        {
            return HelicopterBody.position - transform.position;
        }
    }

    public void UseGather()
    {
        if (PathType != E_Path.Game)
            return;

        if (CameraState == E_CameraState.FirstView)
        {
            Data.Hurry += 3;
        }
        else
        {
            if (CanUseGather && !BossStep)
            {
                UsingGather = true;
                CanUseGather = false;
                PilotController.UseGather();
            }
        }
       
    }

    private bool CameraSpeedUp;
    private bool CameraSpeedDown;
    public void CamerStartSpeedUp()
    {
        CameraSpeedUp = true;
        CameraSpeedDown = false;
    }

    public void CameraEndSpeedUp()
    {
        CameraSpeedUp = false;
        CameraSpeedDown = true;
    }

    public void AddLife()
    {
        Data.AddLife(Const.GAME_CONFIG_TIME);
        PilotController.ContinueToPlay();
    }

    public void OnCoin(int value)
    {
        Data.AddCoin(value);
    }

    public void OnFix(int value)
    {
        Data.AddFix(value);
        ActiveEffectHurt(Data.ConditionRate);
    }

    public void OnFuel(int value)
    {
        Data.AddFuel(value);
    }

    public void OnDiamond(int value)
    {
        Data.AddDiamond(value);
    }

    public void OnBat(int value)
    {
        Data.AddBat(value);
    }

    public void OnFeiQi(int value)
    {
        Data.AddFeiQi(value);
    }

    public void OnShield(int value)
    {
        ShieldLife = 10;
        Data.AddShield(value);
        Effect_Shield.SetActive(true);
    }

    public void OnMagnet(int value)
    {
        Effect_Magent.SetActive(true);
        Data.AddMagnet(value);
    }

    public void OnWall(int value)
    {
        Data.AddWall(value);
    }

    public void CloseMangent()
    {
        Effect_Magent.SetActive(false);
    }

    /// <summary>
    /// 受影响的生命值， 受影响的机身耐久度值
    /// </summary>
    /// <param name="value0"></param>
    /// <param name="value1"></param>
    public void OnDamage(float value0, float value1 = 0)
    {
        if (value1 == 0)
            value1 = value0;

        if (value0 > 0)
            Data.OnDamage(value0);
        else if (value0 < 0)
        {
            ShieldLife += value0;
            value1 += ShieldLife;
            if (ShieldLife <= 0)
            {
                if (ShieldLife < 0)
                {
                    Data.OnDamage(ShieldLife, value1);
                    ActiveEffectHurt(Data.ConditionRate);
                }
                DestroyShield();
            }
        }
    }

    public void DestroyShield()
    {
        ShieldLife = 0;
        Effect_Shield.SetActive(false);
    }

    /// <summary>
    /// 开火射击
    /// </summary>
    public void OnFire()
    {     
        if (!PilotController.CanControlled)
            return;

        if (!CanFire)
            return;

        if (Data.Hot >= 30)
            return;

        Data.Hot += 1;

        CanFire = false;

        Transform trans = null;
        WeaponBehaviour pb = WeaponManager.Instance.CreateWeapon(ModelName.WBullet);
        pb.Owner = GameTag.Player;

        if (!BossStep)
        {
            trans = SearchTarget(pb);
            pb.transform.position = FirePoint.position;

            if (trans != null)
            {
                pb.transform.LookAt(trans.position);
                pb.SetMoveToTargetTransform(trans, 100);
            }
            else
            {
                pb.SetMoveToDir(FirePoint.forward);
            }
        }
        else
        {
            pb.transform.position = BossCameraObj.transform.position;
            pb.SetMoveToDir(ShowUITran.position - BossCameraObj.transform.position);
        }

        WeaponManager.Instance.PlayerWeapon(pb);
    }

    public void AbilityMissile()
    {
        EventDispatcher.TriggerEvent(EventDefine.Event_Missile_Fired_Or_Trigger, false);

        for (int i = 0; i < MissilePoints.Length; ++i )
        {
            MissilePoints[i].SetActive(true);
        }

        RedMissile.SetActive(false);
        YellowMissile.SetActive(true);
    }

    /// <summary>
    /// 发射导弹
    /// </summary>
    public void OnMissile()
    {
        if (PathType == E_Path.Ready)
        {
            EventDispatcher.TriggerEvent(EventDefine.Event_Skip_Describe);
        }

        if (!PilotController.CanControlled)
            return;

        if ((!CanMissile && !PilotController._AbilityMissile))
            return;

        CanMissile = false;
              
         //三发导弹
        if (PilotController._AbilityMissile)
        {
            PilotController._AbilityMissile = false;
            YellowMissile.SetActive(false);

            for (int i = 0; i < MissilePoints.Length; ++i)
            {
                MissilePoints[i].SetActive(false);
                Transform trans = null;
                WeaponBehaviour pb = WeaponManager.Instance.CreateWeapon(ModelName.WMissle2);

                pb.transform.position = MissilePoints[i].transform.position;
                pb.Owner = GameTag.Player;

                if (!BossStep)
                {
                    trans = SearchTarget(pb);

                    if (trans != null)
                    {
                        pb.transform.LookAt(trans.position + i * Vector3.right * 10);
                        pb.SetMoveToTargetTransform(trans, 120);
                    }
                    else
                    {
                        pb.SetMoveToDir(FirePoint.forward);
                    }
                }
                else
                {
                    pb.transform.position = BossCameraObj.transform.position;
                    pb.SetMoveToDir(ShowUITran.position - BossCameraObj.transform.position);
                }

                WeaponManager.Instance.PlayerWeapon(pb);
            }

        }
        else
        {
            Transform trans         = null;
            WeaponBehaviour pb      = WeaponManager.Instance.CreateWeapon(ModelName.WMissle);
            pb.transform.position   = MissilePoints[0].transform.position;
            MissilePoints[0].SetActive(false);
            pb.Owner = GameTag.Player;
            if (!BossStep)
            {
                trans = SearchTarget(pb);
                if (trans != null)
                {
                    pb.transform.LookAt(trans.position);
                    pb.SetMoveToTargetTransform(trans, 120);
                }
                else
                {
                    pb.SetMoveToDir(FirePoint.forward);
                }
            }
            else
            {
                pb.transform.position = BossCameraObj.transform.position;
                pb.SetMoveToDir(ShowUITran.position - BossCameraObj.transform.position);
            }
          
            WeaponManager.Instance.PlayerWeapon(pb);
        }

        EventDispatcher.TriggerEvent(EventDefine.Event_Missile_Fired_Or_Trigger, true);

    }
    #endregion
}
