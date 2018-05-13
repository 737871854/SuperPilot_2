using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Need.Mx;

public class IOManager
{
    private static readonly object _obj = new object();
    private static IOManager _instance;
    public static IOManager Instance
    {
        get 
        {
            if (null == _instance)
            {
                lock(_obj)
                {
                    if (null == _instance)
                    {
                        _instance = new IOManager();
                    }
                }
            }
            return _instance;
        }
    }

    private int iocount=1;
    private SerialIOHost serialiohost;

    public SerialIOHost serialioHost
    {
        get { return serialiohost;}
    }
    private IOEvent ie = new IOEvent();
    private IOEvent[] ioevent;

    public IOEvent IoEvent { get { return ioevent[0]; } }

    public void ResetEvent_0(int i)
    {
        ioevent[i].Reset_0();
    }

    public void ResetEvent_1(int i)
    {
        ioevent[i].Reset_1();
    }

    #region MyRegion
   
    public int NeedOutPutTicket = 0;
   
    #endregion
   
    public void Close()
    {
        if(serialiohost != null)
        {
            serialiohost.Close();
        }
    }

    //初始化操作，打开端口，初始化ioEvent数组
    public void Init(int portCount)
    {
        iocount = portCount;
        ioevent = new IOEvent[portCount];
        for (byte i = 0; i < iocount; i++)
        {
            ioevent[i] = new IOEvent();
        }
        serialiohost= new SerialIOHost();
        serialiohost.Init(7, 7, 10);
    }

    /// <summary>
    ///  进入后台通知IO Board
    /// </summary>
    public void SendMessageEnterSetting()
    {
        if (ioo.gameController.Type == InputType.Mouse)
            return;

        byte[] data = new byte[] { 0x60, 0x00, 0x00, 0x00, 0x00, 0x60 };

        serialiohost.WriteAndSendMessage(0, data);

        //Debug.Log("发送进入后台信号给IO Board");
    }

    /// <summary>
    /// 退出后台
    /// </summary>
    public void SendMessageExitSetting()
    {
        if (ioo.gameController.Type == InputType.Mouse)
            return;

        byte[] data = new byte[] { 0x40, 0x00, 0x00, 0x00, 0x00, 0x40 };

        serialiohost.WriteAndSendMessage(0, data);

        //Debug.Log("发送退出后台信号给IO Board");
    }

    /// <summary>
    /// 游戏开始信号
    /// </summary>
    public void SendMessageGameBegine()
    {
        if (ioo.gameController.Type == InputType.Mouse)
            return;

        byte[] data = new byte[] { 0x80, 0x00, 0x00, 0x00, 0x00, 0x80 };

        serialiohost.WriteAndSendMessage(0, data);
        //serialiohost.Send();
        //CoroutineController.Instance.StartCoroutine(SendGameStartMessageToIOBoard(data));
    }

    /// <summary>
    /// 游戏结束信号
    /// </summary>
    public void SendMessageGameEnd()
    {
        if (ioo.gameController.Type == InputType.Mouse)
            return;

        byte[] data = new byte[] { 0x40, 0x00, 0x00, 0x00, 0x00, 0x40 };

        serialiohost.WriteAndSendMessage(0, data);
    }

    IEnumerator SendGameStartMessageToIOBoard(byte[] data)
    {
        int count = 5;
        while (--count > 0)
        {
            //Debug.Log("发送游戏开始信号给IO Board");
            serialiohost.WriteAndSendMessage(0, data);
            yield return new WaitForSeconds(0.3f);
        }
    }

    /// <summary>
    /// 出票
    /// </summary>
    public void SeneMessageTicket(int value)
    {
        if (ioo.gameController.Type == InputType.Mouse)
            return;

        byte t0 = 0x00;
        byte t1 = 0x00;
        byte t2 = 0x00;
        byte[] b = System.BitConverter.GetBytes(value);
        if (b.Length == 1)
            t1 = b[0];
        else
        {
            t0 = b[0];
            t1 = b[1];
        }

        t2 |= 0x40;
        t2 |= t0;
        t2 |= t1;

        byte[] data = new byte[] { 0x40, 0x00, t1, t0, 0x00, t2 };

        serialiohost.WriteAndSendMessage(0, data);
        //serialiohost.Send();
        //CoroutineController.Instance.StartCoroutine(SendTicketMessageToIOBoard(data));
    }

    IEnumerator SendTicketMessageToIOBoard(byte[] data)
    {
        int count = 5;
        while (--count > 0)
        {
            //Debug.Log("发送出票信息给IO Board");
            serialiohost.WriteAndSendMessage(0, data);
            yield return new WaitForSeconds(0.3f);
        }
    }

    /// <summary>
    /// 告诉IO板子进入游戏
    /// </summary>
    public void TellIOBoardEnterGame()
    {
        if (ioo.gameController.Type == InputType.Mouse)
            return;

        serialiohost.WriteAndSendMessage(0, new byte[] { 0x00, 0x44, 0x00, 0x00, 0x00, 0x00 });
        //serialiohost.Send();
        //CoroutineController.Instance.StartCoroutine(SendMessageToPort());
    }

    IEnumerator SendMessageToPort()
    {
        int count = 5;
        while (--count > 0)
        {
            serialiohost.WriteAndSendMessage(0, new byte[] { 0x00, 0x44, 0x00, 0x00, 0x00, 0x00 });
            yield return new WaitForSeconds(0.3f);
        }
    }

