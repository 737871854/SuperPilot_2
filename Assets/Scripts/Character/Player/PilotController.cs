/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   PilotController.cs
 * 
 * 简    介:    负责飞机速度的控制，以及响应玩家操作和外部触发
 * 
 * 创建标识：   Pancake 2017/5/25 15:32:46
 * 
 * 修改描述：   延用了欧阳飞机速度控制方案
 * 
 */

using UnityEngine;
using System.Collections;
using Need.Mx;

public class PilotController : MonoBehaviour
{
    private enum E_State
    {
        /// <summary>
        /// 正常行驶
        /// </summary>
        Normal,
        /// <summary>
        /// 加速
        /// </summary>
        SpeedUp,
        /// <summary>
        /// 保持速度
        /// </summary>
        KeepSpeed,
        /// <summary>
        /// 减速
        /// </summary>
        SpeedDown,
        /// <summary>
        /// 聚气加速
        /// </summary>
        Gather,
    }

    private enum E_Step
    {
        Ready,
        Play,
        Land,
        Cointinue,
    }

    private Player _Player;

    //基本平均速度
    public float baseSpeed;
    //飞机加速
    private bool speedUp = false;
    //飞机减速
    private bool speedDown = false;

    [Range(0, 1)]
    public float SpeedTimeFactor = 1.0f;    // 左右移动速度因子       
    [Range(0, 1)]
    public float RotateAngleFactor = 1.0f;  // 

    private float MaxRollAngle = 50;
    private float MaxPitchAngle = 25;

    private float MaxRollView = 20;

    [System.NonSerialized]
    public bool CanControlled;

    private bool HasCameraOp;
    private float RollRestCameraTime = 0.1f;

    // 绕Z轴旋转
    private bool HasRollOP;
    // 复位Roll等待时间
    private float RollRestTime = 0.1f;
    // 绕X轴旋转
    private bool HasPitchOP;
    // 复位Pitch等待时间
    private float PitchRestTime = 0.1f;

    // 路宽
    private float RoadSize = 3;

    private E_State _State = E_State.Normal;
    private E_Step _Step = E_Step.Ready;
    // 最小行驶速度因子
    private float _MinFactor = 0.45f;
    // 最大行驶速度因子
    private float _MaxFactor = 1.35f;
    // 正常行驶速度因子 
    private float _NormalFactor = 0.9f;
    // 当前行驶速度因子
    public float  _CurFactor;
    public float _LandFactor;
    // 保持当前速度时间
    private float _LastTime;

    // 磁铁
    private bool _AbilityMagnet;
    private float _MagnetTime;

    // 导弹
    [System.NonSerialized]
    public bool _AbilityMissile;

    private float Height;

    public bool SlowDown;

