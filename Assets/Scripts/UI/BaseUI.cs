/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   BaseUI.cs
 * 
 * 简    介:    和UI逻辑相关脚本的基类
 * 
 * 创建标识：   Pancake 2016/7/21 17:23:21
 * 
 * 修改描述：   2017/3/29 引入Mediator的注册
 * 
 */


using UnityEngine;
using System.Collections;

   
public abstract class BaseUI : MonoBehaviour
{
    #region MediatorPlug
    public MediatorPlug MP;

    public abstract void OnRegesterMediatorPlug();
    #endregion

    #region Cache gameObject & transfrom

    private Transform _CachedTransform;
	/// <summary>
	/// Gets the cached transform.
	/// </summary>
	/// <value>The cached transform.</value>
	public Transform cachedTransform
	{
		get
		{
			if (!_CachedTransform)
			{
				_CachedTransform = this.transform;
			}
			return _CachedTransform;
		}
	}
		
	private GameObject _CachedGameObject;
	/// <summary>
	/// Gets the cached game object.
	/// </summary>
	/// <value>The cached game object.</value>
	public GameObject cachedGameObject
	{
		get
		{
			if (!_CachedGameObject)
			{
				_CachedGameObject = this.gameObject;
			}
			return _CachedGameObject;
		}
	}

    private Camera _UiCamera;
    public Camera uiCamera
    {
        get
        {
            if (!_UiCamera)
            {
                _UiCamera = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
            }
            return _UiCamera;
        }
    }
      
	#endregion
		
	#region UIType & EnumObjectState
	/// <summary>
	/// The state.
	/// </summary>
	protected EnumObjectState state = EnumObjectState.None;

	/// <summary>
	/// Occurs when state changed.
	/// </summary>
	public event StateChangedEvent StateChanged;

	/// <summary>
	/// Gets or sets the state.
	/// </summary>
	/// <value>The state.</value>
	public EnumObjectState State
	{
		protected set
		{
			if (value != state)
			{
				EnumObjectState oldState = state;
				state = value;
				if (null != StateChanged)
				{
					StateChanged(this, state, oldState);
				}
			}
		}
		get { return this.state; }
	}
	
	/// <summary>
	/// Gets the type of the user interface.
	/// </summary>
	/// <returns>The user interface type.</returns>
	public abstract EnumUIType GetUIType ();

	#endregion

	/// <summary>
	/// UI层级置顶
	/// </summary>
	protected virtual void SetDepthToTop()
	{
			
	}
		
	// Use this for initialization
	void Start ()
	{
		OnStart ();
        OnRegesterMediatorPlug();
	}

	void Awake()
	{
		this.State = EnumObjectState.Initial;
		OnAwake ();
	}
		
	// Update is called once per frame
	void Update ()
	{
		if (EnumObjectState.Ready == this.state) 
		{
			OnUpdate(Time.deltaTime);
		}
	}

	/// <summary>
	/// Release this instance.
	/// </summary>
	public void Release()
	{
		this.State = EnumObjectState.Closing;
		GameObject.Destroy (cachedGameObject);
		OnRelease ();
        OnPlayOpenUIAudio();
	}


    protected abstract void OnStart();

    protected virtual void OnAwake()
	{
		this.State = EnumObjectState.Loading;
		//播放音乐
		this.OnPlayOpenUIAudio();
	}
		
	protected virtual void OnUpdate(float deltaTime)
	{
			
	}

    protected abstract void OnRelease();
		

		
	/// <summary>
	/// 播放打开界面音乐
	/// </summary>
	protected virtual void OnPlayOpenUIAudio()
	{
			
	}
		
	/// <summary>
	/// 播放关闭界面音乐
	/// </summary>
	protected virtual void OnPlayCloseUIAudio()
	{
			
	}

	protected virtual void SetUI(params object[] uiParams)
	{
		this.State = EnumObjectState.Loading;
	}
		
	public virtual void SetUIparam(params object[] uiParams)
	{
			
	}
		
		
	protected virtual void OnLoadData()
	{
			
	}
		
	public void SetUIWhenOpening(params object[] uiParams)
	{
		SetUI(uiParams);
		CoroutineController.Instance.StartCoroutine(AsyncOnLoadData());
	}
		
	private IEnumerator AsyncOnLoadData()
	{
		yield return new WaitForSeconds(0);
		if (this.State == EnumObjectState.Loading)
		{
			this.OnLoadData();
			this.State = EnumObjectState.Ready;
		}
	}
}
