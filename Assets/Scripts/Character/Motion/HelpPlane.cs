/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   HelpPlane.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/7/18 12:03:04
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;

public class HelpPlane : MonoBehaviour
{
    /// <summary>
    /// 射击点
    /// </summary>
    public Transform FirePoint;

    public int ID;

    public Transform Panter;

    private float deltaTime = 0.2f;

    private Vector3 StartPos;
    private Quaternion StartRotation;

    private float DisapperTime;

    void Awake()
    {
        StartPos = transform.position;
        StartRotation = transform.rotation;
    }

    void OnEnable()
    {
        DisapperTime        = 0;
        transform.position  = StartPos;
        StartRotation       = transform.rotation;
        if (ioo.gameMode.Boss != null)
        {
            olddir = ioo.gameMode.Boss.ShootPoint.position - transform.position;
            olddir.Normalize();
        }
    }

    private Vector3 olddir;
    private float CoolTime;
    private int Count;
    // Update is called once per frame
    void Update()
    {
        if (ioo.gameMode.State >= GameState.Back)
            return;

        Vector3 dir0 = Panter.position - transform.position;
        if (dir0.magnitude < 20)
            dir0.Normalize();
        else
            dir0 = Vector3.zero;

        if (ioo.gameMode.Boss != null && !ioo.gameMode.Boss.IsDead)
        {
            Vector3 dir1 = ioo.gameMode.Boss.ShootPoint.transform.position - transform.position;
            Quaternion toRotation = Quaternion.LookRotation(dir1);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, Time.deltaTime * 5);
            if (dir1.magnitude > 60 && Vector3.Angle(olddir, dir1) < 160)
            {
                transform.position += (dir1.normalized + dir0) * Time.deltaTime * 20;
            }
            else
            {
                if (CoolTime > 0)
                {
                    CoolTime -= Time.deltaTime;
                    return;
                }
                else
                    CoolTime = 0;

                if (deltaTime > 0)
                {
                    deltaTime -= Time.deltaTime;
                }
                else
                {
                    deltaTime = 0.2f;
                    FriendFire(Count);
                    if (++Count == 4)
                    {
                        Count = 0;
                        CoolTime = 1;
                    }
                }
            }
        }
        else
        {
            DisapperTime += Time.deltaTime;
            if (DisapperTime > 5)
                gameObject.SetActive(false);
            transform.position += transform.up * Time.deltaTime * 50;
        }
    }

    private void FriendFire(int count)
    {
        if (ioo.gameMode.State != GameState.Play)
            return;

        Transform target = null;
        WeaponBehaviour wb = null;

        if (ioo.gameMode.Boss != null && !ioo.gameMode.Boss.IsDead)
        {
            switch (count)
            {
                case 0:
                case 1:
                case 2:
                    wb = WeaponManager.Instance.CreateWeapon(ModelName.WBullet);
                    break;
                case 3:
                    wb = WeaponManager.Instance.CreateWeapon(ModelName.WMissle);
                    break;
            }
            wb.transform.position = FirePoint.position;
            target = ioo.gameMode.Boss.ShootPoint;
            wb.SetMoveToTargetTransform(target, 100);
            WeaponManager.Instance.MiniPlanWeapon(wb);
        }
        return;
    }

    //private void Fire()
    //{
    //    if (ioo.gameMode.Boss != null && !ioo.gameMode.Boss.IsDead)
    //    {
    //        WeaponBehaviour wb      = WeaponManager.Instance.CreateWeapon(ModelName.WBullet);
    //        wb.transform.position   = FirePoint.position;
    //        wb.Owner                = GameTag.Friend;
    //        wb.SetMoveToTargetTransform(ioo.gameMode.Boss.ShootPoint);
    //        WeaponManager.Instance.MiniPlanWeapon(wb);
    //    }
    //}
}
