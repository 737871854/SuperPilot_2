/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   CoinView.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/5/9 10:57:57
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PanelCoinView
{
    public Text Text_Coin;

    public RawImage Movie;

    public GameObject Warning_Ticket;
    public Text Ticket_Number;

    public GameObject Please0;
    public GameObject Please1;
    public GameObject Effect_Please1;

    public void Init(Transform transform)
    {
        Text_Coin = transform.Find("Text_Coin").GetComponent<Text>();

        Movie = transform.Find("RawImage").GetComponent<RawImage>();

        Warning_Ticket = transform.Find("Warning_Ticket").gameObject;
        Ticket_Number = Warning_Ticket.transform.Find("Number").GetComponent<Text>();

        Please0 = transform.Find("Effect_Please").gameObject;
        Please1 = transform.Find("Press_Please").gameObject;
        Effect_Please1 = Please1.transform.Find("Effect_Press_Please").gameObject;
    }
}
