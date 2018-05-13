/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   EffectManager.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/5/27 9:21:08
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;

public class EffectManager 
{
    private static readonly object _object = new object();
    private static EffectManager _instance = null;
    public static EffectManager Instance
    {
        get
        {
            if (null == _instance)
            {
                lock (_object)
                {
                    if (null == _instance)
                        _instance = new EffectManager();
                }
            }
            return _instance;
        }
    }


    #region Public Function
   /// <summary>
   /// 在指定位置播放特效
   /// </summary>
   /// <param name="name"></param>
   /// <param name="pos"></param>
    public void Spawn(string name, Vector3 pos)
    {
        GameObject effect = ioo.poolManager.Spawn(name);
        effect.GetOrAddComponent<EffectBehaviour>();
        effect.transform.position = pos;
    }

    /// <summary>
    /// 特效更随指定对象
    /// </summary>
    /// <param name="name"></param>
    /// <param name="trans"></param>
    public void Spawn(string name, Transform trans)
    {
        GameObject effect = ioo.poolManager.Spawn(name);
        EffectBehaviour eb = effect.GetOrAddComponent<EffectBehaviour>();
        eb.ToFollow = trans;
    }
    
    /// <summary>
    /// 在玩家身上的指定点播放特效
    /// </summary>
    public void SpawnEffectInPlayer(string name)
    {
        GameObject effect   = ioo.poolManager.Spawn(name);
        EffectBehaviour eb  = effect.GetOrAddComponent<EffectBehaviour>();
        eb.ToFollow         = ioo.gameMode.Player.EffectPoint;
    }
    #endregion
}
