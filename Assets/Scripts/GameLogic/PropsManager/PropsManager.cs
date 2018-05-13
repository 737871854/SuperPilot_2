/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   PropsManager.cs
 * 
 * 简    介:    道具管理类
 * 
 * 创建标识：   Pancake 2017/5/26 11:22:33
 * 
 * 修改描述：   沿用了一代飞机道具点预计算思想,为了适应当前项目做了变换
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using Need.Mx;

/// <summary>
/// 存放路径上的道具点信息
/// </summary>
public struct PointInfo
{
    public string name;             // 道具名字
    public Vector3 pos;             // 位置坐标
    public Vector3 nexPos;          // 下个位置坐标
    public bool isMidle;            // 模型不偏移
    public bool isLeft;             // 模型偏移方向
    public float LocatePercent;     // 处于路径位置百分比
    public bool used;               // 该点是否正在被使用
    public PropType type;           // 道具类型
    public int score;               // 分数
    public float OffsetPrecent;     // 该点的位置在LocaltePercent的基础上便宜量
    public bool Assaultable;        // 改点对应的道具是否可被攻击
    public string BornVolumeName;   // 音效名字
    public string DieVolumeName;    // 音效名字
    public float DamageValue;       // 伤害值 （+ 增益， - 减益）
    public float DamagePlane;
    public string BornEffect;       // 出生特效名
    public string DieEffect;        // 死亡特效名
}


public class PropsManager 
{
    private static readonly object _object = new object();
    private static PropsManager _instance = null;
    public static PropsManager Instance
    {
        get
        {
            if (null == _instance)
            {
                lock(_object)
                {
                    if (null == _instance)
                        _instance = new PropsManager();
                }
            }
            return _instance;
        }
    }

    #region 内部配置字段
    
    #endregion
    // 路径上各类道具点的间隔
    private float GenPropStep = 15;
    // 第一个道具点距离路径首端距离
    private float BeginSpace = 20;
    // 最后一个道具点距离路径末端距离
    private float EndSpace = 20;
    // 可视范围
    private float VisablePercent = 0.05f;   
    // 为记录道具点对应前方位置
    protected const float Step = 0.001f;

    #region 逻辑变量
    // 可以开始逻辑帧更新
    public bool CanUpdate;

    // 配表字段起始与结束ID
    protected const int StartID = 1001;
    protected const int EndID = 1010;

    #endregion

    // 存放场景道具位置信息
    private List<PointInfo> PosInfoList = new List<PointInfo>();

    #region 存放场景中被使用的道具
    private List<PropBehaviour> PropList = new List<PropBehaviour>();
    private List<PropBehaviour> DespawnPropList = new List<PropBehaviour>();
    #endregion

    private int Count = 0;

    private PropsPO CoinPo;
    private PropsPO DiamondPo;

    #region Public Function
    /// <summary>
    /// 初始化，配置路径字段
    /// </summary>
    public void Init()
    {
        Count       = 0;
        _curIndex   = 0;
        PosInfoList.Clear();

        float pathLength    = PathManager.Instance.PathInfo1.PathLength;
        //int pointCount      = (int)(pathLength / GenPropStep);
        //float stepPrecent   = GenPropStep / pathLength;
        int pointCount = (int)(pathLength / GenPropStep);
        float beginPrecent  = BeginSpace / pathLength;
        float endPrecent    = 1 - EndSpace / pathLength;
        float curPrecent    = beginPrecent;

        int lastID          = -1;
        float lastPrecent = 0;
        for (int n = 0; n < pointCount; ++n)
        {
            float rand = Random.Range(0f, 100f);

            bool loop = true;
            while (loop)
            {
                for (int i = StartID; i <= EndID; ++i)
                {
                    PropsPO po = PropsData.Instance.GetPropsPO(i);

                    if ((PropType)po.Type == PropType.Coin)
                        CoinPo = po;
                    if ((PropType)po.Type == PropType.Diamond)
                        DiamondPo = po;

                    if (rand >= po.MinPrecent && rand < po.MaxPrecent)
                    {
                        if (lastID == i)
                        {
                            rand = Random.Range(0f, 100f);
                            break;
                        }

                        if (rand >= 68 && (curPrecent - lastPrecent) < 0.01f)
                        {
                            rand = Random.Range(0f, 68);
                            break;
                        }

                        curPrecent += GenProp(po, curPrecent);

                        if (rand >= 68)
                            lastPrecent = curPrecent;

                        lastID = i;
                        loop = false;
                        if (curPrecent >= endPrecent)
                        {
                            EventDispatcher.TriggerEvent(EventDefine.Event_PathInfo_Has_Init);
                            return;
                        }
                        break;
                    }
                }
            }
        }

    }

