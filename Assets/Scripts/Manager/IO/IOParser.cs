/**
* Copyright (c) 2012,广州纷享游艺设备有限公司
* All rights reserved.
* 
* 文件名称：IOOperation.cs
* 简    述：协议解析方法
* 创建标识：meij  2015/10/28
* 修改标识：meij  2015/11/06
* 修改描述：加入屏幕坐标信息。
*/
using UnityEngine;
using System.Collections;

public class IOParser
{
    private static float width;
    private static float height;
    private static float miniheight;
    /// <summary>
    /// 取一个字节中指定的某一位
    /// </summary>
    /// <param name="i">所求的位的索引</param>
    /// <param name="k">当前字节</param>
    /// <returns></returns>
    public static byte GetBit(byte i, byte k)
    {
        byte value = 0;
        value = k;
        value = (byte)(value << (7-i));
        value = (byte)(value >> 7);
        return value;
    }

    /// <summary>
    ///  byte类型转为float类型
    /// </summary>
    /// <param name="b"></param>
    /// <returns></returns>
    public static float Byte2Float(byte b)
    {
        if ((b & 0x80) != 0)
        {
            return -((float)(b & 0x7f));
        }
        return (float)b;
    }
    public static string ByteArray2String(byte[] b)
    {
        string str = "";
        for (int i = 0; i < b.Length - 1; ++i)
        {
            str += b[i].ToString();
            str += "/";
        }
        str += b[b.Length - 1].ToString();
        return str;
    }
    public static byte[] String2IntArray(string str)
    {
        string[] strArray = str.Split('/');
        byte[] array = new byte[strArray.Length];
        for (int i = 0; i < strArray.Length; ++i)
        {
            array[i] = (byte)int.Parse(strArray[i]);
        }
        return array;
    }
    /// <summary>
    /// 根据得到的字节数进行解析
    /// </summary>
    /// <param name="bytes">上位机一次能接受到的所有的字节数</param>
    /// <param name="ioEvent">存储按键事件</param>
    public static void GetBools(byte[] bytes, IOEvent ioEvent)
    {
        //byte[0]为协议头
        byte k = bytes[1];
        byte q = bytes[2];

        ioEvent.IsGather    = (IOParser.GetBit(0, k) == 1);
        ioEvent.IsPullDown  = (IOParser.GetBit(1, k) == 1);
        ioEvent.IsPullUp    = (IOParser.GetBit(2, k) == 1);
        ioEvent.IsTurnRight = (IOParser.GetBit(3, k) == 1);
        ioEvent.IsTurnLeft  = (IOParser.GetBit(4, k) == 1);
        ioEvent.IsMissile   = (IOParser.GetBit(5, k) == 1);
        ioEvent.IsStart     = (IOParser.GetBit(6, k) == 1);
        ioEvent.IsCoin      = (IOParser.GetBit(7, k) == 1);

        ioEvent.IsConfirm   = (IOParser.GetBit(7, q) == 1);
        ioEvent.IsSelect    = (IOParser.GetBit(6, q) == 1);
        ioEvent.IsResetEye  = (IOParser.GetBit(5, q) == 1);
        ioEvent.IsUpEye     = (IOParser.GetBit(4, q) == 1);
        ioEvent.IsDownEye   = (IOParser.GetBit(3, q) == 1);
        ioEvent.IsTicket    = (IOParser.GetBit(2, q) == 1);
    }
}


