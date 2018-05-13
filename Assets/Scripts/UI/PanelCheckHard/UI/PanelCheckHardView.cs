/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   PanelCheckHardView.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/5/12 18:03:32
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;

public class PanelCheckHardView
{
    public class CItem
    {
        public GameObject markWrong;
        public GameObject markRight;
        public GameObject labelEn;
        public GameObject labelCh;
    }
    public List<CItem> Check;
   

    public void Init(Transform transform)
    {
        Check = new List<CItem>();
        for (int i = 0; i < 14; ++i )
        {
            CItem citem = new CItem();
            GameObject item = transform.Find("Check/Item" + i).gameObject;
            citem.markRight = item.transform.Find("Checkmark0").gameObject;
            citem.markWrong = item.transform.Find("Checkmark1").gameObject;
            citem.labelCh = item.transform.Find("Label0").gameObject;
            citem.labelEn = item.transform.Find("Label1").gameObject;
            Check.Add(citem);
        }

    }
}