    public void Begine()
    {
        CanUpdate   = true;
    }

    public void End()
    {
        CanUpdate = false;
        PropList.Clear();
        DespawnPropList.Clear();
    }
    
    #region Boss Player
    // 销毁普通道具
    public void AddDespawnNormal(PropBehaviour pb)
    {
        DespawnPropList.Add(pb);
    }

    /// <summary>
    /// 寻找与玩家距离最近的敌人
    /// </summary>
    /// <returns></returns>
    public Transform NearestEnemyToPlayer()
    {
        float dis = Const.GAME_CONFIG_ATTACK_RADIUS + 1;
        Transform ret = null;
        for (int i = 0; i < PropList.Count; ++i )
        {
            if (!PropList[i].Assaultable || PropList[i].IsLocked)
                continue;

            Vector3 dir = PropList[i].ShootPoint.position - ioo.gameMode.Player.FirePoint.position;
            float angle = Vector3.Angle(ioo.gameMode.Player.FirePoint.forward, dir);
            if (angle > 60)
                continue;

            float temp = Vector3.Distance(ioo.gameMode.Player.FirePoint.position, PropList[i].Root.position);
            if (dis > temp)
            {
                dis = temp;
                ret = PropList[i].ShootPoint;
            }
        }

        return ret;
    }
    #endregion

    private int _curIndex = 0;
    private PointInfo _updateInfo;
    private PropBehaviour _prop;
    public void Update()
    {
        if (!CanUpdate)
            return;
        #region 按路径下发道具
        float percent = ioo.gameMode.GetPilotPrecent;
        int frameCount = Mathf.FloorToInt(1 / Time.deltaTime);

        if (PosInfoList.Count > 0)
        {
            int updateCount = PosInfoList.Count * 5 / frameCount;
            for (int i = 0; i < updateCount; ++i)
            {
                if (_curIndex >= PosInfoList.Count)
                {
                    _curIndex = 0;
                    ResetPropInfo();
                }

                _updateInfo = PosInfoList[_curIndex];
                if (_updateInfo.LocatePercent <= percent + VisablePercent && _updateInfo.LocatePercent > percent)
                {
                    if (!_updateInfo.used)
                    {
                        if (ioo.gameMode.Boss != null && (
                                                          //_updateInfo.type == PropType.Engine   || 
                                                          //_updateInfo.type == PropType.Wall     || 
                                                          //_updateInfo.type == PropType.Shield   ||
                                                          //_updateInfo.type == PropType.Mine
                                                          _updateInfo.type == PropType.Coin ||
                                                          _updateInfo.type == PropType.Mine ||
                                                          _updateInfo.type == PropType.Engine
                                                          ))
                        {
                            _prop                       = null;
                            _updateInfo.used            = true;
                            GameObject obj              = ioo.poolManager.Spawn(CoinPo.Name);
                            _prop                       = obj.GetOrAddComponent<PropBehaviour>();
                            //_prop.Type                  = (PropType)CoinPo.Type;
                            _prop.Type                  = _updateInfo.type;
                            _prop.transform.position    = _updateInfo.pos;
                            //_prop.LocalPercent          = _updateInfo.LocatePercent;
                            _prop.LocalPercent          = _updateInfo.LocatePercent - 0.2f;
                            _prop.Score                 = CoinPo.Score;
                            _prop.Assaultable           = CoinPo.IsEnermy == 1 ? true : false;
                            _prop.BornVolumeName        = CoinPo.BornVolumeName;
                            _prop.DieVolumeName         = CoinPo.DieVolumeName;
                            _prop.DamageValue           = CoinPo.Damage;
                            _prop.DamagePlane           = CoinPo.DamagePlane;
                            _prop.BornEffect            = CoinPo.BornEffect;
                            _prop.DieEffect             = CoinPo.DieEffect;
                            _prop.Owner                 = string.Empty;

                        }
                        else
                        {
                            _prop                           = null;
                            _updateInfo.used                = true;
                            GameObject obj                  = ioo.poolManager.Spawn(_updateInfo.name);
                            _prop                           = obj.GetOrAddComponent<PropBehaviour>();
                            _prop.Type                      = _updateInfo.type;
                            _prop.transform.position        = _updateInfo.pos;
                            _prop.LocalPercent              = _updateInfo.LocatePercent;
                            _prop.Score                     = _updateInfo.score;
                            _prop.Assaultable               = _updateInfo.Assaultable;
                            _prop.BornVolumeName            = _updateInfo.BornVolumeName;
                            _prop.DieVolumeName             = _updateInfo.DieVolumeName;
                            _prop.DamageValue               = _updateInfo.DamageValue;
                            _prop.DamagePlane               = _updateInfo.DamagePlane;
                            _prop.BornEffect                = _updateInfo.BornEffect;
                            _prop.DieEffect                 = _updateInfo.DieEffect;
                            _prop.Owner                     = string.Empty;
                        }

                        Vector3 rightPos    = PathManager.Instance.PathInfo1.GetRightPos(_updateInfo.LocatePercent);
                        Vector2 upPos       = PathManager.Instance.PathInfo1.GetUpPos(_updateInfo.LocatePercent);

                        _prop.Spawn(rightPos, upPos, Count);
                        if (!_updateInfo.isMidle)
                        {
                            if (_updateInfo.isLeft)
                            {
                                _prop.LocateLeft();
                            }
                            else
                            {
                                _prop.LocateRight();
                            }
                        }

                        _prop.transform.LookAt(_updateInfo.nexPos);

                        PropList.Add(_prop);
                        ++Count;
                        Count %= 5;
                    }
                }
                else
                {
                    break;
                }
                _curIndex++;
                if (_curIndex >= PosInfoList.Count)
                {
                    _curIndex = 0;
                    ResetPropInfo();
                }
            }
        }

        for (int i = 0; i < PropList.Count; ++i)
        {
            if (PropList[i].LocalPercent < percent - 0.002f)
            {
                DespawnPropList.Add(PropList[i]);
            }
        }

        for (int i = 0; i < DespawnPropList.Count; ++i)
        {
            PropBehaviour pb = DespawnPropList[i];
            PropList.Remove(pb);
            pb.DeSpawn();
        }

        DespawnPropList.Clear();
      
        #endregion
    }

