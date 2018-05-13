/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   Boss_Gorge.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/7/14 9:31:04
 * 
 * 修改描述： 峡谷场景Boss
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using Need.Mx;
using System.Collections;

public class Boss_Gorge : BossBase
{
    private enum E_State
    {
        /// <summary>
        /// 飞起
        /// </summary>
        Fly = 0,
        /// <summary>
        /// 吐火
        /// </summary>
        Fire = 1,
        /// <summary>
        /// 起飞
        /// </summary>
        Move = 2,
        /// <summary>
        /// 攻击
        /// </summary>
        Attack = 3,
        /// <summary>
        /// 死亡
        /// </summary>
        Dead = 4,
        /// <summary>
        /// 吼叫 
        /// </summary>
        Bellow = 5,
        /// <summary>
        /// 被攻击
        /// </summary>
        Hurt = 6,
    }

    public GameObject PathObj;

    public GameObject FireEffect;
    public GameObject AttackEffect;

    public GameObject DeadEffect;

    public Transform FireBallBorn;

    private List<Transform> PathList = new List<Transform>();

    private int Index;
    private Transform Target;
    private bool GetTarget;

    private bool TargetIsBoss;

    private E_State State = E_State.Fly;

    private bool CanRotation;
    private bool CanMove;

    private float CurCoolTime;
    private float CoolTime = 5;

    private Vector3 StartPos;
    private Quaternion StartRotation;

    private bool SkillCool;

    private float rate;

    public List<GorgeBossHitTrigger> HitList;
    public List<GorgeBossHitTrigger> UsedHitList = new List<GorgeBossHitTrigger>();

    public MeshCollider MeshCollider;

    void Awake()
    {
        StartPos = transform.position;
        StartRotation = transform.rotation;

        if (PathObj == null)
            return;

        for (int i = 0; i < PathObj.transform.childCount; ++i)
        {
            PathList.Add(PathObj.transform.GetChild(i).transform);
        }
    }

    void OnEnable()
    {
        Init();
        PlayBackGroundMusic();
    }

    void OnDisable()
    {
        StopBackGroundMusic();
        EventDispatcher.RemoveEventListener(EventDefine.Event_Gorge_Boss_Fly, TriggerFly);
    }

    void Init()
    {
        SkillCool   = true;
        CurCoolTime = 0;
        CoolTime    = 5;
        Index       = 0;
        MaxLife     = 200;
        Speed       = 40;
        Life        = MaxLife;
        GetTarget   = false;
        IsDead      = false;
        CanRotation = false;
        CanMove     = false;
        HasFlyOver  = false;
        CanUpdate   = false;
        Thunder     = false;
        TargetIsBoss = false;
        ioo.gameMode.Boss = this;

        transform.position = StartPos;
        transform.rotation = StartRotation;

        Target = PathList[0];
        Root = transform.Find("Root").transform;

        Anim        = Root.GetComponent<Animator>();
        rate        = Worth / MaxLife;
        Anim.speed  = 0;

        olddir = Target.position - transform.position;

        Root.gameObject.SetActive(true);

        EventDispatcher.AddEventListener(EventDefine.Event_Gorge_Boss_Fly, TriggerFly);
    }

