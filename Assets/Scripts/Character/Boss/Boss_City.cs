/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   Boss.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/5/28 11:56:16
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using System;
using Need.Mx;
using DG.Tweening;
using DG.Tweening.Core;

public class Boss_City : BossBase
{
    enum E_State
    {
        Idle = 0,
        /// <summary>
        /// 激光
        /// </summary>
        Laser = 1,
        /// <summary>
        /// 扔石头
        /// </summary>
        Throw = 2,
    }

    public GameObject JiGuangEffect;

    public GameObject StoneBornPoint;

    public SkinnedMeshRenderer MeshRender;

    private E_State State;

    private int CurIndex;
    private int NextIndex;

    private List<Transform> PathList = new List<Transform>();

    private float CurCoolTime;
    private float CoolTime = 5;

    private List<Vector3> PlayerPosList = new List<Vector3>();

    private float rate;
    
    #region Unity CallBack

    void OnEnable()
    {
        Init();
        PlayBackGroundMusic();
    }

    void OnDisable()
    {
        for (int i = 0; i < StoneList.Count; ++i)
        {
            WeaponManager.Instance.AddDespawnWeapon(StoneList[i]);
        }
        StoneList.Clear();

        StopBackGroundMusic();

        // 防止放激光武器时突然死亡
        EventDispatcher.TriggerEvent(EventDefine.Event_On_Trigger_JiGuang_Weapon, false);

        ioo.audioManager.PlaySoundOnPoint("SFX_Sound_City_Boss_Dead", transform.position);
    }

    void Init()
    {
        CurIndex    = 0;
        NextIndex   = 1;
        MaxLife     = 200;
        Life        = MaxLife;
        IsDead      = false;
        HasStone    = false;
        HasThrow    = false;
        CanChangeColor = true;
        HasStonrBornEffect  = false;
        ioo.gameMode.Boss   = this;
        State       = E_State.Idle;
        Root        = transform.Find("Root").transform;
        PathList    = ioo.gameMode.Player.BossPathList;
        transform.localPosition         = Vector3.zero;
        transform.localEulerAngles      = Vector3.zero;
        Root.transform.localPosition    = Vector3.zero;
        Root.transform.localEulerAngles = Vector3.zero;

        Anim = Root.GetComponent<Animator>();

        MeshRender.material.SetColor("_EmissionColor", Color.white);

        PlayerPosList.Clear();

        rate = Worth / MaxLife;
    }

    private Vector3 oldDirection = Vector3.zero;
    private int weapon = 0;
    void Update()
    {
        if(ioo.gameMode.State >= GameState.Back)
            return;

        PlayerPosList.Add(ioo.gameMode.Player.HelicopterBody.position);
        if (PlayerPosList.Count > 20)
            PlayerPosList.RemoveAt(0);

        #region Boss死亡
        if (Life == 0)
        {
            IsDead = true;
            EffectManager.Instance.Spawn(EffectName.Effect_Boss_City_Dead, transform.position);
            ioo.gameMode.Boss = null;
            gameObject.SetActive(false);
            ioo.audioManager.PlayPersonSound("Person_Sound_City_Boss_Be_Destroyed", true);
            ioo.gameMode.Player.Data.AddLife(15);
            EventDispatcher.TriggerEvent(EventDefine.Event_Tips_Type_By_Int, 1, transform.position, 15);
            return;
        }
        #endregion

        #region 更随玩家
        float percent       = ioo.gameMode.Player.Percent + 0.005f;
        Vector3 curPoint    = PathManager.Instance.PathInfo1.GetPos(percent);
        transform.position  = curPoint - Vector3.up * 10;

        transform.LookAt(ioo.gameMode.Player.transform.position);
        #endregion

        #region 释放技能
        if (CurCoolTime < CoolTime)
        {
            CurCoolTime += Time.deltaTime;
        }
        else
        {
            CurCoolTime = 0;
            SpawnWeapon(weapon);
            ++weapon;
            weapon %= 2;
        }
        #endregion

        //#region 吸附道具技能
        //if (SkillTime > 0)
        //{
        //    SkillTime -= Time.deltaTime;
        //    Check();
        //    if (SkillTime <= 0)
        //        SkillTime = 0;
        //}
        //#endregion

        //Vector3 targetPos = PathList[NextIndex].position;
        //Vector3 direction = targetPos - Root.position;
        //float angle = Vector3.Angle(direction, oldDirection);
        //if (Vector3.Distance(targetPos, Root.position) < 0.1f || angle >= 150)
        //{
        //    ++NextIndex;
        //    NextIndex %= PathList.Count;
        //}
        //else
        //{
        //    direction.Normalize();
        //    Root.position += direction * Time.deltaTime * 10;
        //    Root.LookAt(ioo.gameMode.Player.transform.position);
        //    oldDirection = direction;
        //}

        UpdateAnimaton();

        if (JiGuangEffect.activeSelf && PlayerPosList.Count > 0)
        {
            Vector3 TargetPos = PlayerPosList[0];
            PlayerPosList.RemoveAt(0);
            JiGuangEffect.transform.LookAt(TargetPos);
        }
    }

