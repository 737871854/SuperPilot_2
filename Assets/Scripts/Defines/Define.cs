/*
 * Copyright (c) 
 * 
 * 文件名称：   Define.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/3/28 17:13:21
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;


#region 委托
public delegate void OnTouchEventHandle(GameObject _listener, object _args, params object[] _params);

public delegate void StateChangedEvent(object sender, EnumObjectState newState, EnumObjectState oldState);
#endregion

public static class SceneName
{
    public const string CheckGUID   = "CheckGUID";
    public const string Main        = "Main";
    public const string Coin        = "Coin";
    public const string Loading     = "Loading";
    public const string City        = "City";
    public const string Gorge       = "Gorge";
}

/// <summary>
/// 游戏状态
/// </summary>
public enum GameState
{
    Check,
    Idle,
    Coin,
    Waitting,
    Setting,
    Select,
    Loading,
    Play,
    Continue,
    Back,
    Summary,
}

public enum PilotType
{
    Player,
    AI,
}

public enum E_Map
{
    /// <summary>
    /// 城市
    /// </summary>
    City,
    /// <summary>
    /// 峡谷
    /// </summary>
    Gorge,
}

/// <summary>
/// 对象当前状态 
/// </summary>
public enum EnumObjectState
{
    /// <summary>
    /// The none.
    /// </summary>
    None,
    /// <summary>
    /// The initial.
    /// </summary>
    Initial,
    /// <summary>
    /// The loading.
    /// </summary>
    Loading,
    /// <summary>
    /// The ready.
    /// </summary>
    Ready,
    /// <summary>
    /// The disabled.
    /// </summary>
    Disabled,
    /// <summary>
    /// The closing.
    /// </summary>
    Closing
}

/// <summary>
/// Enum user interface type.
/// UI面板类型
/// </summary>
public enum EnumUIType : int
{
    /// <summary>
    /// The none.
    /// </summary>
    None = -1,
    /// <summary>
    /// UI Root
    /// </summary>
    UIRoot,
    /// <summary>
    /// 投币面板
    /// </summary>
    PanelCoin,
    /// <summary>
    /// 后台面板
    /// </summary>
    PanelSetting,
    /// <summary>
    /// 选择地图面板
    /// </summary>
    PanelSelect,
    /// <summary>
    /// 游戏主面板
    /// </summary>
    PanelMain,
    /// <summary>
    /// 结算面板
    /// </summary>
    PanelSummary,
    /// <summary>
    /// 硬件检测面板
    /// </summary>
    PanelCheckHard,
    /// <summary>
    /// 加载界面
    /// </summary>
    PanelLoading,

}

/// <summary>
/// 路径定义。
/// </summary>
public static class UIPathDefines
{
    /// <summary>
    /// UI预设。
    /// </summary>
    public static string UI_PREFAB = string.Empty;

    /// <summary>
    /// Gets the type of the prefab path by.
    /// </summary>
    /// <returns>The prefab path by type.</returns>
    /// <param name="_uiType">_ui type.</param>
    public static string GetPrefabPathByType(EnumUIType _uiType)
    {
        UI_PREFAB = SettingManager.Instance.GameLanguage == 0 ? "Prefabs/UI/Chinese/" : "Prefabs/UI/English/";

        string _path = string.Empty;
        switch (_uiType)
        {
            case EnumUIType.UIRoot:
                _path = UI_PREFAB + "UIRoot";
                break;
            // UI
            case EnumUIType.PanelCoin:
                _path = UI_PREFAB + "PanelCoin";
                break;
            case EnumUIType.PanelSetting:
                _path = UI_PREFAB + "PanelSetting";
                break;
            case EnumUIType.PanelSelect:
                _path = UI_PREFAB + "PanelSelect";
                break;
            case EnumUIType.PanelMain:
                _path = UI_PREFAB + "PanelMain";
                break;
            case EnumUIType.PanelSummary:
                _path = UI_PREFAB + "PanelSummary";
                break;
            case EnumUIType.PanelCheckHard:
                _path = UI_PREFAB + "PanelCheckHard";
                break;
            case EnumUIType.PanelLoading:
                _path = UI_PREFAB + "PanelLoading";
                break;
            default:
                Debug.Log("Not Find EnumUIType! type: " + _uiType.ToString());
                break;
        }
        return _path;
    }

    /// <summary>
    /// Gets the type of the user interface script by.
    /// </summary>
    /// <returns>The user interface script by type.</returns>
    /// <param name="_uiType">_ui type.</param>
    public static System.Type GetUIScriptByType(EnumUIType _uiType)
    {
        System.Type _scriptType = null;
        switch (_uiType)
        {
            case EnumUIType.PanelCoin:
                _scriptType = typeof(PanelCoinLogic);
                break;
            case EnumUIType.PanelSetting:
                _scriptType = typeof(PanelSettingLogic);
                break;
            case EnumUIType.PanelSelect:
                _scriptType = typeof(PanelSelectLogic);
                break;
            case EnumUIType.PanelMain:
                _scriptType = typeof(PanelMainLogic);
                break;
            case EnumUIType.PanelSummary:
                _scriptType = typeof(PanelSummaryLogic);
                break;
            case EnumUIType.PanelCheckHard:
                _scriptType = typeof(PanelCheckHardLogic);
                break;
            case EnumUIType.PanelLoading:
                _scriptType = typeof(PanelLoadingLogic);
                break;
            default:
                Debug.Log("Not Find EnumUIType! type: " + _uiType.ToString());
                break;
        }
        return _scriptType;
    }

}

public enum MovieType
{
    Welcome,
    Coin,

}

public class MoviePathDefines
{
    private static string Movie_Root_Path = "Movies";

    public string GetMoviePath(MovieType _type, int index)
    {
        string path = string.Empty;
        switch(_type)
        {
            case MovieType.Coin:
                path = Movie_Root_Path + "Coin_" + index;
                break;
        }
        return path;
    }
}

/// <summary>
/// 道具类型
/// </summary>
public enum PropType
{
    Coin    = 1,
    Fix     = 2,
    Diamond = 3,
    //FeiQi   = 4,
    Fuel    = 4,
    Magnet  = 5,
    Mine    = 6,
    Shield  = 7,
    //Bat     = 9,
    Missile = 8,
    Engine  = 9,
    Wall    = 10,


    // 已经被移除出道具，类型留下，方便使用，懒得在分
    Bug     = 11,
}

/// <summary>
/// NotificationName
/// </summary>
public class NotificationName
{
    /// <summary>
    /// 
    /// </summary>
    public static string STARTUP = "GameFacade_StartUp";

    public static string PanelCoin = "GameFacade_PanelCoin";

    public static string PanelSelect = "GameFacade_PanelSelect";

    public static string PanelMain = "GameFacade_PanMain";

    public static string PanelSummary = "GameFacade_PanelSummary";

    public static string PanelCheckHard = "PanelCheckHard";

    public static string PanelLoading = "PanelLoading";

    public static string PanelSetting = "PanelSetting";

}

/// <summary>
/// 
/// </summary>
public static class MyProxyName
{
    /// <summary>
    /// 
    /// </summary>
    public const string CountProxy = "CountProxy";

    /// <summary>
    /// 
    /// </summary>
    public const string Count2Proxy = "Count2Proxy";

    /// <summary>
    /// 投币界面Model
    /// </summary>
    public const string PanelCoinProxy = "PanelCoinProxy";

    public const string PanelMeterProxy = "PanelMeterProxy";

    public const string PanelSelectProxy = "PanelSelectProxy";

    public const string PanelMainProxy = "PanelMainProxy";

    public const string PanelSummaryProxy = "PanelSummaryProxy";

    public const string PanelCheckHardProxy = "PanelCheckHardProxy";

    public const string PanelSettingProxy = "PanelSettingProxy";

    public const string PanelLoadingProxy = "PanelLoadingProxy";

}

/// <summary>
/// 
/// </summary>
public static class MyProxyUpdated
{
    /// <summary>
    /// 测试
    /// </summary>
    public const string CountProxy_Updated = "CountProxy_Updated";

    /// <summary>
    /// 测试
    /// </summary>
    public const string Count2Proxy_Updated = "Count2Proxy_Updated";

    /// <summary>
    /// 投币面板 币数，币率更新
    /// </summary>
    public const string PanelCoinProxy_UpdatedView = "PanelCoinProxy_UpdatedView";

    /// <summary>
    /// 选择面板 地图，更新
    /// </summary>
    public const string PanelSelectProxy_UpdateCoin = "PanelSelectProxy_UpdateCoin";

    /// <summary>
    /// 主界面
    /// </summary>
    public const string PanelMainProxy_UpdateCoin = "PanelMainProxy_UpdateCoin";

    /// <summary>
    /// 结算界面
    /// </summary>
    public const string PanelSummary_Update = "PanelSummary_Update";

    /// <summary>
    /// 后台
    /// </summary>
    public const string PanelSetting_Update = "PanelSetting_Update";
    public const string PanelSetting_UpdateRate = "PanelSetting_UpdateRate";
    public const string PanelSetting_UpdateVolume = "PanelSetting_UpdateVolume";
    public const string PanelSetting_UpdateLanguage = "PanelSetting_UpdateLanguage";
    public const string PanelSetting_UpdateMonth = "PanelSetting_UpdateMonth";
    public const string PanelSetting_UpdateTotal = "PanelSetting_UpdateTotal";
    public const string PanelSetting_UpdateTicket = "PanelSetting_UpdateTicket";
    public const string Panelsetting_UpdateTime = "Panelsetting_UpdateTime";

    /// <summary>
    /// 进度条
    /// </summary>
    public const string PanelLoading_Update_Num = "PanelTest_Update_Num";
    public const string PanelLoading_Update_Coin = "PanelLoading_Update_Coin";
}


public static class MyMediatorName
{
    /// <summary>
    /// 
    /// </summary>
    public const string CountMediator = "CountMediator";

    /// <summary>
    /// 
    /// </summary>
    public const string Count2Mediator = "Count2Mediator";

    /// <summary>
    /// 投币面板中间层
    /// </summary>
    public const string PanelCoinMediator = "PanelCoinMediator";

    /// <summary>
    /// 选择面板中间层
    /// </summary>
    public const string PanelSelectMediator = "PanelSelectMediator";

    /// <summary>
    /// 主界面
    /// </summary>
    public const string PanelMainMediator = "PanelMainMediator";

    /// <summary>
    /// 结算界面
    /// </summary>
    public const string PanelSummaryMediator = "PanelSummaryMediator";

    /// <summary>
    /// 硬件检测界面
    /// </summary>
    public const string PanelCheckHardMediator = "PanelCheckHardMediator";

    /// <summary>
    /// 后台
    /// </summary>
    public const string PanelSettingMediator = "PanelSettingMediator";

    /// <summary>
    /// 进度条
    /// </summary>
    public const string PanelLoadingMediator = "PanelLoadingMediator";


}

public enum EnumTouchEventType
{
    OnClick,
    OnDoubleClick,
    OnDown,
    OnUp,
    OnEnter,
    OnExit,
    OnSelect,
    OnUpdateSelect,
    OnDeSelect,
    OnDrag,
    OnDragEnd,
    OnDrop,
    OnScroll,
    OnMove,
}

public class EventDefine
{
    // 游戏循环
    public static string Event_Game_Reset = "Event_Game_Reset";

    // 更新投币
    public static string Event_Update_Coin = "Event_Update_Coin";

    // 确定
    public static string Event_Sure_Or_Missile = "Event_Sure_Or_Missile";
    //public static string Event_Button_Sure = "Event_Button_Sure";

    public static string Event_Accelerate = "Event_Accelerate";

    public static string Event_DownTrumpt = "Event_DownTrumpt";

    public static string Event_DownWiper = "Event_DownWiper";

    public static string Event_Button_A = "Event_Button_A";

    public static string Event_Button_B = "Event_Button_B";

    public static string Event_Turn_Left = "Event_Turn_Left";

    public static string Event_Turn_Right = "Event_Turn_Right";

    public static string Event_Turn_Up = "Event_Turn_Up";

    public static string Event_Turn_Down = "Event_Turn_Down";

    public static string Event_OnTraffic = "Event_OnTraffic";

    public static string Event_Set_KnowLedage = "Event_Set_KnowLedage";

    public static string Event_Tip_Left = "Event_Tip_Left";

    public static string Event_Tip_Right = "Event_Tip_Right";

    public static string Event_Camera_State_FirstOrThird = "Event_Camera_State_FirstOrThird";

    public static string Event_Friend_Leave = "Event_Friend_Leave";

    public static string Event_Boss_Must_Dead = "Event_Boss_Must_Dead";

    //public static string Event_Player_Missile = "Event_Player_Missile";

    public static string Event_Player_Gather = "Event_Player_Gather";

    public static string Event_Gorge_Boss_Animation = "Event_Gorge_Boss_Animation";

    public static string Event_On_TieQian_Effect = "Event_On_TieQian_Effect";

    public static string Event_On_Trigger_JiGuang_Weapon = "Event_On_Trigger_JiGuang_Weapon";

    public static string Event_OnTrigger_Wall = "Event_OnTrigger_Wall";

    public static string Event_On_Describe = "Event_On_Describe";

    public static string Event_Trigger_Ice = "Event_Trigger_Ice";

    public static string Event_Missile_Fired_Or_Trigger = "Event_Missile_Fired_Or_Trigger";

    public static string Event_Tips_Type_By_Int = "Event_Tips_Type_By_Int";

    public static string Event_Boss_Warning = "Event_Boss_Warning";

    public static string Event_Enter_Follow_Water = "Event_Enter_Follow_Water";

    public static string Event_Engine_Speed_Up = "Event_Engine_Speed_Up";

    public static string Event_Can_Attack_Gorge_Boss = "Event_Can_Attack_Gorge_Boss";

    public static string Event_Lock_Pos = "Event_Lock_Pos";

    public static string Event_Groge_Boss_Attack = "Event_Groge_Boss_Attack";

    public static string Event_Idle_Movie = "Event_Idle_Movie";

    public static string Event_PathInfo_Has_Init = "Event_PathInfo_Has_Init";

    public static string Event_Hurry_ShowOrHide = "Event_Hurry_ShowOrHide";

    public static string Event_Skip_Describe = "Event_Skip_Describe";

    public static string Event_Gorge_Boss_QTE = "Event_Gorge_Boss_QTE";

    public static string Event_Hit_Break = "Event_Hit_Break";

    // 更新进度条
    public static string Event_Loading_Progress = "Event_Loading_Progress";
    // 加载完毕
    public static string Event_Loading_End = "Event_Loading_End";
    // 射击
    //public static string Event_Player_Fire = "Event_Player_Fire";

    public static string Event_Select_Map_Move_Left = "Event_Select_Map_Move_Left";

    public static string Event_Select_Map_Move_Right = "Event_Select_Map_Move_Right";

    public static string Event_Lift_Up = "Event_Lift_Up";
    public static string Event_Lift_Down = "Event_Lift_Down";
    public static string Event_Lift_End = "Event_Lift_End";

    public static string Event_Gorge_Boss_Fly = "Event_Gorge_Boss_Fly";

    public static string Event_Gorge_Boss_Follow = "Event_Gorge_Boss_Follow";

    public static string Event_JiKu_To_Game_Light = "Event_JiKu_To_Game_Light";

    #region 场景触发
    // 触发水波特效
    public static string Event_Trigger_Effect_Water = "Event_Trigger_Effect_Water";

    // 触发杨尘特效
    public static string Event_Trigger_Effect_Dust = "Event_Trigger_Effect_Dust";

    #endregion
}

public class ModelName
{
    public static string Player0    = "Player0";
    public static string Player1    = "Player1";
    public static string Player2    = "Player2";

    public static string Boss_City  = "Boss_City";
    public static string Coin       = "Coin";
    public static string Diamond    = "Diamond";
    public static string FeiQi      = "FeiQi";
    public static string Fuel       = "Fuel";
    public static string Magnet     = "Magnet";
    public static string Mine       = "Mine";
    public static string Missle     = "Missle";
    public static string Shield     = "Shield";
    public static string WBullet    = "WBullet";
    public static string WMissle    = "WMissle";
    public static string WStone     = "WStone";
    public static string WFireBall  = "WFireBall";
    public static string WMissle2   = "WMissle2";
}

public static class EffectName
{
    // 更随特效 水
    public const string Effect_Water = "Effect_Water";

    // 杨尘
    public const string Effect_Dust = "Effect_Dust";

    // 碎石 0
    public const string Effect_Rock0 = "Effect_Rock0";

    // 碎石 1
    public const string Effect_Rock1 = "Effect_Rock1";

    // 碎石 2
    public const string Effect_Rock2 = "Effect_Rock2";

    // 落石
    public const string Effect_Stone = "Effect_Stone";

    // 炸弹 0
    public const string Effect_Bomb0 = "Effect_Bomb0";

    // 炸弹 0
    public const string Effect_Bomb1 = "Effect_Bomb1";

    public const string Effect_Boss_City_Dead = "Effect_Boss_City_Dead";

    public const string Effect_WMissle_baoz = "Effect_WMissle_baoz";

    public const string Effect_mine_bomb = "Effect_mine_bomb";

    // 敌机1爆炸
    public const string Effect_diji_Plane_baoz = "Effect_diji_Plane_baoz";

    // 敌机2爆炸
    public const string Effect_Enemy_2_baoz = "Effect_Enemy_2_baoz";

    public const string Effect_WStone_Born = "Effect_WStone_Born";

    public const string Effect_Aerolite_baoz = "Effect_Aerolite_baoz";
}

public static class GameTag
{
    public const string Player = "Player";

    public const string Boss = "Boss";

    public const string Props = "Props";

    public const string Enemy = "Enemy";

    public const string Friend = "Friend";

    public const string Weapon = "Weapon";

    public const string GuangGaoPai = "GuangGaoPai";

    public const string JiGuang = "JiGuang";

    public const string EffectIdle = "EffectIdle";

    public const string EffectHurt = "EffectHurt";

    public const string MissilePoint = "MissilePoint";

    public const string Glow = "Glow";

    public const string Gorge = "Gorge";

    public const string Bug = "Bug";

    public const string Tree = "Tree";

    public const string CameraStop = "CameraStop";

    public const string TuoWei = "TuoWei";
}