    /// <summary>
    /// 每帧分别读取一次协议队列中的协议，并存入协议信息类对象数组ioEvent
    /// </summary>
    public void UpdateIOEvent()
    {
        if (serialiohost != null && serialiohost.HadDevice())
        {
            serialiohost.Update();

            // 操作：左
            if (ioevent[0].IsTurnLeft)
            {
                EventDispatcher.TriggerEvent(EventDefine.Event_Turn_Left);
            }

            // 操作：右
            if (ioevent[0].IsTurnRight)
            {
                EventDispatcher.TriggerEvent(EventDefine.Event_Turn_Right);
            }

            // 操作：上
            if (ioevent[0].IsPullUp)
            {
                EventDispatcher.TriggerEvent(EventDefine.Event_Turn_Up);
            }

            // 操作：下
            if (ioevent[0].IsPullDown)
            {
                EventDispatcher.TriggerEvent(EventDefine.Event_Turn_Down);
            }

            if (ioo.gameMode.State == GameState.Select)
            {
                IOManager.Instance.ResetEvent_1(0);
            }
        }
    }

  
    public void ReadDatas()
    {
        byte h = serialiohost.Read(-1);

        byte k = serialiohost.Read(0);
        byte q = serialiohost.Read(1);

        ioevent[0].IsGather     = (IOParser.GetBit(0, k) == 1);
        ioevent[0].IsPullDown   = (IOParser.GetBit(1, k) == 1);
        ioevent[0].IsPullUp     = (IOParser.GetBit(2, k) == 1);
        ioevent[0].IsTurnRight  = (IOParser.GetBit(3, k) == 1);
        ioevent[0].IsTurnLeft   = (IOParser.GetBit(4, k) == 1);
        ioevent[0].IsMissile    = (IOParser.GetBit(5, k) == 1);
        ioevent[0].IsStart      = (IOParser.GetBit(6, k) == 1);
        ioevent[0].IsCoin       = (IOParser.GetBit(7, k) == 1);

        ioevent[0].IsConfirm    = (IOParser.GetBit(7, q) == 1);
        ioevent[0].IsSelect     = (IOParser.GetBit(6, q) == 1);
        ioevent[0].IsResetEye   = (IOParser.GetBit(5, q) == 1);
        ioevent[0].IsUpEye      = (IOParser.GetBit(4, q) == 1);
        ioevent[0].IsDownEye    = (IOParser.GetBit(3, q) == 1);
        ioevent[0].IsTicket     = (IOParser.GetBit(2, q) == 1);

        IOManager.Instance.NeedOutPutTicket = System.BitConverter.ToInt32(new byte[] { serialiohost.Read(4), serialiohost.Read(3), 0, 0 }, 0);

        serialiohost.ResetRead();

        if (h == 0xAA)
        {
            // 投币
            if (ioevent[0].IsCoin)
            {
                ioo.gameMode.Normal();
                SettingManager.Instance.AddCoin();
                SettingManager.Instance.Save();
                EventDispatcher.TriggerEvent(EventDefine.Event_Update_Coin);
            }

            if (ioevent[0].IsMissile)
            {
                if (ioo.gameMode.State == GameState.Coin)
                    ioo.gameMode.Normal();
                EventDispatcher.TriggerEvent(EventDefine.Event_Sure_Or_Missile);
            }

            //if (ioo.gameMode.State == GameState.Coin)
            //{
            //    // 开始游戏
            //    if (ioevent[0].IsStart)
            //    {
            //        ioo.gameMode.Normal();
            //        EventDispatcher.TriggerEvent(EventDefine.Event_Button_Sure);
            //    }
            //}

            // 射击
            if (ioo.gameMode.State == GameState.Play)
            {
                if (ioevent[0].IsStart)
                {
                    //EventDispatcher.TriggerEvent(EventDefine.Event_Player_Fire);
                    ioo.gameMode.Player.IsPress = true;
                    ioo.gameMode.CanFire = true;
                }
                else
                {
                    ioo.gameMode.Player.IsPress = false;
                    ioo.gameMode.CanFire = false;
                }

                if (ioevent[0].IsGather)
                {
                    EventDispatcher.TriggerEvent(EventDefine.Event_Player_Gather);
                }
            }

            //if (ioo.gameMode.State == GameState.Continue)
            //{
            //    if (ioevent[0].IsStart)
            //    {
            //        EventDispatcher.TriggerEvent(EventDefine.Event_Button_Sure);
            //    }
            //}

            //if (ioo.gameMode.State == GameState.Select)
            //{
            //    // 确认地图
            //    if (ioevent[0].IsStart)
            //    {
            //        EventDispatcher.TriggerEvent(EventDefine.Event_Button_Sure);
            //    }
            //}

            // 后台A
            if (ioevent[0].IsConfirm)
            {
                ioo.gameMode.Normal();
                EventDispatcher.TriggerEvent(EventDefine.Event_Button_A);
            }

            // 后台B
            if (ioevent[0].IsSelect)
            {
                ioo.gameMode.Normal();
                EventDispatcher.TriggerEvent(EventDefine.Event_Button_B);
            }
            
            IOManager.Instance.ResetEvent_0(0);
        }
    }

}
