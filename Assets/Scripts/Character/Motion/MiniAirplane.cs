/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   MiniAirplane.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/7/3 16:08:56
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;
using Need.Mx;

public class MiniAirplane : MonoBehaviour
{
    public enum E_PlaneType
    {
        Enemy,
        Friend,
        Dragon,
    }

    public Transform FirePoint;

    public int Index = 0; // 0:上，1：左

    public E_PlaneType Type;

    public Transform HitPoint;

    public float OffsetUp = 3;
    public float OffsetLeft = 10;
    public float OffsetRight = 10;

    public float OffsetPercent = 0.05f;

    public bool TargetIsBossOnly = false;

    public Animation Anim;

    public float AnimationTime;
    public bool CanRotation;

    public float Offset_Y;

    public bool KeepInDistance = true;
    private bool HasGetGo;

    private float FriendAnimTime;
    private bool FriendCanRotation = false;

    private float Life;

    private float deltaTime = 0.2f;

    private float startPercent;
    private float endPercent;
    private float percent;
    private PathInfo pathinfo;

    private Vector3 targetPos;
    private bool isLeave;

    void OnEnable()
    {
        if (Type == E_PlaneType.Enemy)
        {
            isLeave         = false;
            Life            = 1;
            startPercent    = ioo.gameMode.Player.Percent;
            percent         = startPercent + OffsetPercent;
            pathinfo        = PathManager.Instance.PathInfo1;
            ioo.audioManager.PlayBackMusic("Music_Enemy_Fire");
        }
       
        if (Type == E_PlaneType.Friend)
        {
            transform.localEulerAngles = Vector3.zero;
            HasGetGo            = false;
            FriendCanRotation = CanRotation;
            FriendAnimTime  = AnimationTime;
            percent         = ioo.gameMode.Player.Percent - 0.01f;
            endPercent      = percent + OffsetPercent;
            pathinfo        = PathManager.Instance.PathInfo1;
            EventDispatcher.AddEventListener<Vector3, Vector3>(EventDefine.Event_Friend_Leave, OnLeave);
            EventDispatcher.AddEventListener<bool>(EventDefine.Event_Boss_Must_Dead, OnKill);
        }

        if (Type == E_PlaneType.Dragon)
        {
            startPercent    = ioo.gameMode.Player.Percent;
            percent         = startPercent + OffsetPercent;
            pathinfo        = PathManager.Instance.PathInfo1;
        }
    }

    void OnDisable()
    {
        if (Type == E_PlaneType.Friend)
        {
            isLeave = false;
            Anim.CrossFade("Friend", 0);
            EventDispatcher.RemoveEventListener<Vector3, Vector3>(EventDefine.Event_Friend_Leave, OnLeave);
            EventDispatcher.RemoveEventListener<bool>(EventDefine.Event_Boss_Must_Dead, OnKill);
        }

        if (Type == E_PlaneType.Enemy)
        {
            ioo.audioManager.StopBackMusic("Music_Enemy_Fire");
        }

         if (Type == E_PlaneType.Dragon)
         {
             //ioo.audioManager.StopBackMusic("Music_Dragon_Fly");
         }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.tag.Equals(GameTag.Weapon) || Type == E_PlaneType.Dragon)
            return;

        WeaponBehaviour wb = other.GetComponent<WeaponBehaviour>();

        if (wb.Owner != GameTag.Player)
            return;

        OnDamage(wb.DamageValue);
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

    private void Death()
    {
        ioo.audioManager.PlaySoundOnPoint("SFX_Sound_Enemy_Plane_Destroy", transform.position);
        EffectManager.Instance.Spawn(EffectName.Effect_diji_Plane_baoz, transform.position);
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Type == E_PlaneType.Enemy)
        {
            UpdateEnemy();
        }

        if (Type == E_PlaneType.Friend)
        {
            UpdateFriend();
        }