    #region Unity CallBack
    void Start()
    {
        SlowDown        = false;
        _LandFactor     = 1;
        CanControlled   = false;
        _Player         = ioo.gameMode.Player;

        if (ioo.gameMode.PlayerName == "Player2")
            Height = 2.6f;
        else
            Height = 3.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (SlowDown)
        {
            baseSpeed -= Time.deltaTime * 30;
        }

        if (JiGuangTrigger > 0)
            JiGuangTrigger -= Time.deltaTime;
        else
            JiGuangTrigger = 0;

        #region 速度因子控制
        if (_Player.AnimController.State == AnimController.E_State.TakeOff || 
            _Player.AnimController.State == AnimController.E_State.Land || 
            _Player.AnimController.State == AnimController.E_State.Down)
        {
            _CurFactor = 0;
            return;
        }

        if (_Step == E_Step.Ready)
        {
            if (_CurFactor < _NormalFactor)
            {
                _CurFactor += Time.deltaTime;
            }
            else
            {
                _CurFactor = _NormalFactor;
            }
            return;
        }

        if (_Step == E_Step.Land)
        {
            if (_CurFactor > _MinFactor)
            {
                _CurFactor -= Time.deltaTime;
            }
            else
            {
                _CurFactor = _MinFactor;
            }
            return;
        }

        if (_Step == E_Step.Cointinue)
        {
            if (_CurFactor > 0)
            {
                _CurFactor -= Time.deltaTime;
            }
            else
            {
                _CurFactor = 0;
            }
            return;
        }

        switch (_State)
        {
            case E_State.Normal:
                if (_CurFactor < _NormalFactor)
                {
                    _CurFactor += Time.deltaTime * _NormalFactor;
                }
                else
                {
                    _CurFactor = _NormalFactor;
                }
                break;
            case E_State.SpeedUp:
            case E_State.Gather:
                _Player.CamerStartSpeedUp();
                if (_CurFactor >= _MaxFactor)
                {
                    _CurFactor = _MaxFactor;
                    _State = E_State.KeepSpeed;
                }
                else
                {
                    _CurFactor += Time.deltaTime * (_MaxFactor - _NormalFactor);
                }
                _Player.EffectSpeedUp.SetActive(true);
                break;
            case E_State.SpeedDown:
                _Player.CameraEndSpeedUp();
                if (_CurFactor <= _NormalFactor)
                {
                    _State = E_State.Normal;
                    _CurFactor = _NormalFactor;
                }
                else
                {
                    _CurFactor -= Time.deltaTime * (_MaxFactor - _NormalFactor);
                }
                _Player.EffectSpeedUp.SetActive(false);
                break;
            case E_State.KeepSpeed:

                // 防止续币后导致飞机停止
                if (_CurFactor < _MaxFactor)
                    _CurFactor += Time.deltaTime * (_MaxFactor - _NormalFactor);
                else
                    _CurFactor = _MaxFactor;

                if (_LastTime <= 0)
                {
                    _State = E_State.SpeedDown;
                     ioo.audioManager.StopBackMusic("Music_Speed_Up");
                     EventDispatcher.TriggerEvent(EventDefine.Event_Engine_Speed_Up, false);
                }
                else
                    _LastTime -= Time.deltaTime;
                break;
        }

        #region 依据路面上坡下坡调整速度
        if (_State == E_State.Normal)
        {
            Vector3 pathDir = _Player.nextPoint - _Player.curPoint;
            pathDir.Normalize();
            float Angle = Vector3.Angle(pathDir, Vector3.up);
            _LandFactor -= Mathf.Cos(Angle * Mathf.Deg2Rad) * _NormalFactor * 0.1f;
        }
        #endregion

        _LandFactor = Mathf.Clamp(_CurFactor, _MinFactor, _MaxFactor);

        #endregion

        #region 吸附能力
        if(_MagnetTime > 0)
        {
            _MagnetTime -= Time.deltaTime;
            Check();
        }
        else
        {
            _MagnetTime = 0;
            _Player.CloseMangent();
        }
        #endregion

        #region 飞行Roll旋转
        if (RollRestTime > 0)
        {
            RollRestTime -= Time.deltaTime;
        }
        else
        {
            RollRestTime = 0;
            HasRollOP = false;
        }

        // 飞机Roll 复位
        if (!HasRollOP)
        {
            float angle = AngleCorrection(_Player.HelicopterBody.localEulerAngles.z);
            if (angle != 0)
            {
                angle = angle / Mathf.Abs(angle);
                _Player.HelicopterBody.localEulerAngles -= new Vector3(0, 0, angle);
                float z = AngleCorrection(_Player.HelicopterBody.localEulerAngles.z);
                if ((angle == 1 && z < 0) || (angle == -1 && z > 0))
                    _Player.HelicopterBody.localEulerAngles = new Vector3(_Player.HelicopterBody.localEulerAngles.x, _Player.HelicopterBody.localEulerAngles.y, 0);
            }
        }

        // 相机Roll 复位
        if (RollRestCameraTime > 0)
        {
            RollRestCameraTime -= Time.deltaTime;
        }
        else
        {
            RollRestCameraTime = 0;
            HasCameraOp = false;
        }

        if (!HasCameraOp && _Player.CanRotation())
        {
            float angle = AngleCorrection(_Player.CameraObj.transform.localEulerAngles.y);
            if (angle != 0)
            {
                angle = angle / Mathf.Abs(angle);
                _Player.CameraObj.transform.localEulerAngles -= new Vector3(0, angle * 0.3f, 0);
                float y = AngleCorrection(_Player.CameraObj.transform.localEulerAngles.y);
                if ((angle == 1 && y < 0) || (angle == -1 && y > 0))
                    _Player.CameraObj.transform.localEulerAngles = new Vector3(_Player.CameraObj.transform.localEulerAngles.x, 0, _Player.CameraObj.transform.localEulerAngles.z);
            }
        }
        #endregion

        #region 飞行Pitch旋转
        if (PitchRestTime > 0)
        {
            PitchRestTime -= Time.deltaTime;
        }
        else
        {
            PitchRestTime = 0;
            HasPitchOP = false;
        }

        // 飞机Pitch 复位
        if (!HasPitchOP)
        {
            float angle = AngleCorrection(_Player.HelicopterBody.localEulerAngles.x);
            if (angle != 0)
            {
                angle = angle / Mathf.Abs(angle);
                _Player.HelicopterBody.localEulerAngles -= new Vector3(angle, 0, 0);
                float z = AngleCorrection(_Player.HelicopterBody.localEulerAngles.x);
                if ((angle == 1 && z < 0) || (angle == -1 && z > 0))
                    _Player.HelicopterBody.localEulerAngles = new Vector3(0, _Player.HelicopterBody.localEulerAngles.y, _Player.HelicopterBody.localEulerAngles.z);
            }
        }
        #endregion
    }

