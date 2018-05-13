using UnityEngine;
using System.Collections;
using Need.Mx;
using System.Collections.Generic;

public class PanelSelectMediator : PureMVC.Patterns.Mediator
{
    public const string NAME = MyMediatorName.PanelSelectMediator;

    private PanelSelectProxy proxy;

    private PanelSelectLogic ui { get { return ((GameObject)ViewComponent).GetComponent<PanelSelectLogic>(); } }

    public PanelSelectMediator(string mediatorName, object viewComponent) : base(mediatorName, viewComponent) { }

    public override System.Collections.Generic.IList<string> ListNotificationInterests()
    {
        IList<string> list = new List<string>();
        list.Add(PanelSelectProxy.UPDATE_COIN);
        return list;
    }

    public override void HandleNotification(PureMVC.Interfaces.INotification notification)
    {
        switch(notification.Name)
        {
            case PanelSelectProxy.UPDATE_COIN:
                ui.UpdateCoin(proxy.Coin, proxy.Rate);
                break;
        }
    }

    public override void OnRegister()
    {
        Debug.Log("OnRegister: " + MediatorName);
        proxy = Facade.RetrieveProxy(PanelSelectProxy.NAME) as PanelSelectProxy;

        ui.InitMediator(this);

        EventDispatcher.AddEventListener(EventDefine.Event_Update_Coin, OnCoin);
        EventDispatcher.AddEventListener(EventDefine.Event_Turn_Left, OPLeft);
        EventDispatcher.AddEventListener(EventDefine.Event_Turn_Right, OPRight);
        EventDispatcher.AddEventListener(EventDefine.Event_Sure_Or_Missile, Sure);

        proxy.Init(SettingManager.Instance.HasCoin(0), SettingManager.Instance.GameRate);

        ioo.audioManager.PlayBackMusic("Music_Panel_Select_Map");
        ioo.audioManager.PlayPersonMusic("Person_Sound_Choose_Map");

    }

    public override void OnRemove()
    {
        EventDispatcher.RemoveEventListener(EventDefine.Event_Update_Coin, OnCoin);
        EventDispatcher.RemoveEventListener(EventDefine.Event_Turn_Left, OPLeft);
        EventDispatcher.RemoveEventListener(EventDefine.Event_Turn_Right, OPRight);
        EventDispatcher.RemoveEventListener(EventDefine.Event_Sure_Or_Missile, Sure);
    }

    private void OnCoin()
    {
        proxy.AddCoin();
    }

    private void OPLeft()
    {
        ui.OnLeft();
    }

    private void OPRight()
    {
        ui.OnRight();
    }

    private void Sure()
    {
        ui.OnSure();
    }

    #region Public Function
    public void TriggerSure()
    {
        EventDispatcher.TriggerEvent(EventDefine.Event_Sure_Or_Missile);
    }
    #endregion
}
