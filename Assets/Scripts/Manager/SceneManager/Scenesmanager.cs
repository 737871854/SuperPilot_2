/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   ScenesManager.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/5/23 15:22:31
 * 
 * 修改描述：   2017/5/25 修复部分Bug，更新API
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Need.Mx;
using System.Collections;

public class ScenesManager : MonoBehaviour
{
    void Awake()
    {
        SceneManager.sceneLoaded += OnLevelLoaded;
        EventDispatcher.AddEventListener(EventDefine.Event_Loading_End, OnEndLoading);
    }

    void Destroy()
    {
        SceneManager.sceneLoaded -= OnLevelLoaded;
        EventDispatcher.RemoveEventListener(EventDefine.Event_Loading_End, OnEndLoading);
    }
    
    // 需要加载的场景名
    private string _LevelToLoad = string.Empty;

    ////需要预加载的资源列表;
    //private List<string> _ResourceList = new List<string>();
    //private List<int> _GoCountInPool = new List<int>();

    #region 切换场景时调用的函数 Public Function
    /// <summary>
    /// 直接加载场景（无进度条）
    /// </summary>
    /// <param name="name"></param>
    public void LoadSceneDir(string name)
    {
        _LevelToLoad = name;
        ioo.audioManager.StopAll();
        ioo.poolManager.Clear();
        CoroutineController.Instance.StartCoroutine(AsyncLoadScene());
    }

    private EnumUIType _OpenUIType;
    /// <summary>
    /// 异步加载场景（有进度条）
    /// </summary>
    /// <param name="name"></param>
    public void LoadScene(string name, EnumUIType type)
    {
        _LevelToLoad        = name;
        _OpenUIType         = type;
        ioo.poolManager.Clear();
        SceneManager.LoadScene(SceneName.Loading);
        UIManager.Instance.OpenUI(EnumUIType.PanelLoading);
        ioo.audioManager.PlayBackMusic("Music_Loading");
        ioo.audioManager.PlayPersonMusic("Person_Sound_Guided");
    }
    #endregion

    #region 内部调用 Private Function
    private void OnLevelLoaded(Scene scene, LoadSceneMode mod)
    {
        if (scene.name == "Loading")
        {
             StartCoroutine(OnLoadLoadingLevel());
             string name = string.Empty;
             Material mt = null;
             switch (_LevelToLoad)
             {
                 case SceneName.City:
                     mt = Resources.Load("Material/City") as Material;
                     break;
                 case SceneName.Gorge:
                     mt = Resources.Load("Material/Gorge") as Material;
                     break;
             }
             RenderSettings.skybox = mt;
        }
        else
        {
            OnLoadScene();
        }
    }

    /// <summary>
    /// 载入了空场景
    /// </summary>
    private IEnumerator OnLoadLoadingLevel()
    {
        yield return new WaitForEndOfFrame();
        StartCoroutine(OnAsyncLoading());
    }

    /// <summary>
    /// 目标场景加载完成
    /// </summary>
    private int _ResIndex;
    private void OnLoadScene()
    {
        if (!ioo.poolManager.NeedPreLoadInLoading)
            return;

        _ResIndex           = 0;
        ioo.poolManager.OnPreLoad();
        if (ioo.poolManager.PreLoadNameList.Count == 0)
            return;
        
       StartCoroutine(OnPreLoadingRes());
    }

   
    private IEnumerator<AsyncOperation> AsyncLoadScene()
    {
        AsyncOperation oper = SceneManager.LoadSceneAsync(_LevelToLoad);
        yield return oper;
    }

    private void OnEndLoading()
    {
        ioo.gameMode.CurrentScene(_LevelToLoad);
        UIManager.Instance.CloseUI(EnumUIType.PanelLoading);
        UIManager.Instance.OpenUI(_OpenUIType);
    }
    #endregion


    private int _ToProgress;
    /// <summary>
    /// 异步加载场景
    /// </summary>
    /// <returns></returns>
    IEnumerator OnAsyncLoading()
    {
        _ToProgress = 50;
        // 为了美观，直接给个进度值 (没有实时准确的反映真实的进度值，障眼法而已)
        EventDispatcher.TriggerEvent(EventDefine.Event_Loading_Progress, _ToProgress);

        AsyncOperation op = SceneManager.LoadSceneAsync(_LevelToLoad);
        op.allowSceneActivation = false;
        while (op.progress < 0.9f)
        {
            yield return new WaitForEndOfFrame();
        }

        if (!ioo.poolManager.NeedPreLoadInLoading)
            _ToProgress = 100;
        else
            _ToProgress = 90;

        op.allowSceneActivation = true;

        EventDispatcher.TriggerEvent(EventDefine.Event_Loading_Progress, _ToProgress);
    }

    /// <summary>
    /// 预加载
    /// </summary>
    /// <returns></returns>
    IEnumerator OnPreLoadingRes()
    {
        int resCount = ioo.poolManager.PreLoadNameList.Count;
        float delta = 0.1f / resCount;

        int count = ioo.poolManager.PreLoadCountList[_ResIndex];
        ioo.poolManager.CreatePool(ioo.poolManager.PreLoadNameList[_ResIndex], count);
        ++_ResIndex;
        _ToProgress = (int)((0.9f + (_ResIndex) * delta) * 100);
        EventDispatcher.TriggerEvent(EventDefine.Event_Loading_Progress, _ToProgress);
        if (_ResIndex < resCount)
        {
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(OnPreLoadingRes());
        }
        else
        {
            // 防止浮点运算导致预加载完成后，进度还达不到100的情况
            EventDispatcher.TriggerEvent(EventDefine.Event_Loading_Progress, 110);
            yield return new WaitForSeconds(0.2f);
            Debug.Log("PreLoadingRes Over!!!!!!");
            ioo.poolManager.EndPreLoad();
            ioo.audioManager.StopBackMusic("Music_Loading");
        }
    }

}
