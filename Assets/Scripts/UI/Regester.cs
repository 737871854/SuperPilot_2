/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   Regester.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/5/12 17:41:38
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Regester : MonoBehaviour
{
    public GameObject text;
    // Use this for initialization
    IEnumerator Start()
    {
        PlayerPrefs.SetString("CitySUVGUID", SystemInfo.deviceUniqueIdentifier);
        text.SetActive(true);
        yield return new WaitForSeconds(2);
        Application.Quit();
    }

    public int Value;
    public byte t0 = 0;
    public byte t1 = 0;
    public byte[] s0;
    public byte[] s1;

    public byte Value2;
    void Update()
    {
        //Value = System.BitConverter.ToInt32(new byte[] { t0, t1, 0, 0 }, 0);

        Value2 = 0x00 | 0x40 | 0x01;
    }
}
