/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   WeaponManager.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/5/31 16:48:24
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;

public class WeaponManager
{
    private static readonly object _object = new object();
    private static WeaponManager _instance = null;
    public static WeaponManager Instance
    {
        get
        {
            if (null == _instance)
            {
                lock (_object)
                {
                    if (null == _instance)
                        _instance = new WeaponManager();
                }
            }
            return _instance;
        }
    }

    // 配表字段起始与结束ID
    protected const int StartID = 2001;
    protected const int EndID = 2007;

    // 武器名，对应数据ID
    private Dictionary<string, int> NameIDDict = new Dictionary<string, int>();

    // Boss Player 武器道具
    private Dictionary<WeaponBehaviour, float> WeaponDict = new Dictionary<WeaponBehaviour, float>();

    #region Public Function
    public void Init()
    {
        WeaponDict.Clear();
        NameIDDict.Clear();

        for (int i = StartID; i <= EndID; ++i)
        {
            WeaponPO bullet = WeaponData.Instance.GetWeaponPO(i);
            NameIDDict.Add(bullet.Name, i);
        }
    }

    private WeaponBehaviour _weapon;
    /// <summary>
    /// 依据道具名字生成一个道具
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public WeaponBehaviour CreateWeapon(string name, bool bornsound = true)
    {
        if (!NameIDDict.ContainsKey(name))
        {
            Debug.LogError("The name: " + name + " is not in the config file!");
            return null;
        }
        int id          = NameIDDict[name];
        WeaponPO po     = WeaponData.Instance.GetWeaponPO(id);
        GameObject obj  = ioo.poolManager.Spawn(name);
        _weapon         = obj.GetOrAddComponent<WeaponBehaviour>();
        _weapon.Type    = (WeaponType)po.Type;
        _weapon.Assaultable     = po.IsEnermy == 1 ? true : false;
        _weapon.BornVolumeName  = po.BornVolumeName;
        _weapon.DieVolumeName   = po.DieVolumeName;
        _weapon.DamageValue     = po.Damage;
        _weapon.DamagePlane     = po.DamagePlane;
        _weapon.BornEffect      = po.BornEffect;
        _weapon.DieEffect       = po.DieEffect;
        _weapon.Owner           = string.Empty;

        _weapon.Spawn(bornsound);
        return _weapon;
    }

    /// <summary>
    /// 寻找与玩家距离最近的敌人
    /// </summary>
    /// <returns></returns>
    public Transform NearestEnemyToPlayer()
    {
        List<WeaponBehaviour> pblist = new List<WeaponBehaviour>(WeaponDict.Keys);
        float dis = Const.GAME_CONFIG_ATTACK_RADIUS + 1;
        Transform ret = null;
        for (int i = 0; i < pblist.Count; ++i)
        {
            if (!pblist[i].Assaultable)
                continue;

            Vector3 dir = pblist[i].ShootPoint.position - ioo.gameMode.Player.FirePoint.position;
            float angle = Vector3.Angle(ioo.gameMode.Player.FirePoint.forward, dir);
            if (angle > 60)
                continue;

            float temp = Vector3.Distance(ioo.gameMode.Player.FirePoint.position, pblist[i].Root.position);
            if (dis > temp)
            {
                dis = temp;
                ret = pblist[i].ShootPoint;
            }
        }

        return ret;
    }

    // Boss武器
    public void BossWeapon(WeaponBehaviour pb)
    {
        WeaponDict.Add(pb, 3);
    }

    // 玩家武器
    public void PlayerWeapon(WeaponBehaviour pb)
    {
        WeaponDict.Add(pb, 4);
    }

    /// <summary>
    ///  小飞机武器
    /// </summary>
    /// <param name="pb"></param>
    public void MiniPlanWeapon(WeaponBehaviour pb)
    {
        WeaponDict.Add(pb, 3);
    }

    // 玩家和Boss销毁武器
    public void AddDespawnWeapon(WeaponBehaviour pb)
    {
        WeaponDict[pb] = 0;
    }

    public void Update()
    {
        #region Boss Player武器道具

        #region 移除过时武器道具
        List<WeaponBehaviour> clearList = new List<WeaponBehaviour>();
        List<WeaponBehaviour> pblist = new List<WeaponBehaviour>(WeaponDict.Keys);
        List<float> flist = new List<float>(WeaponDict.Values);
        for (int i = 0; i < flist.Count; ++i)
        {
            if (flist[i] > 0)
            {
                flist[i] -= Time.deltaTime;
            }
            else
            {
                flist[i] = 0;
                clearList.Add(pblist[i]);
            }
            WeaponDict[pblist[i]] = flist[i];
        }

        for (int i = 0; i < clearList.Count; ++i)
        {
            WeaponBehaviour pb = clearList[i];
            WeaponDict.Remove(pb);
            pb.DeSpawn();
        }

        #endregion

        #endregion

    }
    #endregion
 
}
