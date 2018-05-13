/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   PanelLoadingLogic.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/5/12 11:16:25
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Need.Mx;
using System.Collections.Generic;

public class PanelLoadingLogic : BaseUI
{
    public override EnumUIType GetUIType()
    {
        return EnumUIType.PanelLoading;
    }

    public override void OnRegesterMediatorPlug()
    {
        MediatorManager.Instance.RegesterMediatorPlug(GetUIType(), gameObject, MyMediatorName.PanelLoadingMediator);
    }

    protected override void OnRelease()
    {
        MediatorManager.Instance.RemoveMediatorPlug(GetUIType());
    }

    private PanelLoadingView _View;
    private PanelLoadingMediator _Mediator;

    private float _CurProgress;
    private float _ToProgress;

    private bool _HasSkip;

    private float _time = 12;
    private bool _hasEnd = false;
    private bool _loadingIsReady = false;
    protected override void OnStart()
    {
        _View = new PanelLoadingView();

        _View.Init(transform);
    }

    void Update()
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

        if (_time > 0)
            _time -= Time.deltaTime;
        else
            _time = 0;

        if (!_hasEnd && !_HasSkip)
        {
            if (_time == 0 && _loadingIsReady)
            {
                _hasEnd = true;
                _Mediator.LoadingEnd();
            }
        }

        if (_CurProgress >= 110)
            return;

        if (_CurProgress < _ToProgress)
        {
            if (_ToProgress - _CurProgress > 10)
                _CurProgress += (_ToProgress - _CurProgress) * 0.3f;
            else
            {
                ++_CurProgress;
            }
        }
        else
            return;

        float progress = _CurProgress > 100 ? 100 : _CurProgress;
        _View.Progress.text = ((int)progress).ToString() + "%";

        // 加载完毕
        if (_CurProgress >= 110)
        {
            _loadingIsReady = true;
            _View.Skip.SetActive(true);
        }
    }
    #region Public Function
    public void InitMediator(PanelLoadingMediator mediator)
    {
        _Mediator = mediator;
    }

    /// <summary>
    /// 设置进度条
    /// </summary>
    /// <param name="progress"></param>
    public void SetProgress(int toProgress)
    {
        _ToProgress = toProgress;
    }
    
    /// <summary>
    /// 更新币率
    /// </summary>
    /// <param name="coin"></param>
    /// <param name="rate"></param>
    public void UpdateCoin(int coin, int rate)
    {
        _View.Text_Coin.text = coin + "/" + rate;
    }

    /// <summary>
    /// 跳过
    /// </summary>
    public void OnSkip()
    {
        if (_HasSkip || _hasEnd || !_loadingIsReady)
            return;

        _HasSkip = true;
        _Mediator.LoadingEnd();
        _View.Effect_Please.SetActive(true);
    }

    #endregion

}
