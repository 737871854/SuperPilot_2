/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   PanelSummaryView.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/5/11 8:48:38
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PanelSummaryView
{
    public GameObject View0;
    public GameObject View1;
    public GameObject View2;

    public Text ResultScore;
    public Text Score;
    public Text BossScore;
    public Text DamageScore;
    public List<GameObject> ListSABC;

    public List<GameObject> HonourList = new List<GameObject>();

    public GameObject Warning_Ticket;
    public Text Ticket_Number;
    public Text Ticket_Count;
    public GameObject Defeated;

    public void Init(Transform transform)
    {
        View0 = transform.Find("View0").gameObject;
        View1 = transform.Find("View1").gameObject;
        View2 = transform.Find("View2").gameObject;

        ResultScore = View1.transform.Find("Data/Score/Score").GetComponent<Text>();
        Score       = View1.transform.Find("Data/Gold/Value").GetComponent<Text>();
        BossScore   = View1.transform.Find("Data/Boss/Value").GetComponent<Text>();
        DamageScore = View1.transform.Find("Data/Damage/Value").GetComponent<Text>();

        Ticket_Count = View2.transform.Find("Text").GetComponent<Text>();

        ListSABC = new List<GameObject>();
        for (int i = 0; i < 4; ++i )
        {
            ListSABC.Add(View1.transform.Find("Effect_SABC_UI" + i).gameObject);
        }

        for (int i = 0; i < 3; ++i)
        {
            GameObject Honour = transform.Find("View0/Honour" + i).gameObject;
            HonourList.Add(Honour);
        }

        Warning_Ticket = transform.Find("Warning_Ticket").gameObject;
        Ticket_Number = Warning_Ticket.transform.Find("Number").GetComponent<Text>();

        Defeated = transform.Find("Defeated").gameObject;
    }
}