    void OnTriggerEnter(Collider other)
    {
        if (_Player.BossStep)
        {
            return;
        }
        
        //  道具类
        if (other.tag.Equals(GameTag.Props))
        {
            PropBehaviour pb = other.transform.parent.GetComponent<PropBehaviour>();

            if (pb == null)
                pb = other.transform.parent.parent.GetComponent<PropBehaviour>();

            if (pb.Owner == GameTag.Player)
                return;

            float damage0 = pb.DamageValue;
            float damage1 = pb.DamagePlane;

            if (damage0 < 0 && (_Player.ShieldLife + damage0) < 0)
            {
                EventDispatcher.TriggerEvent(EventDefine.Event_Tips_Type_By_Int, 0, pb.transform.position, (int)(Mathf.Abs(_Player.ShieldLife + damage0)));
            }
            else if (damage0 > 0)
            {
                EventDispatcher.TriggerEvent(EventDefine.Event_Tips_Type_By_Int, 1, pb.transform.position, (int)(Mathf.Abs(damage0)));
            }

            _Player.OnDamage(damage0, damage1);

            pb.Trigger();

            switch (pb.Type)
            {
                case PropType.Coin:
                    PropsManager.Instance.AddDespawnNormal(pb);
                    _Player.OnCoin(pb.Score);
                    break;
                case PropType.Fix:
                    if (_Player.Data.ConditionRate < 0.9f)
                        ioo.audioManager.PlayPersonSound("Person_Sound_Fix");
                    if (_Player.Data.ConditionRate < 1)
                        _Player.EffectFixed.SetActive(true);

                    PropsManager.Instance.AddDespawnNormal(pb);
                    _Player.OnFix(pb.Score);
                    EventDispatcher.TriggerEvent(EventDefine.Event_Tips_Type_By_Int, 4, pb.transform.position, 0);
                    break;
                case PropType.Diamond:
                    PropsManager.Instance.AddDespawnNormal(pb);
                    _Player.OnDiamond(pb.Score);
                    break;
                case PropType.Fuel:
                    ioo.audioManager.PlayPersonSound("Person_Sound_Time_Add");
                    PropsManager.Instance.AddDespawnNormal(pb);
                    _Player.OnFuel(pb.Score);
                    break;
                case PropType.Shield:
                    ioo.audioManager.PlayPersonSound("Person_Sound_Shield");
                    PropsManager.Instance.AddDespawnNormal(pb);
                    _Player.OnShield(pb.Score);
                    EventDispatcher.TriggerEvent(EventDefine.Event_Tips_Type_By_Int, 2, pb.transform.position, 0);
                    break;
                case PropType.Magnet:
                    _MagnetTime = 8;
                    _AbilityMagnet = true;
                    PropsManager.Instance.AddDespawnNormal(pb);
                    _Player.OnMagnet(pb.Score);
                    break;
                case PropType.Wall:
                    EventDispatcher.TriggerEvent(EventDefine.Event_OnTrigger_Wall);
                    _Player.OnWall(pb.Score);
                    break;
                //case PropType.FeiQi:
                //    PropsManager.Instance.AddDespawnNormal(pb);
                //    break;
                //case PropType.Bat:
                //    PropsManager.Instance.AddDespawnNormal(pb);
                //    break;
                case PropType.Mine:
                    PropsManager.Instance.AddDespawnNormal(pb);
                    break;
                case PropType.Missile:
                    ioo.audioManager.PlayPersonSound("Person_Sound_Missile");
                    _AbilityMissile = true;
                    _Player.AbilityMissile();
                    PropsManager.Instance.AddDespawnNormal(pb);
                    EventDispatcher.TriggerEvent(EventDefine.Event_Tips_Type_By_Int, 5, pb.transform.position, 0);
                    break;
                case PropType.Engine:
                    ioo.audioManager.PlayPersonSound("Person_Sound_Speed_Up");
                    PropsManager.Instance.AddDespawnNormal(pb);
                    EventDispatcher.TriggerEvent(EventDefine.Event_Tips_Type_By_Int, 3, pb.transform.position, 0);
                    break;
            }
            SetFactorP(pb.Type);
        }

        // 武器类
       if (other.tag.Equals(GameTag.Weapon))
       {
           WeaponBehaviour wb = other.gameObject.GetComponent<WeaponBehaviour>();
           if (wb.Owner == GameTag.Player)
               return;

           _Player.OnDamage(wb.DamageValue, wb.DamagePlane);

           wb.Trigger();

           switch(wb.Type)
           {
               case WeaponType.Mine:
               case WeaponType.Missle:
               case WeaponType.Laser:
               case WeaponType.FireBal:
               case WeaponType.Missle2:
                   WeaponManager.Instance.AddDespawnWeapon(wb);
                   break;
               case WeaponType.Stone:
                   _Player.ShackCameraByPosition(1);
                   WeaponManager.Instance.AddDespawnWeapon(wb);
                   EffectManager.Instance.Spawn(EffectName.Effect_Aerolite_baoz, wb.transform.position);
                   if (_Player.ShieldLife + wb.DamageValue < 0)
                        EventDispatcher.TriggerEvent(EventDefine.Event_Tips_Type_By_Int, 0, wb.transform.position, (int)(Mathf.Abs(_Player.ShieldLife + wb.DamageValue)));
                   break;
           }
         
           SetFactorW(wb.Type);
       }

        // 广告牌
        if (other.tag.Equals(GameTag.GuangGaoPai))
        {
            ioo.audioManager.PlaySound2D("SFX_Sound_GuangGao");
        }

        // 敌人
       if (other.tag.Equals(GameTag.Enemy))
       {
           _Player.OnDamage(-3, -3);
           _Player.ShackCameraByPosition(1);
       }

       // 加速环
       if (other.tag.Equals(GameTag.Glow))
       {
           // 和加速引擎道具一样的效果
           SetFactorP(PropType.Engine);
       }

        // 虫怪
       if (other.tag.Equals(GameTag.Bug))
       {
           _Player.OnDamage(-2, -2);
           SetFactorP(PropType.Bug);
           _Player.ShackCameraByPosition(0.2f);
           if (_Player.ShieldLife - 2 < 0)
                EventDispatcher.TriggerEvent(EventDefine.Event_Tips_Type_By_Int, 0, other.transform.position, (int)Mathf.Abs(2 - _Player.ShieldLife));
       }

        // 特效树
       if (other.tag.Equals(GameTag.Tree))
       {
           _Player.ShackCameraByPosition(1);
       }

       if (other.tag.Equals(GameTag.CameraStop))
       {
           if (ioo.gameMode.State == GameState.Back)
           {
               SlowDown = true;
           }
       }
    }

