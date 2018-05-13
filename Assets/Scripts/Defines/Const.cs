
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Const
{
    public static string LocalTestWebUrl = "file:///" + Application.streamingAssetsPath + "/";		//默认安装包本地路径更新地址

    public static string Props_Json_Path = "json/Props.json";
    public static string Weapon_Json_Path = "json/Weapon.json";

    public static float GAME_CONFIT_READY_TIME      = 2;
    public static float GAME_CONFIT_LAND_TIME       = 2;
    public static float GAME_CONFIG_TIME            = 30;
    public static float GAME_CONFIG_ATTACK_RADIUS   = 120;

    public static string Setting_Coinfig_Path   = "SettingConfig.json";
    public static string Audio_Coinfig_Path     = "aduiolist.txt";
    public static string Pool_Config_Path       = "poollist.txt";
    public static string Pool_Config_Obj_Path   = "gameobjectpool";

    public static string Path_Config_Obj_Ready  = "readypath";
    public static string Path_Config_Obj_City   = "citypath";
    public static string Path_Config_Obj_Gorge  = "gorgepath";
    public static string Path_Config_Obj_Land   = "landpath";

    public static string Path_Config_Idle_Movie_Path = "Movies/gameMovie";

    public static string GetLocalFileUrl(string parPath)
    {
        string path = "";
        path = Const.LocalTestWebUrl.Replace("file:///", "");
       
        return path + parPath;
    }
}





