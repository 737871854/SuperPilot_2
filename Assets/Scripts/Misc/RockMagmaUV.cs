/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   RockMagmaUV.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/7/31 15:53:05
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;

public class RockMagmaUV : MonoBehaviour
{
    public Material material;
    public Material material1;

    private Vector2 offset;
    void Update()
    {
        offset += Vector2.up * Time.deltaTime * 0.1f;
        material.SetTextureOffset("_MainTex", offset);
        if (material1 != null)
            material1.SetTextureOffset("_MainTex", offset);
    }
}
