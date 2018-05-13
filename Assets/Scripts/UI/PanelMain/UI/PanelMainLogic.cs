/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   PanelMainLogic.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/5/10 9:53:20
 * 
 * 修改描述：
 * 
 */

using UnityEngine;
using System.Collections.Generic;
using Need.Mx;
using System.Collections;
using System.Reflection;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine.UI;

public class PanelMainLogic : BaseUI
{
    public override EnumUIType GetUIType()
    {
        return EnumUIType.PanelMain;
    }

    public override void OnRegesterMediatorPlug()
    {
        MediatorManager.Instance.RegesterMediatorPlug(GetUIType(), gameObject, MyMediatorName.PanelMainMediator);
    }

    protected override void OnRelease()
    {
        MediatorManager.Instance.RemoveMediatorPlug(GetUIType());
    }

    // UI
    private PanelMainView _View;
    private PanelMainMediator _Mediator;
   
    // 小地图路径
    private List<Transform> PointsList;
    // 速度
    private float speed;
    private Transform targetTran;
    private int targetIndex;
    private Vector3 direction;

    protected override void OnStart()
    {
        _View = new PanelMainView();

        _View.Init(transform);

        _View.Lock.GetComponent<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;

        _View.Continue.GetComponent<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;

        ContinueTime = 15;
    }

    private float blurAmount;
    private float ContinueTime;
    private void Update()
    {
        if (ioo.gameMode.State != GameState.Summary)
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
        else
        {
            _View.Warning_Ticket.SetActive(false);
        }

        UpdateTime();

        UpdateScore();

        UpdateContinue();

        UpdateLock();

        UpdateQTE();

        _View.Progress.fillAmount = ioo.gameMode.Player.Data.Gather;

        if (_View.Progress.fillAmount < 0.2)
        {
            _View.Progress.sprite = _View.Blue.sprite;
        }
        else
        {
            _View.Progress.sprite = _View.Yellow.sprite;           
        }

        if (_View.Progress.fillAmount == 1)
        {
            _View.Gather_Effect.SetActive(true);
        }
        else
        {
            _View.Gather_Effect.SetActive(false);
        }

        if (ioo.gameMode.Player.BossStep)
            _View.Gather.SetActive(false);
        else if (!_View.HurryPush.activeSelf)
            _View.Gather.SetActive(true);

        if(ioo.gameMode.State >= GameState.Back)
        {
            _View.Gather.SetActive(false);
            _View.MissileObj.SetActive(false);
            _View.Boss_Health_bar.SetActive(false);
            _View.BossWarning.SetActive(false);
        }

        if (_View.HurryPush.activeSelf)
        {
            _View.Hurry1.fillAmount = ioo.gameMode.Player.Data.Hurry * 0.05f;
            _View.Hurry0.fillAmount = _View.Hurry1.fillAmount + 0.05f;
        }

        float remain = _View.BulletList.Count - ioo.gameMode.Player.Data.Hot;
        for (int i = 0; i < _View.BulletList.Count; ++i)
        {
            if (i < remain)
            {
                _View.BulletList[i].SetActive(true);
            }
            else
            {
                _View.BulletList[i].SetActive(false);
            }
        }
    }

