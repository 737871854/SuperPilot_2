/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   EnemyManager.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/7/11 16:43:57
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : SingletonBehaviour<EnemyManager>
{
    public override void Awake()
    {
        base.Awake();
    }

    public List<MiniAirplane> EnemyList;
    public List<RobotBehaviour> RobotList;
    public List<MiniAirplane2> Enemy2List;
    public List<DamagedPlane> DamageList;
    public List<BugAttack> BugsList;

    void Start()
    {
        BugsList = new List<BugAttack>();
        GameObject[] obj = GameObject.FindGameObjectsWithTag(GameTag.Bug);
        for (int i = 0; i < obj.Length; ++i )
        {
            BugsList.Add(obj[i].GetComponent<BugAttack>());
        }
    }

    public Transform NearestEnemyToPlayer()
    {
        float dis = Const.GAME_CONFIG_ATTACK_RADIUS + 1;
        Transform ret = null;
        for (int i = 0; i < EnemyList.Count; ++i )
        {
            if (!EnemyList[i].gameObject.activeSelf)
                continue;

            Vector3 dir = EnemyList[i].HitPoint.position - ioo.gameMode.Player.FirePoint.position;
            float angle = Vector3.Angle(ioo.gameMode.Player.FirePoint.forward, dir);
            if (angle > 60)
                continue;

            float temp = Vector3.Distance(ioo.gameMode.Player.FirePoint.position, EnemyList[i].HitPoint.position);
            if (dis > temp)
            {
                dis = temp;
                ret = EnemyList[i].HitPoint;
            }
        }

        for (int i = 0; i < Enemy2List.Count; ++i)
        {
            if (!Enemy2List[i].gameObject.activeSelf)
                continue;

            Vector3 dir = Enemy2List[i].HitPoint.position - ioo.gameMode.Player.FirePoint.position;
            float angle = Vector3.Angle(ioo.gameMode.Player.FirePoint.forward, dir);
            if (angle > 60)
                continue;

            float temp = Vector3.Distance(ioo.gameMode.Player.FirePoint.position, Enemy2List[i].HitPoint.position);
            if (dis > temp)
            {
                dis = temp;
                ret = Enemy2List[i].HitPoint;
            }
        }

        for (int i = 0; i < DamageList.Count; ++i)
        {
            if (!DamageList[i].gameObject.activeSelf)
                continue;

            Vector3 dir = DamageList[i].transform.position - ioo.gameMode.Player.FirePoint.position;
            float angle = Vector3.Angle(ioo.gameMode.Player.FirePoint.forward, dir);
            if (angle > 60)
                continue;

            float temp = Vector3.Distance(ioo.gameMode.Player.FirePoint.position, DamageList[i].transform.position);
            if (dis > temp)
            {
                dis = temp;
                ret = DamageList[i].transform;
            }
        }

        for (int i = 0; i < RobotList.Count; ++i)
        {
            if (!RobotList[i].gameObject.activeSelf)
                continue;

            Vector3 dir = RobotList[i].HitPoint.position - ioo.gameMode.Player.FirePoint.position;
            float angle = Vector3.Angle(ioo.gameMode.Player.FirePoint.forward, dir);
            if (angle > 60)
                continue;

            float temp = Vector3.Distance(ioo.gameMode.Player.FirePoint.position, RobotList[i].HitPoint.position);
            if (dis > temp)
            {
                dis = temp;
                ret = RobotList[i].HitPoint;
            }
        }

        for (int i = 0; i < BugsList.Count; ++i)
        {
            if (!BugsList[i].gameObject.activeSelf)
                continue;

            Vector3 dir = BugsList[i].ShootPoint.position - ioo.gameMode.Player.FirePoint.position;
            float angle = Vector3.Angle(ioo.gameMode.Player.FirePoint.forward, dir);
            if (angle > 60)
                continue;

            float temp = Vector3.Distance(ioo.gameMode.Player.FirePoint.position, BugsList[i].ShootPoint.position);
            if (dis > temp)
            {
                dis = temp;
                ret = BugsList[i].ShootPoint;
            }
        }


        return ret;
    }
}