    private Vector3 olddir;
    private float BellowTime;
    private bool CanUpdateHit;
    private bool HitWait;
    private int CurIndex;
    private float HitWaitTime;
    void Update()
    {
        switch (State)
        {
            case E_State.Fire:
                OnFire();
                break;
            case E_State.Fly:
                OnFly();
                break;
            case E_State.Attack:
                OnAttack();
                break;
            case E_State.Move:
                OnMove();
                break;
            case E_State.Dead:
                OnDead();
                break;
            case E_State.Bellow:
                OnBellow();
                break;
            case E_State.Hurt:
                OnHurt();
                break;
        }

        if (ioo.gameMode.State >= GameState.Back)
            return;

        if (CanUpdateHit)
        {
            if (!HitWait)
            {
                HitWait = true;
                HitWaitTime = 0;
                List<GorgeBossHitTrigger> temp = new List<GorgeBossHitTrigger>();

                for (int i = 0; i <= CurIndex; ++i )
                {
                    temp.Add(UsedHitList[i]);
                }

                EventDispatcher.TriggerEvent(EventDefine.Event_Gorge_Boss_QTE, temp);
                ++CurIndex;
                if (CurIndex >= UsedHitList.Count)
                {
                    CanUpdateHit = false;
                  for (int i = 0; i < HitList.Count; ++i)
                  {
                      for (int j = 0; j < UsedHitList.Count; ++j )
                      {
                          if (HitList[i] == UsedHitList[j])
                              HitList[i].EnableCollider();
                      }
                  }
                }
            }
            else
            {
                HitWaitTime += ioo.nonStopTime.deltaTime;
                if (HitWaitTime >= 0.5f)
                    HitWait = false;
            }
        }

        #region Boss死亡
        if (Life == 0 && !IsDead)
        {
            IsDead = true;
            State = E_State.Dead;
            ioo.audioManager.PlayPersonSound("Person_Sound_Gorge_Boss_Be_Destroyed", true);
            ioo.gameMode.Player.Data.AddLife(15);
            EventDispatcher.TriggerEvent(EventDefine.Event_Tips_Type_By_Int, 1, transform.position, 15);
            //StartCoroutine(DestroyBoss());
            return;
        }
        #endregion

        #region 更随玩家

        if (CanRotation)
        {
            Quaternion toRotation = Quaternion.LookRotation(Target.position - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, Time.deltaTime * 8);
        }

        if (CanMove)
        {
            Vector3 dir = Target.position - transform.position;
            if (TargetIsBoss)
            {
                if (!GetTarget)
                {
                    if (dir.magnitude <= 30 && Vector3.Angle(dir, olddir) < 160)
                        GetTarget = true;
                    else
                        transform.position += dir.normalized * Time.deltaTime * 30;
                }
                else
                {
                    TargetIsBoss = false;
                    //Attack();
                    Bellow();
                }
            }
            else
            {
                if (dir.magnitude > 0.2f && Vector3.Angle(dir, olddir) < 120)
                {
                    transform.position += dir.normalized * Time.deltaTime * 10;
                }
                else
                {
                    CanRotation = true;
                    ++Index;
                    Index %= PathList.Count;
                    Target = PathList[Index];
                }

                #region 释放技能
                if (CurCoolTime < CoolTime)
                {
                    CurCoolTime += Time.deltaTime;
                }
                else
                {
                    CurCoolTime = 0;
                    Target = ioo.gameMode.Player.BossCameraObj.transform;
                    TargetIsBoss = true;
                }
                #endregion
            }

            olddir = dir;
        }
        #endregion
    }

    private void OnBellow()
    {
        AnimatorStateInfo info = Anim.GetCurrentAnimatorStateInfo(0);
        if (!info.IsName("Bellow"))
        {
            Anim.SetInteger("State", (int)State);
        }
        else
        {

        }
        
        if (BellowTime >= 8.0f)
        {
            BellowTime = 0;
            State = E_State.Attack;
            for (int i = 0; i < UsedHitList.Count; ++i )
            {
                UsedHitList[i].DisableCollider();
            }
            ioo.gameMode.TimeScale(1);
            ioo.gameMode.Player.Pause = false;
            UsedHitList.Clear();
            MeshCollider.enabled = true;
            EventDispatcher.TriggerEvent(EventDefine.Event_Gorge_Boss_QTE, UsedHitList);
        }
        else
        {
            BellowTime += ioo.nonStopTime.deltaTime;

            if (UsedHitList.Count == 0)
            {
                BellowTime = 0;
                State = E_State.Hurt;
                for (int i = 0; i < UsedHitList.Count; ++i)
                {
                    UsedHitList[i].DisableCollider();
                }
                ioo.gameMode.TimeScale(1);
                ioo.gameMode.Player.Pause = false;
                MeshCollider.enabled = true;
                OnDamage(-25, true);
                EventDispatcher.TriggerEvent(EventDefine.Event_Gorge_Boss_QTE, UsedHitList);
            }
        }
    }

    private void OnHurt()
    {
        AnimatorStateInfo info = Anim.GetCurrentAnimatorStateInfo(0);
        if (!info.IsName("Hurt"))
        {
            Anim.SetInteger("State", (int)State);
        }
        else
        {
            if(info.normalizedTime >= 1.0f)
            {
                CanMove     = true;
                GetTarget   = false;
                Target      = PathList[Index];
                State       = E_State.Move;
            }
        }
    }

    private float downSpeed = 10;
    private void OnDead()
    {
        AnimatorStateInfo info = Anim.GetCurrentAnimatorStateInfo(0);
        if (!info.IsName("Dead"))
        {
            Anim.SetInteger("State", (int)State);
        }
        else
        {
            if (info.normalizedTime >= 0.5f)
            {
                transform.position += Vector3.down * Time.deltaTime * downSpeed;
                downSpeed += 98 * Time.deltaTime;

                if (transform.position.y <= StartPos.y)
                {
                    DeadEffect.transform.position = transform.position;
                    DeadEffect.SetActive(true);
                    Root.gameObject.SetActive(false);
                    StartCoroutine(ChangeCamera());
                }
            }
        }
    }

    IEnumerator ChangeCamera()
    {
        yield return new WaitForSeconds(1);
        EventDispatcher.TriggerEvent(EventDefine.Event_Gorge_Boss_Animation, false);
        ioo.gameMode.Boss = null;
        gameObject.SetActive(false);
    }

    private void OnMove()
    {
        AnimatorStateInfo info = Anim.GetCurrentAnimatorStateInfo(0);
        if (!info.IsName("Move"))
        {
            Anim.SetInteger("State", (int)State);
        }
    }

