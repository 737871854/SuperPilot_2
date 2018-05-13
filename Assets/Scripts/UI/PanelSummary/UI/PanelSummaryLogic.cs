/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   PanelSummaryLogic.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/5/11 8:46:21
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

public class PanelSummaryLogic : BaseUI
{
    public override EnumUIType GetUIType()
    {
        return EnumUIType.PanelSummary;
    }

    public override void OnRegesterMediatorPlug()
    {
        MediatorManager.Instance.RegesterMediatorPlug(GetUIType(), gameObject, MyMediatorName.PanelSummaryMediator);
    }

    protected override void OnRelease()
    {
        MediatorManager.Instance.RemoveMediatorPlug(GetUIType());
    }

    private PanelSummaryView _View;
    private PanelSummaryMediator _Mediator;

    protected override void OnStart()
    {
        _View = new PanelSummaryView();

        _View.Init(transform);

        StartCoroutine(Summary());
    }

    public void InitMediator(PanelSummaryMediator mediator)
    {
        _Mediator = mediator;
    }

    private int _resultScore;
    private int _score;
    private int _bossScore;
    private int _damageScore;
    IEnumerator Summary()
    {
        if (ioo.gameMode.Player.Data.ResultScore >= 5000)
        {
            _View.HonourList[2].SetActive(true);
            ioo.audioManager.PlayPersonSound("Person_Sound_Honour2");
        }
        else if (ioo.gameMode.Player.Data.ResultScore < 5000 && ioo.gameMode.Player.Data.ResultScore > 3000)
        {
            _View.HonourList[1].SetActive(true);
            ioo.audioManager.PlayPersonSound("Person_Sound_Honour1");
        }
        else
        {
            _View.HonourList[0].SetActive(true);
            ioo.audioManager.PlayPersonSound("Person_Sound_Honour0");
        }
                
        yield return new WaitForSeconds(5);

        _View.View0.SetActive(false);
        _View.View1.SetActive(true);
        ioo.audioManager.PlayBackMusic("Music_Summary_Score");        

        while(true)
        {
            // 道具积分
            if (_score != ioo.gameMode.Player.Data.Score)
            {
                int d_value = ioo.gameMode.Player.Data.Score - _score;
                int length  = d_value.ToString().Length;
                int delta   = 0;
                if (length > 2)
                    delta = Random.Range(20, 40) * (length - 1);
                else
                    delta = Random.Range(1, 10);
                _score += delta;
                if (_score > ioo.gameMode.Player.Data.Score)
                    _score = ioo.gameMode.Player.Data.Score;
                _View.Score.text = _score.ToString();
                yield return new WaitForEndOfFrame();
            }
            else
            {
                // Boss积分
                if (_bossScore != ioo.gameMode.Player.Data.BossScore)
                {
                    int d_value = ioo.gameMode.Player.Data.BossScore - _bossScore;
                    int length  = d_value.ToString().Length;
                    int delta = 0;
                    if (length > 2)
                        delta = Random.Range(20, 40) * (length - 1);
                    else
                        delta = Random.Range(1, 10);
                    _bossScore += delta;
                    if (_bossScore > ioo.gameMode.Player.Data.BossScore)
                        _bossScore = ioo.gameMode.Player.Data.BossScore;
                    _View.BossScore.text = "+" + _bossScore.ToString();
                    yield return new WaitForEndOfFrame();
                }
                else
                {
                    // 机身受损
                    if (_damageScore != ioo.gameMode.Player.Data.DamageScore)
                    {
                        int d_value = ioo.gameMode.Player.Data.DamageScore - _damageScore;
                        int length  = d_value.ToString().Length;
                        int delta   = 0;
                        if (length > 2)
                            delta = Random.Range(1, 10) * (length - 1);
                        else
                            delta       = Random.Range(1, 10);
                        _damageScore    += delta;
                        if (_damageScore > ioo.gameMode.Player.Data.DamageScore)
                            _damageScore = ioo.gameMode.Player.Data.DamageScore;
                        _View.DamageScore.text = "-" + _damageScore.ToString();
                        yield return new WaitForEndOfFrame();
                    }else
                    {
                        // 总积分
                        if (_resultScore != ioo.gameMode.Player.Data.ResultScore)
                        {
                            int d_value = ioo.gameMode.Player.Data.ResultScore - _resultScore;
                            int length  = d_value.ToString().Length;
                            int delta   = 0;
                            if (length > 2)
                                delta = Random.Range(40, 80) * (length - 1);
                            else
                                delta       = Random.Range(1, 10);
                            _resultScore    += delta;
                            if (_resultScore > ioo.gameMode.Player.Data.ResultScore)
                                _resultScore = ioo.gameMode.Player.Data.ResultScore;
                            _View.ResultScore.text = _resultScore.ToString();
                            yield return new WaitForEndOfFrame();
                        }
                        else
                            break;
                    }
                }
            }
        }

        ioo.audioManager.StopBackMusic("Music_Summary_Score");

        float grate = ioo.gameMode.Player.Data.Score / (ioo.gameMode.Player.Data.TotalTime * ioo.gameMode.Times);

        if (grate >= 10)
        {
            _View.ListSABC[0].SetActive(true);
        }
        else if (grate < 10 && grate >= 8)
        {
            _View.ListSABC[1].SetActive(true);
        }
        else if (grate < 8 && grate >= 6)
        {
            _View.ListSABC[2].SetActive(true);
        }
        else
        {
            _View.ListSABC[3].SetActive(true);
        }
       
        yield return new WaitForSeconds(2);

        int ticket = ioo.gameMode.Player.Data.ResultScore / SettingManager.Instance.GameTicket;

        if (SettingManager.Instance.GameTicket != 0)
        {
            if (ticket != 0)
            {
                ioo.audioManager.PlayBackMusic("Music_Ticket");
                _View.View2.SetActive(true);
                _View.Ticket_Count.text = (ioo.gameMode.Player.Data.ResultScore / SettingManager.Instance.GameTicket).ToString();
                yield return new WaitForSeconds(6);
            }
            else
            {
                _View.Defeated.SetActive(true);
                yield return new WaitForSeconds(3);
            }
        }

        ioo.audioManager.StopBackMusic("Music_Ticket");
        _Mediator.ToStart();
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
    }
}
