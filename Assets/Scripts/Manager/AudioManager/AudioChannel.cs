/*
 * Copyright (c) 
 * 
 * 文件名称：   AudioChannel.cs
 * 
 * 简    介:    负责音效资源的播放停止和销毁，以及播放位置
 * 
 * 创建标识：   Pancake 2017/4/20 11:48:00
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;

public class AudioChannel : MonoBehaviour
{
    private bool _isFree = true;

    /// <summary>
    /// 音效名字
    /// </summary>
    public string _name;

    private AudioSource _audioSource;
    /// <summary>
    /// 是否是Music
    /// </summary>
    public bool _isMusic;

    /// <summary>
    /// 是否循环播放
    /// </summary>
    private bool _isLoop;

    /// <summary>
    /// 指定播放音效的位置
    /// </summary>
    private Vector3 _targetPos;

    /// <summary>
    /// 指定音效播放到哪个物体
    /// </summary>
    private GameObject _targetObj;

    /// <summary>
    /// 更随指定物体
    /// </summary>
    private bool _isFollow;

    private float _startVolume;
    private float _endVolume;

    /// <summary>
    /// 音效
    /// </summary>
    private AudioClip _clip;

    public bool IsFree      { get { return _isFree; } }
    public string Name      { get { return _name; } }
    public AudioClip Clip   { get { return _clip; } }


    public void Init(string name, AudioClip clip, bool spatial = false)
    {
        if (spatial)
            _audioSource.spatialBlend = 0.3f;
        else
            _audioSource.spatialBlend = 0;

        _name = name;
        _clip = clip;
    }

    public void Init(string name, AudioClip clip, GameObject obj, bool spatial = false)
    {
        if (spatial)
            _audioSource.spatialBlend = 0.3f;
        else
            _audioSource.spatialBlend = 0;
        _name       = name;
        _clip       = clip;
        _targetObj  = obj;
        _isFollow   = true;
    }

    public void Init(string name, AudioClip clip, Vector3 pos, bool spatial = false)
    {
        if (spatial)
            _audioSource.spatialBlend = 0.3f;
        else
            _audioSource.spatialBlend = 0;
        _name       = name;
        _clip       = clip;
        _targetPos  = pos;
        _isFollow   = false;
    }

    /// <summary>
    /// 被回收后重置数据
    /// </summary>
    public void Reset()
    {
        _name       = string.Empty;
        _isMusic    = false;
        _targetObj  = null;
        _targetPos  = Vector3.zero;
        _audioSource.clip = null;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 是否正在播放
    /// </summary>
    /// <returns></returns>
    public bool IsPlaying()
    {
        if (_clip == null)
            return false;
        if (_audioSource.isPlaying)
            return true;
        return false;
    }

    /// <summary>
    /// Music必须设置
    /// </summary>
    /// <param name="ismusic"></param>
    /// <param name="isloop"></param>
    public void SetMusic(bool isloop, float start = 0, float end = 1)
    {
        _isMusic                = true;
        _isLoop                 = isloop;
        _startVolume            = start * SettingManager.Instance.GameVolume * 0.1f;
        _endVolume              = end * SettingManager.Instance.GameVolume * 0.1f;
        _audioSource.volume     = _startVolume;
        _audioSource.loop       = _isLoop;
    }

    public void SetSound()
    {
        _isMusic = false;
        _startVolume = _endVolume = SettingManager.Instance.GameVolume * 0.1f;

        _audioSource.volume = _startVolume;
        _audioSource.loop   = false;
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    public void Play()
    {
        if (_targetObj == null)
            transform.position = _targetPos;

        _isFree              = false;
        _audioSource.clip    = _clip;
        _audioSource.Play();
    }

    /// <summary>
    /// 停止播放音效
    /// </summary>
    public void Stop()
    {
        _isFree = true;
        _isLoop = false;
        _audioSource.Stop();
       
        ioo.audioManager.ToFree(this);
    }

    public void Idle()
    {
        _audioSource.volume *= 0.25f;
        _endVolume *= 0.25f;
    }

    public void Normal()
    {
        _audioSource.volume *= 4;
        _endVolume *= 4;
    }
  
    void Awake()
    {
        _audioSource = gameObject.GetOrAddComponent<AudioSource>();
    }

    void Update()
    {
        if (_isFollow)
        {
            if (_targetObj != null)
                transform.position = _targetObj.transform.position;
            else
            {
                _isFollow = false;
                Stop();
                return;
            }
        }

        if (_audioSource.clip != null)
        {
            if (!_audioSource.isPlaying)
            {
                if (!_isFree)
                    Stop();
            }
            else
            {
                if (_audioSource.volume < _endVolume)
                    _audioSource.volume += Time.deltaTime;
                else
                    _audioSource.volume = _endVolume;
            }
        }
    }
}
