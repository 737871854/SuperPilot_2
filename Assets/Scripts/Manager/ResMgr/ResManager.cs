/**
 * Copyright (c) 2015,Need Corp. ltd;
 * All rights reserved.
 * 
 * 文件名称：ResManager.cs
 * 简    述：全局单例，资源加载管理器，提供方便的下载放法;
 *   bool GetRes(string url, OnResLoadOK callback, LoadPriority param = LoadPriority.Normal)
 *   优先级会影响资源的加载先后;
 * 创建标识：Lorry  2012/10/28
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

namespace Need.Mx
{
    public class ResManager
    {
        #region 单例;
        private ResManager() { }
        private static ResManager instance;
        public static ResManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ResManager();
                }
                return instance;
            }
        }
        #endregion

        #region 公有方法;
        /// <summary>
        /// 加载Json文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="completeHandler"></param>
        public void LoadRes(string url, LoadHandler completeHandler)
        {
            string result = string.Empty;
            url = Const.GetLocalFileUrl(url);
            if (!File.Exists(url))
            {
                return;
            }
            using (StreamReader sr = new StreamReader(url, Encoding.UTF8))
            {
                result = sr.ReadToEnd();
            }

            if (null != completeHandler)
                completeHandler(new LoadedData(result, url, url));
        }
        #endregion
    }
    public delegate void LoadHandler(LoadedData data);
}
