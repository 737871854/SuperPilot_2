/*
 * Copyright (c) 
 * 
 * 文件名称：   PoolManager.cs
 * 
 * 简    介:    对象池管理 通过Spawn获取一个对象，通过DeSpaw回收一个对象(对象池的有所对象过场全部销毁，不会保留)
 * 
 * 创建标识：   Pancake 2017/4/2 16:19:47
 * 
 * 修改描述：   Pancake 2017/4/26 为适应消防员一代变更
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using Need.Mx;

public class PoolManager : MonoBehaviour
{

    private const string CONTAINER_NAME = "SimplePool";
    private static Transform _parentTransform;
    public static Transform ParentTransform
    {
        get
        {
            if (null == _parentTransform)
            {
                _parentTransform = new GameObject(CONTAINER_NAME).transform;
            }
            return _parentTransform;
        }
    }

    /// <summary>
    /// 对象池字典
    /// </summary>
    private Dictionary<string, GameObjectPool> poolDic = new Dictionary<string, GameObjectPool>();

    // 预加载
    private List<string> preLoadNameList = new List<string>();
    private List<int> preLoadCountList = new List<int>();

    // 场景管理器调用
    public List<string> PreLoadNameList { get { return preLoadNameList; } }
    public List<int> PreLoadCountList { get { return preLoadCountList; } }

    // 如果要使用进度预加载，需手动设置为True；如果使用了AddPreLoad方法，则无需手动设置
    public bool NeedPreLoadInLoading { get; set; }
  

    void Awake()
    {
        // 初始化
        LoadPoolConfig();
    }

    /// <summary>
    /// 加载对象池配置文件
    /// </summary>
    private void LoadPoolConfig()
    {
        GameObjectPoolList poolList = Resources.Load<GameObjectPoolList>(Const.Pool_Config_Obj_Path);
        if (null == poolList)
            return;
        foreach(GameObjectPool pool in poolList.poolList)
        {
            poolDic.Add(pool.name, pool);
        }
    }

    /// <summary>
    /// 预先加载(在进入场景后调。如果不想在进度条中进行加载)
    /// </summary>
    public void PreLoad()
    {
        Dictionary<string, GameObjectPool>.Enumerator er = poolDic.GetEnumerator();
        while (er.MoveNext())
        {
            er.Current.Value.PreLoad();
        }
    }

    /// <summary>
    /// 添加预加载 （针对一些需要动态加载的对象，如果是相对明确的对象，比如道具类，可在配置中提前配置好预加载个数）
    /// </summary>
    /// <param name="name">预加载对象的名字</param>
    /// <param name="count">预加载对象的数量</param>
    public void AddPreLoad(string name, int count = 1)
    {
        NeedPreLoadInLoading = true;

        if (preLoadNameList.Contains(name))
        {
            Debug.Log("The name : " + name + " has been in the preLoadNameList !");
            return;
        }

        Dictionary<string, GameObjectPool>.Enumerator er = poolDic.GetEnumerator();
        while (er.MoveNext())
        {
            if (!er.Current.Key.Equals(name))
                continue;

            if (er.Current.Value.PreCount != 0)
            {
                Debug.Log("The Object :" + name + " has been set preLoadCount !");
                return;
            }

            preLoadNameList.Add(name);
            preLoadCountList.Add(count);
            break;
        }
    }

    /// <summary>
    /// 仅暴露接口给ScenesManager (无需手动调用) 
    /// </summary>
    public void OnPreLoad()
    {
        Dictionary<string, GameObjectPool>.Enumerator er = poolDic.GetEnumerator();
        while (er.MoveNext())
        {
            if (er.Current.Value.PreCount != 0)
            {
                preLoadNameList.Add(er.Current.Key);
                preLoadCountList.Add(er.Current.Value.PreCount);
            }
        }
    }

    /// <summary>
    /// 完成预加载后调用
    /// </summary>
    public void EndPreLoad()
    {
        NeedPreLoadInLoading = false;
        preLoadNameList.Clear();
        preLoadCountList.Clear();
    }


    /// <summary>
    /// 创建指定pool，并指定pool中对象个数
    /// </summary>
    /// <param name="poolName"></param>
    /// <param name="count"></param>
    public void CreatePool(string poolName, int count)
    {
        GameObjectPool pool;
        if(poolDic.TryGetValue(poolName, out pool))
        {
            pool.CreateByCount(count);          
        }
        else
            Debug.LogWarning("Pool: " + pool.name + "is not exits!");
    }

    /// <summary>
    /// 从对象池中获取指定的对象
    /// </summary>
    /// <param name="poolName"></param>
    /// <returns></returns>
    public GameObject Spawn(string poolName)
    {
        GameObjectPool pool;
        if (poolDic.TryGetValue(poolName, out pool))
        {
            return pool.Spawn();
        }

        Debug.LogWarning("Pool: " + pool.name + "is not exits!");
        return null;
    }

    /// <summary>
    /// 回收对象
    /// </summary>
    /// <param name="go"></param>
    public void DeSpawn(GameObject go)
    {
        Dictionary<string, GameObjectPool>.Enumerator er = poolDic.GetEnumerator();
        while(er.MoveNext())
        {
            if (er.Current.Value.Contain(go))
                er.Current.Value.Destory(go);
            else
                continue;
        }
    }

    /// <summary>
    /// 清除所有对象
    /// </summary>
    public void Clear()
    {
        Dictionary<string, GameObjectPool>.Enumerator er = poolDic.GetEnumerator();
        while(er.MoveNext())
        {
            er.Current.Value.Clear();
        }
    }


    //List<GameObject> list = new List<GameObject>();
    ///// <summary>
    ///// 使用范例
    ///// </summary>
    //void OnGUI()
    //{
    //    GUI.Label(new Rect(400, 50, 100, 100), "对象池使用范例");

    //    if (GUI.Button(new Rect(300, 100, 300, 100), "创建一个Cube"))
    //    {
    //        GameObject go = ioo.panelManager.Spawn("Cube");
    //        list.Add(go);
    //    }

    //    if (GUI.Button(new Rect(300, 200, 300, 100), "销毁最早创建的Cube"))
    //    {
    //        if(list.Count > 0)
    //        {
    //            GameObject temp = list[0];
    //            list.RemoveAt(0);
    //            ioo.panelManager.DeSpawn(temp);
    //        }
    //    }

    //    if (GUI.Button(new Rect(300, 300, 300, 100), "创建一个Capsule对象池，个数为3"))
    //    {
    //        ioo.panelManager.CreatePool("Capsule", 3);
    //    }
    //}
}