        if (Type == E_PlaneType.Dragon)
        {
            UpdateDragon();
        }
    }

    private void UpdateEnemy()
    {
        if (deltaTime > 0)
        {
            deltaTime -= Time.deltaTime;
        }
        else
        {
            deltaTime = 0.2f;
            EnemyFire();
        }

        // 上
        if (Index == 0)
        {
            Vector3 curPoint = pathinfo.GetPos(percent);
            Vector3 nexPoint = pathinfo.GetPos(percent - 0.05f * Time.deltaTime * 0.2f);
            transform.position = curPoint + transform.up * OffsetUp;

            Quaternion toRotatiojn = Quaternion.LookRotation(nexPoint - curPoint);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotatiojn, Time.deltaTime * 10);
        }

        // 左
        if (Index == 1)
        {
            Vector3 curPoint = pathinfo.GetPos(percent);
            Vector3 nexPoint = pathinfo.GetPos(percent - 0.0001f);
            transform.position = curPoint - transform.right * OffsetLeft + transform.up * OffsetUp;

            Quaternion toRotatiojn = Quaternion.LookRotation(nexPoint - curPoint);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotatiojn, Time.deltaTime * 10);
        }

        // 右
        if (Index == 2)
        {
            Vector3 curPoint = pathinfo.GetPos(percent);
            Vector3 nexPoint = pathinfo.GetPos(percent - 0.0001f);
            transform.position = curPoint + transform.right * OffsetRight + transform.up * OffsetUp;

            Quaternion toRotatiojn = Quaternion.LookRotation(nexPoint - curPoint);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotatiojn, Time.deltaTime * 10);
        }

        percent -= 0.05f * Time.deltaTime * 0.2f;

        if (percent <= startPercent)
        {
            gameObject.SetActive(false);
        }
    }

    private Vector3 PathDir;
    private float CoolTime;
    private int Count;
    private void UpdateFriend()
    {
        if (FriendCanRotation)
        {
            if (FriendAnimTime > 0)
            {
                FriendAnimTime -= Time.deltaTime;
            }
            else
            {
                FriendCanRotation = false;
                Anim.CrossFade("Friend");
            }
        }

        if (!isLeave)
        {
            // 左
            if (Index == 0)
            {
                Vector3 curPoint    = pathinfo.GetPos(percent);
                Vector3 nexPoint    = pathinfo.GetPos(percent + 0.0001f);
                transform.position  = curPoint - transform.right * OffsetLeft + transform.up * Offset_Y;

                Quaternion toRotatiojn  = Quaternion.LookRotation(nexPoint - curPoint);
                transform.rotation      = Quaternion.Lerp(transform.rotation, toRotatiojn, Time.deltaTime * 10);
            }

            // 右
            if (Index == 1)
            {
                Vector3 curPoint = pathinfo.GetPos(percent);
                Vector3 nexPoint = pathinfo.GetPos(percent + 0.0001f);
                transform.position = curPoint + transform.right * OffsetRight + transform.up * Offset_Y;

                Quaternion toRotatiojn = Quaternion.LookRotation(nexPoint - curPoint);
                transform.rotation = Quaternion.Lerp(transform.rotation, toRotatiojn, Time.deltaTime * 10);
            }
    
             if (KeepInDistance)
             {
                 if (percent < ioo.gameMode.Player.Percent + 0.002f)
                 {
                     percent += 0.018f * Time.deltaTime * ioo.gameMode.Player.PilotController._CurFactor * ioo.gameMode.Player.Data.ConditionRate;
                 }
                 else
                 {
                     float rate = 1 / (1000 * (percent - ioo.gameMode.Player.Percent - 0.002f));
                     percent += 0.0056f * Time.deltaTime * ioo.gameMode.Player.PilotController._CurFactor * rate;
                 }
             
             }
             else
             {
                 if (!HasGetGo)
                 {
                     percent += 0.018f * Time.deltaTime * ioo.gameMode.Player.PilotController._CurFactor * ioo.gameMode.Player.Data.ConditionRate;

                     if (percent >= ioo.gameMode.Player.Percent + 0.002f)
                     {
                         HasGetGo = true;
                     }
                 }else
                     percent += 0.0056f * Time.deltaTime * ioo.gameMode.Player.PilotController._CurFactor * ioo.gameMode.Player.Data.ConditionRate;
                 
             }

             PathDir = transform.forward;
        }
       
        if (isLeave)
        {
            Vector3 dir = targetPos - transform.position;
            if (dir.magnitude > 2)
            {
                dir = (dir.normalized + PathDir).normalized;
                transform.position += dir * Time.deltaTime * 120;

                if (PathDir.magnitude > 0.1f)
                    PathDir -= PathDir * Time.deltaTime;
                else
                    PathDir = Vector3.zero;

                Quaternion toRotation = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, Time.deltaTime * 10);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }


        if (ioo.gameMode.State != GameState.Play)
            return;

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

    private void UpdateDragon()
    {
        Vector3 curPoint = pathinfo.GetPos(percent);
        Vector3 nexPoint = pathinfo.GetPos(percent - 0.01f * Time.deltaTime);
        transform.position = curPoint + transform.up * OffsetUp;

        Quaternion toRotatiojn = Quaternion.LookRotation(nexPoint - curPoint);
        transform.rotation = Quaternion.Lerp(transform.rotation, toRotatiojn, Time.deltaTime * 10);

        percent -= 0.005f * Time.deltaTime;

        if (percent <= startPercent)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnLeave(Vector3 pos0, Vector3 pos1)
    {
        isLeave = true;
        if (Index == 0)
        {
            targetPos = pos0;
        }
        else
        {
            targetPos = pos1;
        }
    }

    private void OnKill(bool kill)
    {
        if (kill)
        {
            WeaponBehaviour wb = WeaponManager.Instance.CreateWeapon(ModelName.WMissle);
            wb.transform.position = FirePoint.position;
            wb.Owner = GameTag.Friend;
            wb.SetMoveToTargetTransform(ioo.gameMode.Boss.ShootPoint);
            WeaponManager.Instance.MiniPlanWeapon(wb);
        }
    }

    private void EnemyFire()
    {
        WeaponBehaviour wb = WeaponManager.Instance.CreateWeapon(ModelName.WBullet, false);
        wb.transform.position = FirePoint.position;
        wb.Owner = GameTag.Enemy;
        wb.SetMoveToDir(FirePoint.forward);
        WeaponManager.Instance.MiniPlanWeapon(wb);
    }

    private void FriendFire(int count)
    {
        Transform target = null;
        WeaponBehaviour wb = null;

        if (TargetIsBossOnly)
        {
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
                target                  = ioo.gameMode.Boss.ShootPoint;
                wb.SetMoveToTargetTransform(target, 100);
                WeaponManager.Instance.MiniPlanWeapon(wb);
            }
            return;
        }
        else
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
            target                  = SearchTarget(wb);
            if (target != null)
            {
                wb.SetMoveToTargetTransform(target, 100);
            }
            else
            {
                wb.SetMoveToDir(FirePoint.forward);
            }
            WeaponManager.Instance.MiniPlanWeapon(wb);
        }
    }


    /// <summary>
    /// 搜索射击范围最近目标
    /// </summary>
    /// <returns></returns>
    private Transform SearchTarget(WeaponBehaviour weapon)
    {
        float dis = Const.GAME_CONFIG_ATTACK_RADIUS;
        float dis0 = Const.GAME_CONFIG_ATTACK_RADIUS;
        float dis1 = Const.GAME_CONFIG_ATTACK_RADIUS;
        float dis2 = Const.GAME_CONFIG_ATTACK_RADIUS;
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

        if (ioo.gameMode.Boss != null && !ioo.gameMode.Boss.IsDead)
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

}