    private float JiGuangTrigger = 1;
    void OnTriggerStay(Collider other)
    {
        if (JiGuangTrigger > 0)
            return;

        // 激光
        if (other.tag.Equals(GameTag.JiGuang))
        {
            JiGuangTrigger = 1.5f;
            _Player.OnDamage(-1, -1);
            _Player.ShackCameraByPosition(0.5f, 0.5f);
            EventDispatcher.TriggerEvent(EventDefine.Event_On_Trigger_JiGuang_Weapon, true);
            if (_Player.ShieldLife - 1 < 0)
                EventDispatcher.TriggerEvent(EventDefine.Event_Tips_Type_By_Int, 0, transform.position, (int)(Mathf.Abs(_Player.ShieldLife - 1)));
        }
    }

    void OnTriggerExit(Collider other)
    {
        // 激光
        if (other.tag.Equals(GameTag.JiGuang))
        {
            EventDispatcher.TriggerEvent(EventDefine.Event_On_Trigger_JiGuang_Weapon, false);
        }
    }
    #endregion

    #region Private Function
    //欧拉角转换
    private static float AngleCorrection(float angle)
    {
        if (angle > 180) return angle - 360;
        if (angle < -180) return angle + 360;
        return angle;
    }

    // 检查周围可以吸附的道具
    private void Check()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 50);
        if (colliders.Length == 0)
            return;
        for (int i = 0; i < colliders.Length; ++i )
        {
            if (colliders[i].transform.parent == null)
                continue;
            PropBehaviour pb = colliders[i].transform.parent.GetComponent<PropBehaviour>();
            if (pb == null)
                continue;
            if (IsEnemy(pb))
                continue;

            pb.SetMoveToTargetTransform(transform);
        }
    }

    // 道具是否可以被吸附
    private bool IsEnemy(PropBehaviour pb)
    {
        switch (pb.Type)
        {
            case PropType.Coin:
            case PropType.Diamond:
            case PropType.Fuel:
            case PropType.Magnet:
            case PropType.Shield:
            case PropType.Engine:
            case PropType.Missile:
            case PropType.Fix:
                return false;
        }
        return true;
    }
    #endregion
   
    #region Public Function

    /// <summary>
    /// 初始化
    /// </summary>
    public void ReadyPath()
    {
        _Step = E_Step.Ready;

        CanControlled = false;

        baseSpeed = _Player.pathInfo.PathLength / _Player.OverTime;//基本速度
    }

    public void GamePath()
    {
        _Step = E_Step.Play;
        
        baseSpeed = _Player.pathInfo.PathLength / _Player.OverTime;//基本速度
    }

    public void LandPath()
    {
        _Step = E_Step.Land;

        CanControlled = false;

        baseSpeed = _Player.pathInfo.PathLength / _Player.OverTime;//基本速度
    }

    public void PlayToContinue()
    {
        CanControlled = false;

        _Step = E_Step.Cointinue;
    }

    public void ContinueToPlay()
    {
        CanControlled =true;

        _Step = E_Step.Play;
    }

    /// <summary>
    /// 响应改变速度的事件
    /// </summary>
    /// <param name="type"></param>
    public void SetFactorP(PropType type)
    {
        if (_State == E_State.SpeedUp || _State == E_State.KeepSpeed || _State == E_State.Gather)
        {
            if (type == PropType.Engine)
            {
                _LastTime = 3;
            }
            return;
        }

        switch (type)
        {
            case PropType.Mine:
                _CurFactor = 0.45f * _NormalFactor;
                _Player.CameraEndSpeedUp();
                break;
            case PropType.Engine:
                _LastTime = 3;
                ioo.audioManager.StopBackMusic("Music_Speed_Up");
                ioo.audioManager.PlayBackMusic("Music_Speed_Up");
                _State = E_State.SpeedUp;
                EventDispatcher.TriggerEvent(EventDefine.Event_Engine_Speed_Up, true);
                break;
            case PropType.Wall:
                _CurFactor = 0.45f * _NormalFactor;
                _Player.CameraEndSpeedUp();
                break;
            case PropType.Bug:
                _CurFactor = 0.45f * _NormalFactor;
                _Player.CameraEndSpeedUp();
                break;
        }
    }

    public void SetFactorW(WeaponType type)
    {
        if (_State == E_State.SpeedUp || _State == E_State.KeepSpeed || _State == E_State.Gather)
            return;

        switch(type)
        {
            case WeaponType.Stone:
                _CurFactor = 0.45f * _NormalFactor;
                _Player.CameraEndSpeedUp();
                //_LastTime = 1;
                break;
        }
    }

    public void UseGather()
    {
        if (_State == E_State.SpeedUp || _State == E_State.KeepSpeed)
        {
            _LastTime = 3;
            return;
        }

        _State = E_State.Gather;
        _LastTime += 3 * _Player.Data.Gather;
        ioo.audioManager.StopBackMusic("Music_Speed_Up");
        ioo.audioManager.PlayBackMusic("Music_Speed_Up");
        EventDispatcher.TriggerEvent(EventDefine.Event_Engine_Speed_Up, true);
    }

    //上升
    public void PullUp()
    {
        if (!CanControlled)
            return;

        HasPitchOP = true;
        PitchRestTime = 0.1f;
        if (AngleCorrection(_Player.HelicopterBody.localEulerAngles.x) > -MaxPitchAngle)
            _Player.HelicopterBody.localEulerAngles -= new Vector3(RotateAngleFactor, 0, 0);


        if (_Player.HelicopterBody.localPosition.y < Height)
            _Player.HelicopterBody.localPosition += new Vector3(0, 3 * SpeedTimeFactor * Time.deltaTime, 0);

        _Player.ShowUIMoveDir = Vector3.up;
    }

    //下降
    public void PullDown()
    {
        if (!CanControlled)
            return;

        HasPitchOP = true;
        PitchRestTime = 0.1f;
        if (AngleCorrection(_Player.HelicopterBody.localEulerAngles.x) < MaxPitchAngle)
            _Player.HelicopterBody.localEulerAngles += new Vector3(RotateAngleFactor, 0, 0);

        if (_Player.HelicopterBody.localPosition.y > 0)
            _Player.HelicopterBody.localPosition -= new Vector3(0, 3 * SpeedTimeFactor * Time.deltaTime, 0);

        _Player.ShowUIMoveDir = Vector3.down;
    }

    //左转
    public void TurnLeft()
    {
        if (!CanControlled)
            return;

        HasRollOP       = true;
        RollRestTime    = 0.1f;
        if (AngleCorrection(_Player.HelicopterBody.localEulerAngles.z) < MaxRollAngle)
            _Player.HelicopterBody.localEulerAngles += new Vector3(0, 0, RotateAngleFactor);

        if (_Player.HelicopterBody.localPosition.x > -RoadSize)
            _Player.HelicopterBody.localPosition -= new Vector3(3 * SpeedTimeFactor * Time.deltaTime, 0, 0);

        if (!_Player.CanRotation())
            return;
        HasCameraOp = true;
        RollRestCameraTime = 0.1f;
        if (AngleCorrection(_Player.CameraObj.transform.localEulerAngles.y) > -MaxRollView)
            _Player.CameraObj.transform.localEulerAngles -= new Vector3(0, RotateAngleFactor * 0.2f, 0);

        _Player.ShowUIMoveDir = Vector3.left;

    }

    //右转
    public void TurnRight()
    {
        if (!CanControlled)
            return;

        HasRollOP       = true;
        RollRestTime    = 0.1f;
        if (AngleCorrection(_Player.HelicopterBody.localEulerAngles.z) > -MaxRollAngle)
            _Player.HelicopterBody.localEulerAngles -= new Vector3(0, 0, RotateAngleFactor);

        if (_Player.HelicopterBody.localPosition.x < RoadSize)
            _Player.HelicopterBody.localPosition += new Vector3(3 * SpeedTimeFactor * Time.deltaTime, 0, 0);

        if (!_Player.CanRotation())
            return;
        HasCameraOp = true;
        RollRestCameraTime = 0.1f;
        if (AngleCorrection(_Player.CameraObj.transform.localEulerAngles.y) < MaxRollView)
            _Player.CameraObj.transform.localEulerAngles += new Vector3(0, RotateAngleFactor * 0.2f, 0);

        _Player.ShowUIMoveDir = Vector3.right;
    }

    public void NormalSpeed()
    {
        _CurFactor = _NormalFactor;
    }
    ////复位
    //public void TurnBack()
    //{

    //    if (AngleCorrection(_Player.HelicopterBody.localEulerAngles.z) >= 0.5f)
    //    {
    //        _Player.HelicopterBody.localEulerAngles -= new Vector3(0, 0, RotateAngleFactor);
    //    }
    //    else if (AngleCorrection(_Player.HelicopterBody.localEulerAngles.z) <= -0.5f)
    //    {
    //        _Player.HelicopterBody.localEulerAngles += new Vector3(0, 0, RotateAngleFactor);
    //    }
    //}

    #endregion
   

}
