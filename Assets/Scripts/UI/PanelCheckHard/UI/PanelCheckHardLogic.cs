/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   PanelCheckHardLogic.cs
 * 
 * 简    介:    硬件检测
 * 
 * 创建标识：   Pancake 2017/5/12 18:03:26
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using Need.Mx;
using System.Collections;
using UnityEngine.SceneManagement;

public class PanelCheckHardLogic : BaseUI
{
    public override EnumUIType GetUIType()
    {
        return EnumUIType.PanelCheckHard;
    }

    public override void OnRegesterMediatorPlug()
    {
        MediatorManager.Instance.RegesterMediatorPlug(GetUIType(), gameObject, MyMediatorName.PanelCheckHardMediator);
    }

    protected override void OnRelease()
    {
        MediatorManager.Instance.RemoveMediatorPlug(GetUIType());
    }

    private PanelCheckHardView _View;

    protected override void OnStart()
    {
        _View = new PanelCheckHardView();

        _View.Init(transform);

        InitText();
    }

    private void InitText()
    {
        isOk = new bool[_View.Check.Count];
        if (SettingManager.Instance.GameLanguage == 0)
        {
            for (int i = 0; i < _View.Check.Count; ++i )
                _View.Check[i].labelCh.SetActive(true);
        }
        else
        {
            for (int i = 0; i < _View.Check.Count; ++i)
                _View.Check[i].labelEn.SetActive(true);
        }
    }

    private bool[] isOk;
    private bool pass;
    void Update()
    {
        if (pass)
            return;

        if (ioo.gameController.Type == InputType.JoyStick)
        {
            if (IOManager.Instance.IoEvent != null)
            {
                isOk[0] = IOManager.Instance.IoEvent.IsStart;
                isOk[1] = IOManager.Instance.IoEvent.IsCoin;
                isOk[2] = IOManager.Instance.IoEvent.IsMissile;
                isOk[3] = IOManager.Instance.IoEvent.IsTurnLeft;
                isOk[4] = IOManager.Instance.IoEvent.IsTurnRight;
                isOk[5] = IOManager.Instance.IoEvent.IsPullUp;
                isOk[6] = IOManager.Instance.IoEvent.IsPullDown;
                isOk[7] = IOManager.Instance.IoEvent.IsGather;
                isOk[8] = IOManager.Instance.IoEvent.IsConfirm;
                isOk[9] = IOManager.Instance.IoEvent.IsSelect;
                //isOk[10] = IOManager.Instance.IoEvent.IsResetEye;
                //isOk[11] = IOManager.Instance.IoEvent.IsUpEye;
                //isOk[12] = IOManager.Instance.IoEvent.IsDownEye;
                isOk[10] = true;
                isOk[11] = true;
                isOk[12] = true;

                isOk[13] = IOManager.Instance.IoEvent.IsTicket;
            }

            bool allok = true;
            for (int i = 0; i < isOk.Length; ++i)
            {
                if (!isOk[i])
                {
                    allok = false;
                    _View.Check[i].markRight.SetActive(false);
                    _View.Check[i].markWrong.SetActive(true);
                }
                else
                {
                    _View.Check[i].markRight.SetActive(true);
                    _View.Check[i].markWrong.SetActive(false);
                }
            }

            if (!allok)
                return;
        }

        pass = true;

        StartCoroutine(Pass());
    }

    IEnumerator Pass()
    {
        for (int i = 0; i < isOk.Length; ++i)
        {
           _View.Check[i].markRight.SetActive(true);
        }

        IOManager.Instance.TellIOBoardEnterGame();

        yield return new WaitForSeconds(2);

        ioo.scenesManager.LoadSceneDir(SceneName.Coin);

        UIManager.Instance.CloseUI(EnumUIType.PanelCheckHard);
        UIManager.Instance.OpenUI(EnumUIType.PanelCoin);
        ioo.gameMode.RunMode(GameState.Coin);

    }
       
}