    private void OnAttack()
    {
        AnimatorStateInfo info = Anim.GetCurrentAnimatorStateInfo(0);
        if (!info.IsName("Attack"))
        {
            Anim.SetInteger("State", (int)State);
        }
        else
        {
            if (info.normalizedTime >= 1.0f)
            {
                CanMove = true;
                GetTarget = false;
                Target = PathList[Index];
                State = E_State.Move;
            }

            if (info.normalizedTime >= 0.8f && SkillCool)
            {
                SkillCool = false;
                WeaponBehaviour wb = WeaponManager.Instance.CreateWeapon(ModelName.WFireBall);
                wb.transform.position = FireBallBorn.position;
                wb.Owner = GameTag.Boss;

                wb.SetMoveToTargetTransform(ioo.gameMode.Player.BossAttackPoint, 40);
            }
        }
    }

    private bool HasFlyOver;
    private float Speed;
    private bool CanUpdate;
    private float Timer;
    private bool Thunder;
    private void OnFly()
    {
        AnimatorStateInfo info = Anim.GetCurrentAnimatorStateInfo(0);
        if (!info.IsName("Fly"))
        {
            Anim.SetInteger("State", (int)State);
        }
        else
        {
            if (info.normalizedTime >= 1.0f)
            {
                State = E_State.Fire;
            }
            else if (info.normalizedTime > 0.40f && !Thunder)
            {
                Thunder = true;
                FireEffect.SetActive(true);
                ioo.audioManager.PlaySound2D("SFX_Sound_Dragon_Thunder");
            }
        }

        if (HasFlyOver || !CanUpdate)
            return;

        Vector3 dir = Target.position - transform.position;
        if (dir.magnitude < 0.1f || Vector3.Angle(dir, olddir) > 60)
        {
            ++Index;
            HasFlyOver  = true;
            Target      = PathList[Index];
        }
        else
        {
            Timer               += Time.deltaTime;
            transform.position  += dir.normalized * Time.deltaTime * Speed;
            Speed               += Time.deltaTime * 30;
        }
    }

    private void OnFire()
    {
        AnimatorStateInfo info = Anim.GetCurrentAnimatorStateInfo(0);
        if (!info.IsName("Fire"))
        {
            Anim.SetInteger("State", (int)State);
        }
        else
        {
            if (info.normalizedTime >= 1.0f)
            {
                CanMove = true;
                State = E_State.Move;
            }
        }
    }

    private void OnIdle()
    {
        AnimatorStateInfo info = Anim.GetCurrentAnimatorStateInfo(0);
        if (!info.IsName("Idle"))
        {
            Anim.SetInteger("State", (int)State);
        }
    }

    //private void Attack()
    //{
    //    if (State == E_State.Dead)
    //        return;
    //    CanMove = false;
    //    SkillCool = true;
    //    State = E_State.Attack;
    //    ioo.audioManager.PlayPersonSound("Person_Sound_Boss_Attack" + Random.Range(0, 3));
    //}

    private void Bellow()
    {
        if (State == E_State.Dead)
            return;
        CanMove     = false;
        SkillCool   = true;
        State       = E_State.Bellow;
        MeshCollider.enabled = false;
        ioo.gameMode.Player.Pause = true;
        ioo.gameMode.TimeScale(0.05f);
        FireEffect.SetActive(true);
        UsedHitList.Clear();

        int rand = Random.Range(0, HitList.Count);
        for (int i = 0; i < HitList.Count; ++i)
        {
            if (i == rand)
                continue;
            UsedHitList.Add(HitList[i]);
            //HitList[i].EnableCollider();
        }

        ioo.audioManager.PlayPersonSound("Person_Sound_Boss_Attack" + Random.Range(0, 3));
        CanUpdateHit = true;
        CurIndex = 0;
        HitWaitTime = 0;
        HitWait = false;
    }

    private void TriggerFly()
    {
        CanUpdate = true;
        State = E_State.Fly;
        Anim.speed = 1;
        //ioo.audioManager.PlaySound2D("Music_Dragon_Fire");
    }

    private void TriggerFollow()
    {
        State = E_State.Fly;
    }

    /// <summary>
    /// 受伤
    /// </summary>
    public void Damage(float value, bool isPlaer = false)
    {
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

    public override void OnDamage(float damage, bool isPlaer = false)
    {
        if (State == E_State.Bellow)
            return;

        Damage(damage, isPlaer);
    }

    public void RemoveHit(GorgeBossHitTrigger hit)
    {
        if (UsedHitList.Contains(hit))
        {
            UsedHitList.Remove(hit);
            EventDispatcher.TriggerEvent(EventDefine.Event_Gorge_Boss_QTE, UsedHitList);
        }
    }
    //IEnumerator DestroyBoss()
    //{
    //    yield return new WaitForSeconds(1.5f);
      
    //}
}
