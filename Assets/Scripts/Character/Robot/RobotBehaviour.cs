/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   RobotBehaviour.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/7/8 8:51:57
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RobotBehaviour : MonoBehaviour
{
    private enum E_Sate
    {
        /// <summary>
        /// 巡逻
        /// </summary>
        Patrol,
        /// <summary>
        /// 追逐
        /// </summary>
        Chase,
        /// <summary>
        /// 攻击
        /// </summary>
        Attack,
        /// <summary>
        /// 命中
        /// </summary>
        Get,
        /// <summary>
        /// 未命中
        /// </summary>
        Miss,
    }

    ///// <summary>
    /////  巡逻路径
    ///// </summary>
    //public List<Vector3> PathList;

    /// <summary>
    /// 巡逻移动速度
    /// </summary>
    public float PatrolSpeed;

    /// <summary>
    /// 追逐移动速度
    /// </summary>
    public float ChaseSpeed;

    /// <summary>
    /// 搜索玩家距离
    /// </summary>
    public float SearchDis = 5;

    /// <summary>
    /// 攻击距离
    /// </summary>
    public float AttackDis = 1;

    public Transform HitPoint;

    private float Life;

    public RobotTrigger Left;
    public RobotTrigger Right;

    public GameObject Effect;

    private Animator Anim;

    /// <summary>
    /// 当期状态
    /// </summary>
    private E_Sate State = E_Sate.Patrol;

    /// <summary>
    /// 可以攻击
    /// </summary>
    private bool CanAttack;

    /// <summary>
    /// 下个路径目标点
    /// </summary>
    private int NextIndex;

    /// <summary>
    /// 与玩家距离
    /// </summary>
    private float Distance;

    /// <summary>
    /// 攻击目标
    /// </summary>
    private Transform Target;

    private bool GetTarget;

    private Vector3 StartPos;

    // Use this for initialization
    void Start()
    {
        GetTarget   = false;
        Life        = 5;
        Anim        = transform.Find("Root").GetComponent<Animator>();
        Anim.speed  = 2;
        StartPos    = transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.tag.Equals(GameTag.Weapon))
            return;

        WeaponBehaviour wb = other.GetComponent<WeaponBehaviour>();

        if (wb.Owner != GameTag.Player)
            return;

        OnDamage(wb.DamageValue);

        if (wb.Type == WeaponType.Missle || wb.Type == WeaponType.Missle2)
            wb.Trigger();

        WeaponManager.Instance.AddDespawnWeapon(wb);
    }

    private void OnDamage(float damage)
    {
        Life += damage;
        if (Life <= 0)
        {
            Life = 0;
            Death();
        }
    }

    public void Hide()
    {
        Anim.gameObject.SetActive(false);
        StartCoroutine(DisActive());
    }

    private void Death()
    {
        ioo.audioManager.PlaySoundOnPoint("SFX_Sound_Robot_Dead", transform.position);
        Anim.gameObject.SetActive(false);
        Effect.SetActive(true);
        StartCoroutine(DisActive());
    }

    IEnumerator DisActive()
    {
        yield return new WaitForSeconds(20);
        Life = 5;
        Anim.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Life == 0)
            return;

        if (Target == null)
        {
            Distance = 100;
            GameObject obj = GameObject.FindGameObjectWithTag(GameTag.Player);
            if (obj != null)
                Target = obj.transform;
        }
        else
            Distance = Vector3.Distance(transform.position, Target.position);

        GetTarget = (Left.GetTarget || Right.GetTarget);

        if (!Anim.gameObject.activeSelf)
            return;

        switch(State)
        {
            case E_Sate.Patrol:
                OnPatrol();
                break;
            case E_Sate.Chase:
                OnChase();
                break;
            case E_Sate.Attack:
                OnAttack();
                break;
            case E_Sate.Get:
                OnGet();
                break;
            case E_Sate.Miss:
                OnMiss();
                break;
        }
    }

    /// <summary>
    /// 攻击未命中
    /// </summary>
    private void OnMiss()
    {
        AnimatorStateInfo info = Anim.GetCurrentAnimatorStateInfo(0);
        if (!info.IsName("Miss"))
        {
            Anim.speed = 4;
            Anim.SetInteger("State", 3);
        }
        else
        {
            if (info.normalizedTime >= 1.0f)
                State = E_Sate.Patrol;
        }
    }

    /// <summary>
    /// 攻击命中
    /// </summary>
    private void OnGet()
    {
        AnimatorStateInfo info = Anim.GetCurrentAnimatorStateInfo(0);
        if (!info.IsName("Get"))
        {
            Anim.speed = 4;
            Anim.SetInteger("State", 2);
        }
        else
        {
            if (info.normalizedTime >= 1.0f)
                State = E_Sate.Patrol;
        }
    }

    /// <summary>
    /// 攻击
    /// </summary>
    private void OnAttack()
    {
        transform.LookAt(Target.position);

        AnimatorStateInfo info = Anim.GetCurrentAnimatorStateInfo(0);
        if (!info.IsName("Attack"))
        {
            Anim.speed = 4;
            Anim.SetInteger("State", 1);
        }
        else
        {
            if (info.normalizedTime >= 1.0f)
            {
                if (GetTarget)
                    State = E_Sate.Get;
                else
                    State = E_Sate.Miss;
            }
        }
    }

    /// <summary>
    /// 巡逻
    /// </summary>
    private Vector3 oldPatrolDir;
    private void OnPatrol()
    {
        AnimatorStateInfo info = Anim.GetCurrentAnimatorStateInfo(0);
        if (!info.IsName("Move"))
        {
            Anim.SetInteger("State", 0);
        }

        if (Distance <= SearchDis)
        {
            State = E_Sate.Chase;
            return;
        }

        Vector3 patrolDir = StartPos - transform.position;
        if (patrolDir.magnitude > 0.1f && Vector3.Angle(patrolDir, oldPatrolDir) < 150)
        {
            transform.position += patrolDir.normalized * Time.deltaTime * PatrolSpeed;
            transform.LookAt(Target.position);
        }
        else
        {
            transform.position = StartPos;
        }
    }

    /// <summary>
    /// 追逐
    /// </summary>
    private void OnChase()
    {
        AnimatorStateInfo info = Anim.GetCurrentAnimatorStateInfo(0);
        if (!info.IsName("Move"))
        {
            Anim.SetInteger("State", 0);
        }

        if (Distance <= AttackDis)
        {
            State = E_Sate.Attack;
            return;
        }
        else
        {
            State = E_Sate.Patrol;
        }

        Vector3 patrolDir = Target.position - transform.position;
        if (patrolDir.magnitude > 0.1f && Vector3.Angle(patrolDir, oldPatrolDir) < 150)
        {
            transform.position += patrolDir.normalized * Time.deltaTime * ChaseSpeed;
            Quaternion toRotation = Quaternion.LookRotation(patrolDir);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, Time.deltaTime * 20);
        }
    }
}