    #endregion

    #region Private Function

    private bool HasStonrBornEffect;
    private bool HasStone;
    private bool HasThrow;
    private void UpdateAnimaton()
    {
        AnimatorStateInfo info = Anim.GetCurrentAnimatorStateInfo(0);
        switch (State)
        {
            case E_State.Idle:
                if (!info.IsName("Idle"))
                    Anim.SetInteger("State", (int)State);
                break;
            case E_State.Laser:
                if (!info.IsName("Laser"))
                    Anim.SetInteger("State", (int)State);
                else
                {
                    if (info.normalizedTime >= 0.15f && info.normalizedTime < 1)
                    {
                        Fire();
                    }
                    else if (info.normalizedTime >= 1)
                    {
                        State = E_State.Idle;
                    }
                }
                break;
            case E_State.Throw:
                if (!info.IsName("Throw"))
                    Anim.SetInteger("State", (int)State);
                else
                {
                    if (info.normalizedTime >= 0.24f && !HasStonrBornEffect)
                    {
                        HasStonrBornEffect = true;
                        EffectManager.Instance.Spawn(EffectName.Effect_WStone_Born, StoneBornPoint.transform);
                    }

                    if (info.normalizedTime >= 0.44f && !HasStone)
                    {
                        HasThrow    = false;
                        HasStone    = true;
                        Stone       = WeaponManager.Instance.CreateWeapon(ModelName.WStone);
                        Stone.transform.SetParent(StoneBornPoint.transform);
                        Stone.transform.localPosition = Vector3.zero;
                        StoneList.Add(Stone);
                    }
                    
                    if (info.normalizedTime >= 0.76f && !HasThrow)
                    {
                        HasThrow = true;
                        Throw();
                    }

                    if (info.normalizedTime > 0.99f)
                    {
                        State = E_State.Idle;
                    }
                }
                break;
        }
    }

    /// <summary>
    /// 扔石头
    /// </summary>
    private void Throw()
    {
        // 指定
        float percent       = ioo.gameMode.Player.Percent + 0.0035f; ;
        Vector3 target      = PathManager.Instance.PathInfo1.GetPos(percent);
        Vector3 targetRight = PathManager.Instance.PathInfo1.GetRightPos(percent);

        Vector3 offset  = ioo.gameMode.Player.Offset;
        target          += offset;
        targetRight     += offset;

        Vector3 rightDir = targetRight - target;
        rightDir.Normalize();
        target += rightDir;

        Stone.transform.SetParent(null);

        ioo.audioManager.PlaySoundOnObj("SFX_Stone_Fly", Stone.gameObject);

        Vector3 dir = target - Stone.transform.position;

        Stone.SetMoveToDir(dir.normalized, 50);
        Stone.Owner = GameTag.Boss;

        WeaponManager.Instance.BossWeapon(Stone);

    }

    /// <summary>
    /// 发射激光
    /// </summary>
    private void Fire()
    {
        if (JiGuangEffect.activeSelf)
            return;

        JiGuangEffect.SetActive(true);
    }

    /// <summary>
    /// 依据武器类型下发武器
    /// </summary>
    /// <param name="type"></param>
    private void SpawnWeapon(int type)
    {
        if (UnityEngine.Random.Range(0,10) < 5)
            ioo.audioManager.PlayPersonSound("Person_Sound_Boss_Attack" + UnityEngine.Random.Range(0, 2));
        switch (type)
        {
            case 0:
                //SpawnMine();
                OnLaser();
                break;
            case 1:
                OnThrow();
                //SpawnMissle();
                break;
            //case 2:
            //    SkillTime = 3;
            //    break;
        }
    }

    private List<WeaponBehaviour> StoneList = new List<WeaponBehaviour>();
    private WeaponBehaviour Stone;
    private void OnThrow()
    {
        HasStone            = false;
        HasStonrBornEffect  = false;
        State               = E_State.Throw;
    }

