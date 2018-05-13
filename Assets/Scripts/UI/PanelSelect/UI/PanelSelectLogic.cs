/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   PanelSelectLogic.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/5/9 17:03:17
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using Need.Mx;
using DG.Tweening;
using DG.Tweening.Core;
using System.Collections;
using UnityEngine.SceneManagement;


public class PanelSelectLogic : BaseUI
{
    public override EnumUIType GetUIType()
    {
        return EnumUIType.PanelSelect;
    }

    public override void OnRegesterMediatorPlug()
    {
        MediatorManager.Instance.RegesterMediatorPlug(GetUIType(), gameObject, MyMediatorName.PanelSelectMediator);
    }

    protected override void OnRelease()
    {
        MediatorManager.Instance.RemoveMediatorPlug(GetUIType());
    }

    enum E_Select
    {
        Map,
        Model,
        End,
    }

    private PanelSelectView _View;
    private PanelSelectMediator _Mediator;

    private float _TotalTime;
    private E_Select _State;
    private string _SelectedName;

    protected override void OnStart()
    {
        _View = new PanelSelectView();

        _View.Init(transform);

        _count      = 0;
        _cool       = true;
        _TotalTime  = 20;
        _State      = E_Select.Map;
        _SelectedName = ModelName.Player0;

        PlaneList = new List<int>();
        for (int i = 0; i < _View.PlaneList.Count; ++i)
        {
            PlaneList.Add(_View.PlaneList.Count - 1 - i);
        }

        ioo.gameMode.MapSelect(E_Map.City);
    }

    private void Update()
    {
        if (IOManager.Instance.NeedOutPutTicket > 0)
        {
            _View.Warning_Ticket.SetActive(true);
            _View.Ticket_Number.text = IOManager.Instance.NeedOutPutTicket.ToString();
        }
        else
        {
            _View.Warning_Ticket.SetActive(false);
        }

        if (_coolTime > 0)
        {
            _coolTime -= Time.deltaTime;
        }
        else
        {
            _cool = true;
            _coolTime = 0;
        }

        if (ioo.gameMode.State != GameState.Select)
            return;

        UpdateClock();

        UpdateSelectPlane();

        UpdateChooseHead();

        if (CurIndex == 0)
        {
            _View.Left0.SetActive(false);
            _View.Left1.SetActive(true);
            _View.Right0.SetActive(true);
            _View.Right1.SetActive(false);
        }else if (CurIndex == 1)
        {
            _View.Left0.SetActive(false);
            _View.Left1.SetActive(true);
            _View.Right0.SetActive(false);
            _View.Right1.SetActive(true);
        }else if (CurIndex == 2)
        {
            _View.Left0.SetActive(true);
            _View.Left1.SetActive(false);
            _View.Right0.SetActive(false);
            _View.Right1.SetActive(true);
        }
    }

    private void UpdateChooseHead()
    {
        for (int i = 0; i < _View.HeadList.Count; ++i )
        {
            int temp = (CurIndex % 3 + 3) % 3;
            if (temp == i)
            {
                _View.HeadList[i].NoSelected.SetActive(false);
                _View.HeadList[i].Selected.SetActive(true);
                _View.HeadList[i].Des.SetActive(true);
            }
            else
            {
                _View.HeadList[i].NoSelected.SetActive(true);
                _View.HeadList[i].Selected.SetActive(false);
                _View.HeadList[i].Des.SetActive(false);
            }
        }
    }

    private bool CanMove;
    private List<float> Point_X = new List<float>() { 20, 0, -20 };
    private float Step = 20;
    private int CurIndex = 1;
    //private int TargetIndex;
    private Vector3 TargetPos;
    private Vector3 OldDir;

    private void UpdateSelectPlane()
    {
        if (!CanMove)
            return;

        Vector3 dir = TargetPos - _View.AirPlane.localPosition;
        if (dir.magnitude > 0.25f && Vector3.Angle(dir, OldDir) < 150)
        {
            dir.Normalize();
            _View.AirPlane.localPosition += dir * Time.deltaTime * 25;
        }
        else
        {
            _View.AirPlane.localPosition = TargetPos;
            CanMove = false;
            //CurIndex = TargetIndex;
            if (isRight)
            {
                isRight = false;
                _View.PlaneList[PlaneList[0]].transform.localPosition += Step * 3 * Vector3.left;

                PlaneList.Add(PlaneList[0]);
                PlaneList.RemoveAt(0);
            }

            if (isLeft)
            {
                isLeft = false;
                _View.PlaneList[PlaneList[2]].transform.localPosition += Step * 3 * Vector3.right;

                PlaneList.Insert(0, PlaneList[2]);
                PlaneList.RemoveAt(PlaneList.Count - 1);
            }
        }

    }

    #region Public Function
    public void InitMediator(PanelSelectMediator mediator)
    {
        _Mediator = mediator;
    }

    /// <summary>
    /// 地图选择等待
    /// </summary>
    private float _coolTime;
    private bool _cool;
    private int _count;

    /// <summary>
    /// 更新币率
    /// </summary>
    /// <param name="coin"></param>
    /// <param name="rate"></param>
    public void UpdateCoin(int coin, int rate)
    {
        _View.Text_Coin.text = coin + "/" + rate;
    }

    private List<int> PlaneList;