    private Vector3 MoveDir = Vector3.zero;
    private bool IsLock = false;
    private void UpdateLock()
    {
        if (!IsLock)
            return;

        Canvas canvas = _View.Mask.canvas;
        Vector2 pos0 = Camera.main.WorldToScreenPoint(showpos);
        Vector3 pos1;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.transform as RectTransform, pos0, canvas.worldCamera, out pos1))
        {
            _View.LockRectTran.position = pos1;
        }

        if(ioo.gameMode.Boss == null || ioo.gameMode.Boss.IsDead)
        {
            _View.LockRectTran.gameObject.SetActive(false);
        }

        Ray ray = new Ray(ioo.gameMode.Player.BossCameraObj.transform.position, (showpos - ioo.gameMode.Player.BossCameraObj.transform.position));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (!hit.transform.gameObject.tag.Equals(GameTag.Weapon))
            {
                if (hit.transform.gameObject != null)
                {
                    _View.Lock.SetActive(true);
                    _View.UnLock.SetActive(false);
                }
            }
        }
        else
        {
            _View.Lock.SetActive(false);
            _View.UnLock.SetActive(true);
        }

    }

    private void UpdateQTE()
    {
        if (QTEList == null)
        {
            _View.Hit.SetActive(false);
            return;
        }else
        {
            _View.Hit.SetActive(true);
        }

        Canvas canvas = _View.Mask.canvas;

        for (int i = 0; i < QTEList.Count; ++i )
        {
            _View.HitList[i].SetActive(true);
            Vector2 pos0 = Camera.main.WorldToScreenPoint(QTEList[i].transform.position);
            Vector3 pos1;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.transform as RectTransform, pos0, canvas.worldCamera, out pos1))
            {
                _View.HitList[i].transform.position = pos1;
            }
        }

        for (int i = QTEList.Count; i < _View.HitList.Count; ++i)
        {
            _View.HitList[i].SetActive(false);
        }
    }

    /// <summary>
    /// 续币
    /// </summary>
    private void UpdateContinue()
    {
        if (ioo.gameMode.State == GameState.Continue)
        {
            _View.Continue.SetActive(true);
            _View.Text_Cointinue_TimeUp.text = ((int)ContinueTime).ToString();
            _View.Text_Cointinue_Number.text = _Mediator.CointinueNumber();
            if (_Mediator.CoinIsEnough())
            {
                _View.Continue0.SetActive(false);
                _View.Continue1.SetActive(true);
            }
            else
            {
                _View.Continue0.SetActive(true);
                _View.Continue1.SetActive(false);
            }
            
            if (ContinueTime > 0)
            {
                ContinueTime -= ioo.nonStopTime.deltaTime;
            }
            else
            {
                ContinueTime = 15;
                _Mediator.ToBack();
                _View.Continue.SetActive(false);
            }
        }
        else 
        {
            ContinueTime = 15;
            _View.Continue.SetActive(false);
        }
    }

    /// <summary>
    /// 积分
    /// </summary>
    private void UpdateScore()
    {
        int value = ioo.gameMode.Player.Data.Score;

        int value0 = (int)(value / 10000);
        int value1 = (int)((value - 10000 * value0) / 1000);
        int value2 = (int)((value - 10000 * value0 - 1000 * value1) / 100);
        int value3 = (int)((value - 10000 * value0 - 1000 * value1 - 100 * value2) / 10);
        int value4 = (int)(value - 10000 * value0 - 1000 * value1 - 100 * value2 - 10 * value3); 

        if (value>= 10000)
        {
            _View.Score.Text_Number0.color = Color.white;
            _View.Score.Text_Number1.color = Color.white;
            _View.Score.Text_Number2.color = Color.white;
            _View.Score.Text_Number3.color = Color.white;
            _View.Score.Text_Number4.color = Color.white;
        }
        else if (value < 10000 && value >= 1000)
        {
            _View.Score.Text_Number0.color = Color.grey;
            _View.Score.Text_Number1.color = Color.white;
            _View.Score.Text_Number2.color = Color.white;
            _View.Score.Text_Number3.color = Color.white;
            _View.Score.Text_Number4.color = Color.white;
        }
        else if (value < 1000 && value >= 100)
        {
            _View.Score.Text_Number0.color = Color.grey;
            _View.Score.Text_Number1.color = Color.grey;
            _View.Score.Text_Number2.color = Color.white;
            _View.Score.Text_Number3.color = Color.white;
            _View.Score.Text_Number4.color = Color.white;
        }
        else if (value < 100 && value >= 10)
        {
            _View.Score.Text_Number0.color = Color.grey;
            _View.Score.Text_Number1.color = Color.grey;
            _View.Score.Text_Number2.color = Color.grey;
            _View.Score.Text_Number3.color = Color.white;
            _View.Score.Text_Number4.color = Color.white;
        }else if (value < 10 && value > 0)
        {
            _View.Score.Text_Number0.color = Color.grey;
            _View.Score.Text_Number1.color = Color.grey;
            _View.Score.Text_Number2.color = Color.grey;
            _View.Score.Text_Number3.color = Color.grey;
            _View.Score.Text_Number4.color = Color.white;
        }
        else
        {
            _View.Score.Text_Number0.color = Color.grey;
            _View.Score.Text_Number1.color = Color.grey;
            _View.Score.Text_Number2.color = Color.grey;
            _View.Score.Text_Number3.color = Color.grey;
            _View.Score.Text_Number4.color = Color.grey;
        }

        _View.Score.Text_Number0.text = value0.ToString();
        _View.Score.Text_Number1.text = value1.ToString();
        _View.Score.Text_Number2.text = value2.ToString();
        _View.Score.Text_Number3.text = value3.ToString();
        _View.Score.Text_Number4.text = value4.ToString();
    }

    private float _time;
    /// <summary>
    /// 时间
    /// </summary>
    private void UpdateTime()
    {
        _time = ioo.gameMode.Player.Data.LifeTime;

        int value = (int)_time;
        int value0 = (int)((_time / 100) % 10);
        int value1 = (int)(((_time - 100 * value0) / 10) % 10);
        int value2 = (int)((_time - 100 * value0 - 10 * value1) % 10);

        _View.Time.Text_Number0.text = value0.ToString();
        _View.Time.Text_Number1.text = value1.ToString();
        _View.Time.Text_Number2.text = value2.ToString();
        
        if (_time >= 100)
        {
            _View.Time.Text_Number0.color = Color.white;
            _View.Time.Text_Number1.color = Color.white;
            _View.Time.Text_Number2.color = Color.white;
        }
        
        if (_time < 100 && _time >= 10)
        {
            _View.Time.Text_Number0.color = Color.grey;
            _View.Time.Text_Number1.color = Color.white;
            _View.Time.Text_Number2.color = Color.white;
        }else if (_time < 10)
        {
            _View.Time.Text_Number0.color = Color.grey;
            _View.Time.Text_Number1.color = Color.grey;
            _View.Time.Text_Number2.color = Color.white;
        }

        float millisecond = _time - value;
        value  = (int)(10 * millisecond);
        value0 = (int)(value / 10);
        value1 = (int)(value - 10 * value0);
        _View.Time.Text_Number3.text = value0.ToString();
        _View.Time.Text_Number4.text = value1.ToString();

        // 用事件触发更合理(懒)
        if (ioo.gameMode.Boss != null)
        {
            ShowHealthBar();
            BossHealthProgress(ioo.gameMode.Boss.HealthProgress);
        }
        else
        {
            IsLock = false;
            HideHealthBar();
        }
        
        if (IsUsed)
        {
            _View.Mask.fillAmount -= ioo.nonStopTime.deltaTime * 0.2f;
        }

        if (_View.Mask.fillAmount == 0)
        {
            _View.Effect.SetActive(true);
        }
        else
        {
            _View.Effect.SetActive(false);
        }
    }


    #region Public Function
    public void InitMediator(PanelMainMediator mediator)
    {
        _Mediator = mediator;
    }

    /// <summary>
    /// 实时更新币数
    /// </summary>
    /// <param name="coin"></param>
    /// <param name="rate"></param>
    public void UpdateCoin(int coin, int rate)
    {
        _View.Text_Coin.text = coin.ToString() + "/" + rate.ToString();
        if (_View.Continue0.activeSelf)
        {
            _View.Text_Cointinue_Number.text = _Mediator.CointinueNumber();
        }
    }

    /// <summary>
    /// 铁钳怪特效
    /// </summary>
    public void OnTieQianEffect()
    {
        _View.TieQian_Effect.SetActive(true);
    }

    /// <summary>
    /// 闪电特效
    /// </summary>
    /// <param name="value"></param>
    public void OnJiGuangAttack(bool value)
    {
        _View.Flashing_Effect.SetActive(value);
        _View.BossWarning.SetActive(value);
    }

    public void OnWall()
    {
        _View.Flashing_Effect.SetActive(true);
    }

    /// <summary>
    /// 道具说明
    /// </summary>
    /// <param name="value"></param>
    public void OnDescribe(bool value)
    {
        _View.Describe.SetActive(value);
    }

    /// <summary>
    /// 触发冰柱特效
    /// </summary>
    public void OnTriggerIce()
    {
        _View.TriggerIce_Effect.SetActive(true);
    }

    /// <summary>
    /// 更新血条
    /// </summary>
    /// <param name="value"></param>
    public void BossHealthProgress(float value)
    {
        _View.Progress3.fillAmount = (value - 0.5f) * 2;
        if (value <= 0.5f)
            _View.Progress1.fillAmount = value * 2;


        if(_View.Progress2.fillAmount > _View.Progress3.fillAmount)
        {
            float rate = _View.Progress2.fillAmount - _View.Progress3.fillAmount;
            rate = rate < 0.1f ? 0.1f : rate;
            _View.Progress2.fillAmount -= ioo.nonStopTime.deltaTime * rate;
        }
        else
        {
            _View.Progress2.fillAmount = _View.Progress3.fillAmount;
        }

        if (_View.Progress0.fillAmount > _View.Progress1.fillAmount)
        {
            float rate = _View.Progress0.fillAmount - _View.Progress1.fillAmount;
            rate = rate < 0.1f ? 0.1f : rate;
            _View.Progress0.fillAmount -= ioo.nonStopTime.deltaTime * rate;
        }
        else
        {
            _View.Progress0.fillAmount = _View.Progress1.fillAmount;
        }
    }

    public void ShowThree()
    {
        _View.Three.SetActive(true);
        _View.One.SetActive(false);
    }

    public void ShowOne()
    {
        _View.One.SetActive(true);
        _View.Three.SetActive(false);
    }

    private bool IsUsed;
    public void MissileUsed(bool value)
    {
        IsUsed = value;
        _View.MissileObj.SetActive(true);

        if(IsUsed)
        {
            ShowOne();
            _View.Mask.fillAmount = 1;
        }
        else
        {
            ShowThree();
            _View.Mask.fillAmount = 0;
        }
    }

    private bool bosswarning;
    public void OnBossWarning()
    {
        bosswarning = true;
        _View.BossWarning.SetActive(true);
        ioo.audioManager.PlayBackMusic("SFX_Sound_Boss_Warning");
        DOVirtual.DelayedCall(3.1f, delegate
        {
            bosswarning = false;
            _View.BossWarning.SetActive(false);
            ioo.audioManager.StopBackMusic("SFX_Sound_Boss_Warning");
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"> 0:减分 1：加分 2：Shiled 3：Engine 4：Fix 5：Missile</param>
    /// <param name="pos"></param>
    /// <param name="value"></param>
    public void OnTips(int type, Vector3 pos, int value)
    {
        Canvas canvas = _View.Readuce.canvas;
        Vector2 pos0 = Camera.main.WorldToScreenPoint(pos);
        Vector3 pos1;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.transform as RectTransform, pos0, canvas.worldCamera, out pos1))
        {

        }

        Text text = null;
        Image image = null;
        Vector3 targetpos = Vector3.zero;
        switch (type)
        {
            case 0:
                for (int i = 0; i < _View.ReaduceList.Count; ++i )
                {
                    if (!_View.ReaduceList[i].gameObject.activeSelf)
                    {
                        text = _View.ReaduceList[i];
                        break;
                    }
                }
                
                if (text == null)
                {
                    text = GameObject.Instantiate(_View.ReaduceList[0], _View.RectParent)as Text;
                    text.name = "Readuce" + _View.ReaduceList.Count;
                }
                text.gameObject.SetActive(true);
                text.text = "-" + value.ToString();
                text.transform.position = pos1;
                _View.ReaduceList.Add(text);
                targetpos = pos1 + Vector3.up * 3;
                break;
            case 1:
                  for (int i = 0; i < _View.AddList.Count; ++i )
                {
                    if (!_View.AddList[i].gameObject.activeSelf)
                    {
                        text = _View.AddList[i];
                        break;
                    }
                }
                
                if (text == null)
                {
                    text = GameObject.Instantiate(_View.AddList[0], _View.RectParent) as Text;
                    text.name = "Readuce" + _View.AddList.Count;
                }
                text.gameObject.SetActive(true);
                text.text = "+" + value.ToString();
                text.transform.position = pos1;
                _View.AddList.Add(text);
                targetpos = _View.Time.Text_Number2.transform.position;
                break;
            case 2:
                _View.Shiled.gameObject.SetActive(true);
                _View.Shiled.transform.position = pos1;
                image = _View.Shiled;
                break;
            case 3:
                _View.Engine.gameObject.SetActive(true);
                _View.Engine.transform.position = pos1;
                image = _View.Engine;
                break;
            case 4:
                _View.Fix.gameObject.SetActive(true);
                _View.Fix.transform.position = pos1;
                image = _View.Fix;
                break;
            case 5:
                _View.Missile.gameObject.SetActive(true);
                _View.Missile.transform.position = pos1;
                image = _View.Missile;
                break;
        }

        if (text != null)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(text.transform.DOMove(targetpos, 1));
            sequence.Append(text.DOColor(new Color(1, 1, 1, 0), 0.3f));
            sequence.OnComplete(delegate 
            { 
                text.color = new Color(1, 1, 1, 1);
                text.gameObject.SetActive(false);
            });
        }

        if (image != null)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(image.transform.DOMoveY(image.transform.position.y + 3, 1));
            sequence.Append(image.DOColor(new Color(1, 1, 1, 0), 0.3f));
            sequence.OnComplete(delegate
            {
                image.color = new Color(1, 1, 1, 1);
                image.gameObject.SetActive(false);
            });
        }

        if (type == 0)
        {
            if (!_View.BossWarning.activeSelf)
            {
                _View.BossWarning.SetActive(true);
                DOVirtual.DelayedCall(1, delegate 
                { 
                    if (!bosswarning)
                    {
                        _View.BossWarning.SetActive(false);
                    }
                });
            }
        }
    }

    public void Lock()
    {
        IsLock = true;
        _View.LockRectTran.gameObject.SetActive(true);
    }

    public void OnWater(bool value)
    {
        _View.Water.SetActive(value);
    }

    private Vector3 showpos;
    public void OnShowUI(Vector3 pos)
    {
        showpos = pos;
    }

    public void OnGorgeAttack()
    {
        _View.BoLiSui_Effect.SetActive(true);
    }

    public void ShowHurryPush()
    {
        _View.HurryPush.SetActive(true);
        _View.Gather.SetActive(false);
    }

    public void HideHurryPush()
    {
        _View.HurryPush.SetActive(false);
        _View.Gather.SetActive(true);
    }

    private List<GorgeBossHitTrigger> QTEList = new List<GorgeBossHitTrigger>();
    public void OnQTE(List<GorgeBossHitTrigger> list)
    {
        QTEList = list;
    }

    public void OnBreak(Vector3 pos)
    {
        _View.Break.SetActive(true);
        Canvas canvas   = _View.Mask.canvas;
        Vector2 pos0    = Camera.main.WorldToScreenPoint(pos);
        Vector3 pos1;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.transform as RectTransform, pos0, canvas.worldCamera, out pos1))
        {
            _View.Break.transform.position = pos1;
        }
    }
    #region 响应玩家操作事件

    #endregion

    #endregion

    #region Private Function
    private void ShowHealthBar()
    {
        _View.Boss_Health_bar.SetActive(true);
        if (ioo.gameMode.Map == E_Map.City)
            _View.City.SetActive(true);
        else
            _View.Gorge.SetActive(true);
    }

    private void HideHealthBar()
    {
        _View.Progress0.fillAmount = 1;
        _View.Progress1.fillAmount = 1;
        _View.Progress2.fillAmount = 1;
        _View.Progress3.fillAmount = 1;
        _View.Boss_Health_bar.SetActive(false);
    }

    /// <summary>
    /// 播放游戏背景音效
    /// </summary>
    private void PlaybackGroundSound()
    {
        string soundName = string.Empty;
        switch(ioo.gameMode.Map)
        {
            case E_Map.City:
                soundName = "AnimalCity";
                break;
            case E_Map.Gorge:
                soundName = "HighWay";
                break;
        }
        ioo.audioManager.PlayBackMusic(soundName);
    }
 #endregion
}
