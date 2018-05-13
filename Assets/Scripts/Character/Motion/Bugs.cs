/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   Bugs.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/8/28 16:34:56
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using Need.Mx;

public class Bugs : MonoBehaviour
{
    public Transform PathTran;
    public float SearchLength   = 150;
    public float PatrolSpeed    = 10;
    public float ChaseSpeed     = 20;
    public float Life = 1;

    //public Vector3 Offset = Vector3.up;

    [System.NonSerialized]
    public BugAttack BugAttack;


    private List<BugPoint> PatrolList = new List<BugPoint>();
    private int CurIndex;
    private PathInfo pathinfo;
    private float NearestPercent;
    private float Percent;
    private bool InPath;
    private float OrgLife;

    private enum E_State
    {
        Patrol  = 0,
        Chase   = 1,
        Dead    = 2,
    }

    private E_State State;

    void Start()
    {
        OrgLife = Life;
        PathTran.GetComponentsInChildren<BugPoint>(PatrolList);
        BugAttack   = transform.Find("Root").GetComponent<BugAttack>();
        pathinfo    = PathManager.Instance.PathInfo1;

        EventDispatcher.AddEventListener(EventDefine.Event_PathInfo_Has_Init, OnPathInfo);

        Init();
    }

    void OnDeatroy()
    {
        EventDispatcher.RemoveEventListener(EventDefine.Event_PathInfo_Has_Init, OnPathInfo);
    }

    void Update()
    {
        if (ioo.gameMode.Player == null)
            return;

        switch (State)
        {
            case E_State.Patrol:
                OnPatrol();
                break;
            case E_State.Chase:
                OnChase();
                break;
            //case E_State.Dead:
            //    OnDead();
            //    break;
        }

        if (State == E_State.Dead)
        {
            if (EaterTime > 0)
                EaterTime -= Time.deltaTime;
            else
                Init();
        }
    }

    void OnTriggerEnter(Collider other)
    {

    }

    private void Init()
    {
        InPath      = false;
        CurIndex    = 0;
        Life        = OrgLife;
        State       = E_State.Patrol;
        Percent     = NearestPercent;
        BugAttack.Chase = false;
    }

    private void OnPathInfo()
    {
        NearestPercent = PropsManager.Instance.NearestPercent(transform.position);
        Percent = NearestPercent;
    }

    //private void OnDead()
    //{
    //    State = E_State.Dead;
    //}

    /// <summary>
    /// 攻击
    /// </summary>
    private void OnAttack()
    {
        BugAttack.gameObject.SetActive(false);
    }

    /// <summary>
    /// 巡逻
    /// </summary>
    private void OnPatrol()
    {
        if (!BugAttack.gameObject.activeSelf)
        {
            BugAttack.gameObject.SetActive(true);
            BugAttack.transform.localPosition       = BugAttack.Offset;
            BugAttack.transform.localEulerAngles    = Vector3.zero;
        }

        float dis = Vector3.Distance(transform.position, ioo.gameMode.Player.FirePoint.transform.position);
        if (dis <= SearchLength)
        {
            BugAttack.Chase = true;
            State = E_State.Chase;
            return;
        }

        Vector3 dir = PatrolList[CurIndex].transform.position - transform.position; 
        if (dir.magnitude < 0.4f)
        {
            ++CurIndex;
            CurIndex %= PatrolList.Count;
        }
        else
        {
            transform.position      += dir.normalized * Time.deltaTime * PatrolSpeed * 0.5f;
            Quaternion toRotation   = Quaternion.LookRotation(dir);
            transform.rotation      = Quaternion.Lerp(transform.rotation, toRotation, Time.deltaTime * 10);
        }
    }

    /// <summary>
    /// 追逐
    /// </summary>
    private void OnChase()
    {
        Vector3 dir = ioo.gameMode.Player.FirePoint.transform.position - transform.position;
        if (dir.magnitude > SearchLength)
        {
            BugAttack.Chase = false;
            State = E_State.Patrol;
            return;
        }

        if (InPath)
        {
            Vector3 curPoint = pathinfo.GetPos(Percent);
            Percent = Percent - 0.001f * Time.deltaTime * ChaseSpeed;
            Vector3 nexPoint = pathinfo.GetPos(Percent);
            transform.position = curPoint;

            Quaternion toRotatiojn = Quaternion.LookRotation(nexPoint - curPoint);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotatiojn, Time.deltaTime * 10);
        }
        else
        {
            dir = pathinfo.GetPos(NearestPercent) - transform.position;
            if (dir.magnitude < 0.2f)
            {
                InPath = true;
                return;
            }
            else
            {
                transform.position += dir.normalized * 20 * Time.deltaTime;
                Quaternion toRotation = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, Time.deltaTime * 10);
            }
        }
    }

    private float EaterTime;
    public void OnHurt(float damage)
    {
        Life += damage;

        if (Life <= 0)
        {
            State       = E_State.Dead;
            BugAttack.OnDead();
            EaterTime   = 20;
            ioo.gameMode.Player.OnDamage(2);
            EventDispatcher.TriggerEvent(EventDefine.Event_Tips_Type_By_Int, 1, BugAttack.transform.position, 2);
            PropsManager.Instance.SpawnPropInPos(PropType.Diamond, BugAttack.transform.position, ioo.gameMode.Player.Percent + 0.05f);
        }
    }

    public void OnExplosed()
    {
        Life = 0;
        State = E_State.Dead;
        BugAttack.OnDead();
        EaterTime = 20;
    }
}