    /// <summary>
    /// 在指定位置生成指定类型道具
    /// </summary>
    /// <param name="type"></param>
    /// <param name="pos"></param>
    /// <param name="percent"></param>
    public void SpawnPropInPos(PropType type, Vector3 pos, float percent)
    {
        PropBehaviour pb = null;
        if (type == PropType.Coin)
        {
            GameObject obj          = ioo.poolManager.Spawn(CoinPo.Name);
            pb                      = obj.GetOrAddComponent<PropBehaviour>();
            pb.Type                 = PropType.Coin;
            pb.transform.position   = pos;
            pb.LocalPercent         = percent;
            pb.Score                = CoinPo.Score;
            pb.Assaultable          = false;
            pb.BornVolumeName       = CoinPo.BornVolumeName;
            pb.DieVolumeName        = CoinPo.DieVolumeName;
            pb.DamageValue          = CoinPo.Damage;
            pb.DamagePlane          = CoinPo.DamagePlane;
            pb.BornEffect           = CoinPo.BornEffect;
            pb.DieEffect            = CoinPo.DieEffect;
            pb.Owner                = string.Empty;
        }

        if (type == PropType.Diamond)
        {
            GameObject obj          = ioo.poolManager.Spawn(DiamondPo.Name);
            pb                      = obj.GetOrAddComponent<PropBehaviour>();
            pb.Type                 = PropType.Diamond;
            pb.transform.position   = pos;
            pb.LocalPercent         = percent;
            pb.Score                = DiamondPo.Score;
            pb.Assaultable          = false;
            pb.BornVolumeName       = DiamondPo.BornVolumeName;
            pb.DieVolumeName        = DiamondPo.DieVolumeName;
            pb.DamageValue          = DiamondPo.Damage;
            pb.DamagePlane          = DiamondPo.DamagePlane;
            pb.BornEffect           = DiamondPo.BornEffect;
            pb.DieEffect            = DiamondPo.DieEffect;
            pb.Owner                = string.Empty;
        }

        PropList.Add(pb);
    }