    private bool isLeft;
    /// <summary>
    /// 选择左边地图
    /// </summary>
    public void OnLeft()
    {
        if (!_cool)
            return;
        _cool = false;
        _coolTime = 0.5f;
        if (_State == E_Select.Map)
        {
            --_count;
            ioo.gameMode.MapSelect(_count % 2 == 0 ? E_Map.City : E_Map.Gorge);
            EventDispatcher.TriggerEvent(EventDefine.Event_Select_Map_Move_Left);
            MapSelected(ioo.gameMode.Map);
            ioo.audioManager.PlaySound2D("SFX_Sound_Select_Map");
        }
       
        if (_State == E_Select.Model && !CanMove)
        {
            isRight = false;
            isLeft = false;
            if (CurIndex <= 0)
            {
                isLeft = true;
            }
            else if (CurIndex >= 1)
            {
                _View.PlaneList[PlaneList[2]].transform.localPosition += Step * 3 * Vector3.right;

                PlaneList.Insert(0, PlaneList[2]);
                PlaneList.RemoveAt(PlaneList.Count - 1);
            }

            CanMove = true;
            ++CurIndex;
            TargetPos = new Vector3(_View.AirPlane.localPosition.x - 20, _View.AirPlane.localPosition.y, _View.AirPlane.localPosition.z);
            ioo.audioManager.PlaySound2D("SFX_Sound_Select_Plane");
        }
    }

    private bool isRight;
    /// <summary>
    /// 选择右边地图
    /// </summary>
    public void OnRight()
    {
        if (!_cool)
            return;
        _cool = false;
        _coolTime = 0.5f;

        if (_State == E_Select.Map)
        {
            ++_count;
            ioo.gameMode.MapSelect(_count % 2 == 0 ? E_Map.City : E_Map.Gorge);
            EventDispatcher.TriggerEvent(EventDefine.Event_Select_Map_Move_Right);
            MapSelected(ioo.gameMode.Map);
            ioo.audioManager.PlaySound2D("SFX_Sound_Select_Map");
        }

        if (_State == E_Select.Model && !CanMove)
        {
            isRight = false;
            isLeft = false;
           if(CurIndex >= 1)
           {
               isRight = true;
           }else if (CurIndex <= 0)
           {
               _View.PlaneList[PlaneList[0]].transform.localPosition += Step * 3 * Vector3.left;

               PlaneList.Add(PlaneList[0]);
               PlaneList.RemoveAt(0);
           }

            CanMove     = true;
            --CurIndex;
            TargetPos = new Vector3(_View.AirPlane.localPosition.x + 20, _View.AirPlane.localPosition.y, _View.AirPlane.localPosition.z);
            ioo.audioManager.PlaySound2D("SFX_Sound_Select_Plane");
        }
    }

    /// <summary>
    /// 确认地图
    /// </summary>
    public void OnSure()
    {
        if (_State == E_Select.End)
            return;

        ioo.audioManager.PlaySound2D("SFX_Sound_Sure");
     
        switch (_State)
        {
            case E_Select.Map:
                _TimeUp = 30;
                _State = E_Select.Model;
                _View.MapRoot.SetActive(false);
                _View.ModelRoot.SetActive(true);
                _View.Effect_Please.SetActive(true);

                ioo.audioManager.StopPersonMusic("Person_Sound_Choose_Map");
                ioo.audioManager.StopBackMusic("Music_Panel_Select_Map");
                ioo.audioManager.PlayBackMusic("Music_Panel_Select_Plane");
                ioo.audioManager.PlayPersonMusic("Person_Sound_Choose_Plane");

                break;
            case E_Select.Model:
                _TimeUp = 30;
                _State = E_Select.End;
                _View.Effect_Please.SetActive(false);
                _View.Effect_Please.SetActive(true);

                ioo.audioManager.StopBackMusic("Music_Panel_Select_Plane");
                ioo.audioManager.StopPersonMusic("Person_Sound_Choose_Plane");
                ioo.audioManager.PlayPersonSound("Person_Sound_Plane_Is_Good_Condition");

                CurIndex = (CurIndex % 3 + 3) % 3;
                switch(CurIndex)
                {
                    case 0:
                        _SelectedName = ModelName.Player0;
                        break;
                    case 1:
                        _SelectedName = ModelName.Player1;
                        break;
                    case 2:
                        _SelectedName = ModelName.Player2;
                        break;
                }

                StartCoroutine(ToLoading());
                break;
        }
    }
    #endregion

    #region Private Function
    IEnumerator ToLoading()
    {
        yield return new WaitForSeconds(3.5f);

        // 切换到下个场景
        UIManager.Instance.CloseUI(EnumUIType.PanelSelect);
        ioo.gameMode.PlayerSelected(_SelectedName);
        ioo.poolManager.NeedPreLoadInLoading = true;
        ioo.scenesManager.LoadScene(ioo.gameMode.Map.ToString(), EnumUIType.PanelMain);
        ioo.gameMode.RunMode(GameState.Loading);
    }

    /// <summary>
    /// 选择对应地图
    /// </summary>
    /// <param name="map"></param>
    private void MapSelected(E_Map map)
    {
        for (E_Map m = E_Map.City; m <= E_Map.Gorge; ++m )
        {
          
        }
    }

    private float _TimeUp = 30;
    private void UpdateClock()
    {
        if (_State == E_Select.End)
            return;

        _View.Time.text = ((int)_TimeUp).ToString();
        if (_TimeUp > 0)
        {
            _TimeUp -= Time.deltaTime;
        }
        else
        {
            _TimeUp = 0;
            _Mediator.TriggerSure();
        }
    }

    #endregion
}