    /// <summary>
    /// 发射激光
    /// </summary>
    private void OnLaser()
    {
        State = E_State.Laser;
    }

    ///// <summary>
    ///// 部署地雷
    ///// </summary>
    //private void SpawnMine()
    //{
    //    // 指定
    //    float percent       = ioo.gameMode.Player.Percent + 0.01f;
    //    Vector3 target      = PathManager.Instance.PathInfo1.GetPos(percent);
    //    Vector3 targetRight = PathManager.Instance.PathInfo1.GetRightPos(percent);

    //    Vector3 offset = ioo.gameMode.Player.Offset;
    //    target += offset;
    //    targetRight += offset;

    //    float rand          = UnityEngine.Random.Range(-2, 2);
    //    Vector3 rightDir    = targetRight - target;
    //    rightDir.Normalize();
    //    target              += rightDir * rand;

    //    WeaponBehaviour pb = WeaponManager.Instance.CreateWeapon(ModelName.WMine);
    //    pb.transform.position = WeaponPoint.transform.position;
    //    pb.SetMoveToTragetPosition(target, 50);
    //    pb.Owner = GameTag.Boss;

    //    WeaponManager.Instance.BossWeapon(pb);
    //}

    ///// <summary>
    ///// 部署导弹
    ///// </summary>
    //private void SpawnMissle()
    //{
    //    float percent       = ioo.gameMode.Player.Percent + 0.001f;
    //    Vector3 target      = PathManager.Instance.PathInfo1.GetPos(percent);

    //    Vector3 offset = ioo.gameMode.Player.Offset;
    //    target += offset;

    //    WeaponBehaviour pb = WeaponManager.Instance.CreateWeapon(ModelName.WMissle);
    //    pb.transform.position = WeaponPoint.transform.position;
    //    pb.SetMoveToTragetPosition(target, 150);
    //    pb.Owner = GameTag.Boss;

    //    WeaponManager.Instance.BossWeapon(pb);
    //}


    ///// <summary>
    ///// 检查周围可以吸附的道具
    ///// </summary>
    //private void Check()
    //{
    //    Collider[] colliders = Physics.OverlapSphere(transform.position, 20);
    //    if (colliders.Length == 0)
    //        return;
    //    for (int i = 0; i < colliders.Length; ++i)
    //    {
    //        if (colliders[i].transform.parent == null)
    //            continue;
    //        PropBehaviour pb = colliders[i].transform.parent.GetComponent<PropBehaviour>();
    //        if (pb == null)
    //            continue;

    //        pb.SetMoveToTargetTransform(Root, 50);
    //    }
    //}

    private bool CanChangeColor;
    private void ChangeColor()
    {
        if (!CanChangeColor)
            return;

        CanChangeColor = false;
        MeshRender.material.SetColor("_EmissionColor", Color.red);
        DOVirtual.DelayedCall(0.2f, delegate 
        {
            CanChangeColor = true;
            MeshRender.material.SetColor("_EmissionColor", Color.white);
        });
        //Sequence sequence = DOTween.Sequence();
        //sequence.Append(material.DOColor(new Color(1, 0, 0), "_EmissionColor", 0.2f));
        //sequence.Join(material.DOColor(new Color(1, 1, 1), "_EmissionColor", 0.2f));
        //sequence.OnComplete(OnColorEnd);
    }

    //private void OnColorEnd()
    //{
    //    CanChangeColor = true;
    //}

    /// <summary>
    /// 吃道具回血 
    /// </summary>
    public void TriggerProp()
    {
        if (Life == MaxLife)
            return;

        ++Life;
    }

    /// <summary>
    /// 受伤
    /// </summary>
    public void Damage(float value, bool isPlaer = false)
    {
        ChangeColor();

        if (isPlaer)
        {
            if (Life > Mathf.Abs(value))
                ioo.gameMode.Player.Data.AddBossScore((int)(Mathf.Abs(value) * rate));
            else
                ioo.gameMode.Player.Data.AddBossScore((int)(Life * rate));
        }
        Life += value;
        if (Life < 0)
            Life = 0;
    }
    #endregion

    #region Public Function
    public void OnTriggerProp()
    {
        TriggerProp();
    }

    public override void OnDamage(float damage, bool isPlaer = false)
    {
        Damage(damage, isPlaer);
    }
    #endregion
}
