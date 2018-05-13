/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   PanelMainView.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/5/10 10:01:30
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PanelMainView
{
    public struct S_Time
    {
        public Text Text_Number0;
        public Text Text_Number1;
        public Text Text_Number2;
        public Text Text_Number3;
        public Text Text_Number4;
    }
    public S_Time Time;

    public struct S_Score
    {
        public Text Text_Number0;
        public Text Text_Number1;
        public Text Text_Number2;
        public Text Text_Number3;
        public Text Text_Number4;
    }
    public S_Score Score;

    public Text Text_Coin;

    // 续币界面
    public GameObject Continue;
    public Text Text_Cointinue_TimeUp;
    public Text Text_Cointinue_Number;

    public GameObject Continue0;
    public GameObject Continue1;

    // 聚齐
    public GameObject Gather;
    public Image Progress;
    public Image Yellow;
    public Image Blue;
    public GameObject Gather_Effect;

    // 描述
    public GameObject Describe;

    // 特效
    public GameObject TieQian_Effect;
    public GameObject Flashing_Effect;
    public GameObject TriggerIce_Effect;
    public GameObject BoLiSui_Effect;

    /// <summary>
    /// boss血条
    /// </summary>
    public GameObject Boss_Health_bar;
    public Image Progress0;
    public Image Progress1;
    public Image Progress2;
    public Image Progress3;
    public GameObject City;
    public GameObject Gorge;

    // 导弹Tips
    public GameObject MissileObj;
    public GameObject Three;
    public GameObject One;
    public Image Mask;
    public GameObject Effect;

    // 加减分
    public RectTransform RectParent;
    public Text Add;
    public Text Readuce;
    public Image Shiled;
    public Image Engine;
    public Image Fix;
    public Image Missile;

    public RectTransform LockRectTran;
    public GameObject Lock;
    public GameObject UnLock;

    public List<Text> AddList = new List<Text>();
    public List<Text> ReaduceList = new List<Text>();

    public GameObject BossWarning;

    public GameObject Water;

    public GameObject HurryPush;
    public Image Hurry0;
    public Image Hurry1;

    public GameObject Hit;
    public List<GameObject> HitList;
    public GameObject Break;

    public GameObject Warning_Ticket;
    public Text Ticket_Number;

    public List<GameObject> BulletList;


    public void Init(Transform transform)
    {
        Warning_Ticket = transform.Find("Warning_Ticket").gameObject;
        Ticket_Number = Warning_Ticket.transform.Find("Number").GetComponent<Text>();

        // 时间
        Time.Text_Number0 = transform.Find("Clock/Seconds/Number0").GetComponent<Text>();
        Time.Text_Number1 = transform.Find("Clock/Seconds/Number1").GetComponent<Text>();
        Time.Text_Number2 = transform.Find("Clock/Seconds/Number2").GetComponent<Text>();

        Time.Text_Number3 = transform.Find("Clock/Mil/Number0").GetComponent<Text>();
        Time.Text_Number4 = transform.Find("Clock/Mil/Number1").GetComponent<Text>();

        // 积分
        Score.Text_Number0 = transform.Find("Score/Score/Number0").GetComponent<Text>();
        Score.Text_Number1 = transform.Find("Score/Score/Number1").GetComponent<Text>();
        Score.Text_Number2 = transform.Find("Score/Score/Number2").GetComponent<Text>();
        Score.Text_Number3 = transform.Find("Score/Score/Number3").GetComponent<Text>();
        Score.Text_Number4 = transform.Find("Score/Score/Number4").GetComponent<Text>();

        Text_Coin = transform.Find("FillCoin/Text_Coin").GetComponent<Text>();

        // 续币
        Continue = transform.Find("Continue").gameObject;
        Text_Cointinue_TimeUp = Continue.transform.Find("Image/TimeUp").GetComponent<Text>();

        Continue0 = Continue.transform.Find("Image/Continue/Continue0").gameObject;
        Continue1 = Continue.transform.Find("Image/Continue/Continue1").gameObject;

        Text_Cointinue_Number = Continue0.transform.Find("Number").GetComponent<Text>();

        Gather      = transform.Find("Gather").gameObject;
        Progress    = transform.Find("Gather/Progress").GetComponent<Image>();
        Yellow      = transform.Find("Gather/Image0").GetComponent<Image>();
        Blue        = transform.Find("Gather/Image1").GetComponent<Image>();
        Gather_Effect = transform.Find("Gather/Effect_Progress_UI").gameObject;

        // 玻璃破碎特效
        TieQian_Effect  = transform.Find("Effect _ bolisui_robot_UI").gameObject;
        Flashing_Effect = transform.Find("Effect_Electromagnetism_UI").gameObject;
        TriggerIce_Effect = transform.Find("Effect_Freeze_UI").gameObject;
        BoLiSui_Effect  = transform.Find("Effect UI_boli_ sui").gameObject;

        Describe        = transform.Find("Describe").gameObject;

        Boss_Health_bar = transform.Find("BossHealthBar").gameObject;
        Progress0 = Boss_Health_bar.transform.Find("Progress0").GetComponent<Image>();
        Progress1 = Boss_Health_bar.transform.Find("Progress1").GetComponent<Image>();
        Progress2 = Boss_Health_bar.transform.Find("Progress2").GetComponent<Image>();
        Progress3 = Boss_Health_bar.transform.Find("Progress3").GetComponent<Image>();
        City      = Boss_Health_bar.transform.Find("City").gameObject;
        Gorge     = Boss_Health_bar.transform.Find("Gorge").gameObject;

        // 导弹Tips
        MissileObj = transform.Find("Tips/Missile").gameObject;
        Three   = transform.Find("Tips/Missile/Three").gameObject;
        One     = transform.Find("Tips/Missile/One").gameObject;
        Mask    = transform.Find("Tips/Missile/Mask").GetComponent<Image>();
        Effect  = MissileObj.transform.Find("Effect").gameObject;

        RectParent  = transform.Find("Tips").GetComponent<RectTransform>();
        Readuce     = RectParent.Find("Readuce").GetComponent<Text>();
        Add         = RectParent.Find("Add").GetComponent<Text>();
        Shiled      = RectParent.Find("Shiled").GetComponent<Image>();
        Engine      = RectParent.Find("Engine").GetComponent<Image>();
        Fix         = RectParent.Find("Fix").GetComponent<Image>();
        Missile     = RectParent.Find("Missiles").GetComponent<Image>();

        AddList.Add(Add);
        ReaduceList.Add(Readuce);

        BossWarning = transform.Find("BossWarning").gameObject;

        Water = transform.Find("Water").gameObject;

        LockRectTran    = transform.Find("Lock").GetComponent<RectTransform>();
        Lock            = LockRectTran.transform.Find("Lock").gameObject;
        UnLock          = LockRectTran.transform.Find("UnLock").gameObject;

        HurryPush       = transform.Find("HurryPush").gameObject;
        Hurry0          = HurryPush.transform.Find("Hurry0").GetComponent<Image>();
        Hurry1          = HurryPush.transform.Find("Hurry1").GetComponent<Image>();

        BulletList = new List<GameObject>();
        for (int i = 0; i < 30; ++i )
        {
            BulletList.Add(transform.Find("Bullets/GridLayout/Image" + i).gameObject);
        }
       
        Hit = transform.Find("HitEffect").gameObject;
        HitList = new List<GameObject>();
        for (int i = 0; i < 3; ++i )
        {
            GameObject go = transform.Find("HitEffect/Effect" + i).gameObject;
            HitList.Add(go);
        }
        Break = Hit.transform.Find("Effect_Break").gameObject;

    }
}
