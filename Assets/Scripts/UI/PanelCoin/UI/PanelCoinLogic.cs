/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   CoinLogic.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/5/9 10:58:03
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using Need.Mx;

public class PanelCoinLogic : BaseUI
{
    public override EnumUIType GetUIType()
    {
        return EnumUIType.PanelCoin;
    }

    public override void OnRegesterMediatorPlug()
    {
        MediatorManager.Instance.RegesterMediatorPlug(GetUIType(), gameObject, MyMediatorName.PanelCoinMediator);
    }

    private PanelCoinView _View;

    private PanelCoinMediator _Mediator;

    private float _time;

    private AudioSource _audioSource;

    private MovieTexture _movieTexture;

    protected override void OnStart()
    {
        _View = new PanelCoinView();

        _View.Init(transform);

        _time = Random.Range(20, 30);

        //PlayBackGroundMusic();
    }

    protected override void OnRelease()
    {
        MediatorManager.Instance.RemoveMediatorPlug(GetUIType());
    }

    private void PlayBackGroundMusic()
    {
        ioo.audioManager.PlayBackMusic("standby_sound");
    }

    private void Update()
    {
        if (_time > 0)
        {
            _time -= Time.deltaTime;
        }
        else
        {
            _time = Random.Range(20, 30);
            ioo.audioManager.PlayPersonSound("Person_Sound_Idle_Coin");
        }

        if (IOManager.Instance.NeedOutPutTicket > 0)
        {
            _View.Warning_Ticket.SetActive(true);
            _View.Ticket_Number.text = IOManager.Instance.NeedOutPutTicket.ToString();
        }
        else
        {
            _View.Warning_Ticket.SetActive(false);
        }

        if (_movieTexture != null && !_movieTexture.isPlaying)
        {
            _movieTexture.Stop();
            _audioSource.Stop();

            ioo.gameMode.Normal();
        }
    }

    #region Public Function
    public void InitMediator(PanelCoinMediator mediator)
    {
        _Mediator = mediator;
    }

    /// <summary>
    /// 更新币数
    /// </summary>
    /// <param name="value"></param>
    public void UpdateView(int coin, int rate)
    {
        _View.Text_Coin.text = coin + "/" + rate;

        if (_Mediator.EnterGame)
            return;

        if (coin >= rate)
        {
            _View.Please0.SetActive(false);
            _View.Please1.SetActive(true);
        }
        else
        {
            _View.Please0.SetActive(true);
            _View.Please1.SetActive(false);
        }
    }

    public void ShowEffectSure()
    {
        _View.Effect_Please1.SetActive(true);
    }

    public void PlayIdleMovie(MovieTexture mt)
    {
        _movieTexture = mt;

        _View.Movie.gameObject.SetActive(true);
        _View.Movie.texture = _movieTexture;
        if (_audioSource == null)
            _audioSource = _View.Movie.GetComponent<AudioSource>();
        _audioSource.clip = _movieTexture.audioClip;
        _movieTexture.loop = false;
        _movieTexture.Play();
        _audioSource.Play();
    }

    public void StopIdleMovie()
    {
        _audioSource.Stop();
        _movieTexture.Stop();
        _movieTexture = null;
        _View.Movie.gameObject.SetActive(false);
    }
    #endregion
}
