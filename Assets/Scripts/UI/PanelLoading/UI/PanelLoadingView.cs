/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   PanelLoadingView.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/5/12 11:16:33
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PanelLoadingView
{
    public Text Progress;
    public Text Text_Coin;
    public GameObject Warning_Ticket;
    public Text Ticket_Number;
    public GameObject Skip;
    public GameObject Effect_Please;

    public void Init(Transform transform)
    {
        Text_Coin = transform.Find("Text_Coin").GetComponent<Text>();
        Progress = transform.Find("Progress").GetComponent<Text>();

        Warning_Ticket = transform.Find("Warning_Ticket").gameObject;
        Ticket_Number = Warning_Ticket.transform.Find("Number").GetComponent<Text>();

        Skip = transform.Find("Press_Please").gameObject;

        Effect_Please = transform.Find("Press_Please/Effect_Press_Please").gameObject;
    }
}