    private float SuiltableDis = 1;
    private float SuiltPercent;
    public float NearestPercent(Vector3 pos)
    {
        float dis = 500;
        for (int i = 0; i < PosInfoList.Count; ++i )
        {
            Vector3 dir = PathManager.Instance.PathInfo1.GetRightPos(PosInfoList[i].LocatePercent) - pos;
            if (dir.magnitude <= SuiltableDis)
            {
                SuiltPercent = PosInfoList[i].LocatePercent;
                return SuiltPercent;
            }
            else
            {
                if (dir.magnitude < dis)
                {
                    dis = dir.magnitude;
                    SuiltPercent = PosInfoList[i].LocatePercent;
                }
            }
        }
        return SuiltPercent;
    }
    #endregion
   
    #region Pirvate Function
    private void ResetPropInfo()
    {
        for (int j = 0; j < PosInfoList.Count; ++j)
        {
            _updateInfo = PosInfoList[j];
            _updateInfo.used = false;
            PosInfoList[j] = _updateInfo;
        }
    }

    int type = 0;
    private float GenProp(PropsPO po, float startPrecent)
    {
        int num             = Random.Range(po.MinNumber, po.MaxNumber);
        bool left           = UnityEngine.Random.Range(0, 2) == 0;
        float curPrecent    = startPrecent;
        float pathLength    = PathManager.Instance.PathInfo1.PathLength;
        float offsetPrecent = 0;


        for (int i = 0; i < num; ++i)
        {
            PointInfo info = new PointInfo();

            info.name               = po.Name;
            info.type               = (PropType)po.Type;
            info.LocatePercent      = curPrecent;
            info.score              = po.Score;
            info.Assaultable        = po.IsEnermy == 1 ? true : false;
            info.used               = false;
            info.BornVolumeName     = po.BornVolumeName;
            info.DieVolumeName      = po.DieVolumeName;
            info.DamageValue        = po.Damage;
            info.DamagePlane        = po.DamagePlane;
            info.BornEffect         = po.BornEffect;
            info.DieEffect          = po.DieEffect;

            if (info.type == PropType.Wall)
            {
                info.isMidle = true;
                type = 0;
            }
            else
                info.isMidle = false;

            Vector3 nextPos = Vector3.zero;

            // 下
            if (type == 0)
            {
                info.pos = PathManager.Instance.PathInfo1.GetPos(curPrecent);
                nextPos = PathManager.Instance.PathInfo1.GetPos(curPrecent + Step);
            }
            // 上
            else if (type == 1)
            {
                info.pos = PathManager.Instance.PathInfo1.GetPos(curPrecent) + (PathManager.Instance.PathInfo1.GetUpPos(curPrecent) - PathManager.Instance.PathInfo1.GetPos(curPrecent)).normalized * 3;
                nextPos = PathManager.Instance.PathInfo1.GetPos(curPrecent + Step) + (PathManager.Instance.PathInfo1.GetUpPos(curPrecent + Step) - PathManager.Instance.PathInfo1.GetPos(curPrecent + Step)).normalized * 3;
            }
            // 中
            else
            {
                info.pos = (2 * PathManager.Instance.PathInfo1.GetPos(curPrecent) + (PathManager.Instance.PathInfo1.GetUpPos(curPrecent) - PathManager.Instance.PathInfo1.GetPos(curPrecent)).normalized * 3) * 0.5f;
                nextPos = (2 * PathManager.Instance.PathInfo1.GetPos(curPrecent + Step) + (PathManager.Instance.PathInfo1.GetUpPos(curPrecent + Step) - PathManager.Instance.PathInfo1.GetPos(curPrecent + Step)).normalized * 3) * 0.5f;
            }

            if (info.pos != nextPos)
                if (info.pos != nextPos)
                {
                    info.nexPos = nextPos;
                }

            if (left)
            {
                info.isLeft = true;
            }
            else
            {
                info.isLeft = false;
            }

            offsetPrecent = po.Offset / pathLength;
            curPrecent += offsetPrecent;

            PosInfoList.Add(info);
        }

        ++type;
        type %= 3; 

        return offsetPrecent * num;
    }
    #endregion
  
}
