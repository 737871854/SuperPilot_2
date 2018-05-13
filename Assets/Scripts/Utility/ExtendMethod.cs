/*
 * Copyright (c) 
 * 
 * 文件名称：   ExtendMethod.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/3/28 18:17:27
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;

public static class ExtendMethod
{
    public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
    {
        T t = obj.GetComponent<T>();
        if (null == t)
        {
            t = obj.AddComponent<T>();
        }

        return t;
    }

    /// <summary>
    /// 字符串转换浮点 如果转换失败 返回0;
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static float ToFloat(this string self)
    {
        float f = 0;
        float.TryParse(self, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out f);
        return f;
    }
    /// <summary>
    /// 字符串转整形 如果转换失败 返回0;
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static int ToInt(this string self)
    {
        int i = 0;
        int.TryParse(self, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out i);
        return i;
    }
}
