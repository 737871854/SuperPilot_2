/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   BatMaterialChange.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/8/9 15:06:03
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;

public class BatMaterialChange : MonoBehaviour
{
    public Material MT_Normal;
    public Material MT_Forest;
    public Material MT_Valcano;
    public Material MT_Winter;

    public Renderer Renderer;

    void Update()
    {
        if (Renderer != null)
        {
            switch (ioo.gameMode.enviroment)
            {
                case GameMode.Enviroment.Normal:
                    Renderer.material = MT_Normal;
                    break;
                case GameMode.Enviroment.Forest:
                    Renderer.material = MT_Forest;
                    break;
                case GameMode.Enviroment.Volcano:
                    Renderer.material = MT_Valcano;
                    break;
                case GameMode.Enviroment.Winter:
                    Renderer.material = MT_Winter;
                    break;
            }
        }
    }
}
