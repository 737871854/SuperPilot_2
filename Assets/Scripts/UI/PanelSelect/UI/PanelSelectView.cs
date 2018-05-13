/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   PanelSelectView.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/5/9 17:05:35
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PanelSelectView
{
    // 选地图
    public GameObject MapRoot;
    public Transform CityMap;
    public Transform GrogeMap;
    public GameObject CityDes;
    public GameObject GrogeDes;


    // 选飞机
    public GameObject ModelRoot;
    public class C_Head
    {
        public GameObject NoSelected;
        public GameObject Selected;
        public GameObject Des;
    }
    public List<C_Head> HeadList = new List<C_Head>();

    // 倒计时
    public Text Time;

    public Transform AirPlane;
    public List<GameObject> PlaneList;

    // 投币信息
    public Text Text_Coin;

    public GameObject Left0;
    public GameObject Left1;
    public GameObject Right0;
    public GameObject Right1;

    public GameObject Warning_Ticket;
    public Text Ticket_Number;

    public GameObject Effect_Please;

    public void Init(Transform transform)
    {
        // 选地图
        MapRoot = transform.Find("SelectMap").gameObject;
        CityMap = MapRoot.transform.Find("City").transform;
        CityDes = MapRoot.transform.Find("Des/City").gameObject;

        GrogeMap = MapRoot.transform.Find("Groe").transform;
        GrogeDes = MapRoot.transform.Find("Des/Groge").gameObject;

        // 选飞机
        ModelRoot = transform.Find("SelectModel").gameObject;
        for (int i = 0; i < 3; ++i )
        {
            C_Head item     = new C_Head();
            item.NoSelected = ModelRoot.transform.Find("Head/Item" + i + "/NoSelected").gameObject;
            item.Selected   = ModelRoot.transform.Find("Head/Item" + i + "/Selected").gameObject;
            item.Des = ModelRoot.transform.Find("Des/Item" + i).gameObject;
            HeadList.Add(item);
        }
        Time = transform.Find("Time/Time").GetComponent<Text>();

        AirPlane = ModelRoot.transform.Find("AirPlane").transform;
        PlaneList = new List<GameObject>();
        for (int i = 0; i < 3; ++i )
        {
            PlaneList.Add(AirPlane.transform.Find("Player" + i).gameObject);
        }

        // 投币信息
        Text_Coin = transform.Find("Coin/Text_Coin").GetComponent<Text>();

        Left0 = ModelRoot.transform.Find("Left0").gameObject;
        Left1 = ModelRoot.transform.Find("Left1").gameObject;
        Right0 = ModelRoot.transform.Find("Right0").gameObject;
        Right1 = ModelRoot.transform.Find("Right1").gameObject;

        Warning_Ticket = transform.Find("Warning_Ticket").gameObject;
        Ticket_Number = Warning_Ticket.transform.Find("Number").GetComponent<Text>();

        Effect_Please = transform.Find("Op/Image/Effect_Press_Please").gameObject;
    }
}
