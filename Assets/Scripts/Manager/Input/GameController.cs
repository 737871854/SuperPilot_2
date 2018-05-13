using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Need.Mx;

public enum InputType
{
    None,
    Mouse,
    JoyStick
}

public class GameController : MonoBehaviour
{
    private InputType _iType;
    public InputType Type { get { return _iType; } }

    private MouseHandler MouseHandler;

    public void Init(InputType type)
    {
        _iType = type;

        if (_iType == InputType.JoyStick)
            IOManager.Instance.Init(1);

        MouseHandler = new MouseHandler();
    }

    private void Update()
    {
        if (_iType == InputType.JoyStick)
        {
            IOManager.Instance.UpdateIOEvent();
            return;
        }

        // 投币
        if (MouseHandler.Coin())
        {
            ioo.gameMode.Normal();
            SettingManager.Instance.AddCoin();
            SettingManager.Instance.Save();
            EventDispatcher.TriggerEvent(EventDefine.Event_Update_Coin);
        }

        if (MouseHandler.EnterSure())
        {
            if (ioo.gameMode.State == GameState.Coin)
                ioo.gameMode.Normal();

            EventDispatcher.TriggerEvent(EventDefine.Event_Sure_Or_Missile);
        }

        //if (ioo.gameMode.State == GameState.Coin)
        //{
        //    // 开始游戏
        //    if (MouseHandler.EnterSure())
        //    {
        //        ioo.gameMode.Normal();
        //        EventDispatcher.TriggerEvent(EventDefine.Event_Button_Sure);
        //    }
        //}

        // 射击
        if (ioo.gameMode.State == GameState.Play)
        {
            if (MouseHandler.Fire())
            {
                ioo.gameMode.Player.IsPress = true;
                ioo.gameMode.CanFire = true;
                //EventDispatcher.TriggerEvent(EventDefine.Event_Player_Fire);
            }
            else
            {
                ioo.gameMode.Player.IsPress = false;
                ioo.gameMode.CanFire = false;
            }

            if (MouseHandler.Gather())
            {
                EventDispatcher.TriggerEvent(EventDefine.Event_Player_Gather);
            }
        }

        //if (ioo.gameMode.State == GameState.Continue)
        //{
        //    if (MouseHandler.EnterSure())
        //    {
        //        EventDispatcher.TriggerEvent(EventDefine.Event_Button_Sure);
        //    }
        //}

        //if (ioo.gameMode.State == GameState.Select)
        //{
        //    if (MouseHandler.EnterSure())
        //    {
        //        EventDispatcher.TriggerEvent(EventDefine.Event_Button_Sure);
        //    }
        //}

        // 后台A
        if (MouseHandler.Confirm())
        {
            ioo.gameMode.Normal();
            EventDispatcher.TriggerEvent(EventDefine.Event_Button_A);
        }

        // 后台B
        if (MouseHandler.Select())
        {
            ioo.gameMode.Normal();
            EventDispatcher.TriggerEvent(EventDefine.Event_Button_B);
        }

        // 操作：左
        if (MouseHandler.TurnLeft())
        {
            EventDispatcher.TriggerEvent(EventDefine.Event_Turn_Left);
        }

        // 操作：右
        if (MouseHandler.TurnRight())
        {
            EventDispatcher.TriggerEvent(EventDefine.Event_Turn_Right);
        }

        // 操作：上
        if (MouseHandler.PullUp())
        {
            EventDispatcher.TriggerEvent(EventDefine.Event_Turn_Up);
        }

        // 操作：下
        if (MouseHandler.PullDown())
        {
            EventDispatcher.TriggerEvent(EventDefine.Event_Turn_Down);
        }

    }
    private void OnApplicationQuit()
    {
        if (_iType == InputType.JoyStick)
            IOManager.Instance.Close();
    }
    private void OnDestroy()
    {
        if (_iType == InputType.JoyStick)
            IOManager.Instance.Close();
    }
 
}
