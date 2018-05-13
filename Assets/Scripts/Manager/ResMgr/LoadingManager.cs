
using Need.Mx;
/**
 * Copyright (c) 2015,Need Corp. ltd
 * All rights reserved.
 * 文件名称：LoadingManager.cs
 * 简    述： Loading，用来加载显示进度条的;
 * 切换场景的时候用的着，首先要加载一下loading，把旧资源清理掉
 * 然后在去加载目标场景，并构建之，再加载一些需要加载的内容
 * 加载一项更新一下进度条，全部加载完毕后，关闭进度条，并且调回调函数，结束加载;
 * 创建标识：Terry 2015-03-26 04:35:45
*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public delegate void LoadHandler(LoadedData data);

public class LoadingManager
{
    #region Singleton
    protected static LoadingManager instance;
    public static LoadingManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new LoadingManager();
            }
            return instance;
        }
    }

    protected LoadingManager()
    {
        tobeLoadInfoQueue = new Queue<ToBeLoadInfo>();
    }
    #endregion

    private Action completeHandler;
    private Queue<ToBeLoadInfo> tobeLoadInfoQueue;

    private int totalCount = 0;
    private string loadText = string.Empty;
    private bool showProgress = true;

    private ToBeLoadInfo curLoadInfo = null;


    /// <summary>
    /// 开始加载;
    /// </summary>
    /// <param name="callback"></param>
    public void StartLoad(Action overHandler = null, bool needProgress = true, string loadText = "")
    {
        completeHandler = overHandler;
        showProgress = needProgress;

        totalCount = tobeLoadInfoQueue.Count;

        if (totalCount > 0)
        {
            curLoadInfo = tobeLoadInfoQueue.Dequeue();
            ResManager.Instance.LoadRes(curLoadInfo.Path, onLoadHandler);
        }
        else
        {
            if (completeHandler != null)
            {
                completeHandler();
                Debug.Log("Load Complete!");
            }
        }
    }

    public void AddJsonFiles(string filePath, LoadHandler loadHandler)
    {
        tobeLoadInfoQueue.Enqueue(new ToBeLoadInfo(filePath, loadHandler));
    }

    private void onLoadHandler(LoadedData data)
    {
        Debug.Log(data.FilePath + " LoadOver!");

        if (tobeLoadInfoQueue.Count == 0)
        {
            if (curLoadInfo != null && curLoadInfo.OnLoadHandler != null)
            {
                curLoadInfo.OnLoadHandler(data);
                curLoadInfo = null;
            }
            
            if (completeHandler != null)
            {
                completeHandler();
                Debug.Log("Load Complete!");
            }
        }
        else
        {
            //调用一下外部的回调;
            if (curLoadInfo != null && curLoadInfo.OnLoadHandler != null)
            {
                curLoadInfo.OnLoadHandler(data);
            }

            //继续加载下一个;
            curLoadInfo = tobeLoadInfoQueue.Dequeue();
            ResManager.Instance.LoadRes(curLoadInfo.Path, onLoadHandler);
        }
    }

    /// <summary>
    /// 将要加载的信息;
    /// </summary>
    class ToBeLoadInfo
    {
        /// <summary>
        /// 加载路径;
        /// </summary>
        public string Path;

        /// <summary>
        /// 外部回调;
        /// </summary>
        public LoadHandler OnLoadHandler;

        public ToBeLoadInfo(string path, LoadHandler onLoaded)
        {
            Path = path;
            OnLoadHandler = onLoaded;
        }
    }
}
