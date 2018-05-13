/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   PanelMapMoveBehaviour.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/7/4 9:30:50
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Need.Mx;

public class PanelMapMoveBehaviour : MonoBehaviour
{
    // 前到后
    private List<Vector3> Path0_0;
    // 后道前
    private List<Vector3> Path1_0;
    private List<Vector3> Path0_1;
    private List<Vector3> Path1_1;

    public Transform path0;
    public Transform path1;

    public SpriteRenderer SR;
    public MovieTexture MT;
    public MeshRenderer MR;
    public bool IsSelected;
    public bool IsCity;

    public GameObject CityDes;
    public GameObject GorgeDes;

    public GameObject SelectEffect;

    private Material MT_Movie;
    private Material MT_Sprite;
    private bool CanMove;
    private bool IsLeft;

    // Use this for initialization
    void Start()
    {
        Path0_0 = new List<Vector3>();
        Path0_1 = new List<Vector3>();
        Path1_0 = new List<Vector3>();
        Path1_1 = new List<Vector3>();

        MT_Movie = MR.GetComponent<MeshRenderer>().material;
        MT_Movie.SetTexture("_MainTex", MT);
        MT.Play();
        MT.loop = true;

        SetMaterialColor();

        for (int i = 0; i < path0.transform.childCount; ++i )
        {
            Path0_0.Add(path0.transform.GetChild(i).transform.position);
        }

        for (int i = 0; i < path1.transform.childCount; ++i)
        {
            Path1_0.Add(path1.transform.GetChild(i).transform.position);
        }

        for (int i = Path0_0.Count - 1; i >= 0; --i)
        {
            Path0_1.Add(Path0_0[i]);
        }

        for (int i = Path1_0.Count - 1; i >= 0; --i)
        {
            Path1_1.Add(Path1_0[i]);
        }

        if (!IsSelected)
            StartCoroutine(MovieStop());

        EventDispatcher.AddEventListener(EventDefine.Event_Select_Map_Move_Left, OnLeft);
        EventDispatcher.AddEventListener(EventDefine.Event_Select_Map_Move_Right, OnRight);
    }

    IEnumerator MovieStop()
    {
        yield return new WaitForEndOfFrame();
        MT.Pause();
    }

    void Destroy()
    {
        MT.Stop();
        EventDispatcher.RemoveEventListener(EventDefine.Event_Select_Map_Move_Left, OnLeft);
        EventDispatcher.RemoveEventListener(EventDefine.Event_Select_Map_Move_Right, OnRight);
    }

    void OnDisable()
    {
        if (MT != null && MT.isPlaying)
            MT.Stop();
    }

    private void OnRight()
    {
        CanMove = true;
        IsLeft = false;
        index = 1;
    }

    private void OnLeft()
    {
        CanMove = true;
        IsLeft = true;
        index = 1;
    }

    private int index;
    // Update is called once per frame
    void Update()
    {
        if (!CanMove)
            return;

        if (IsSelected)
        {
            if (IsLeft)
            {
                Vector3 nextPos = Path0_0[index];
                Vector3 dir = nextPos - transform.position;
                if (dir.magnitude > 0.1f)
                {
                    transform.position += dir.normalized * Time.deltaTime * 4;
                }
                else
                {
                    if (index == Path0_0.Count - 1)
                    {
                        transform.position = Path0_0[index];
                        IsSelected = false;
                        MoveEnd();
                    }
                    else
                        ++index;
                }
            }
            else
            {
                Vector3 nexPos = Path1_1[index];
                Vector3 dir = nexPos - transform.position;
                if (dir.magnitude > 0.1f)
                {
                    transform.position += dir.normalized * Time.deltaTime * 4;
                }
                else
                {
                    if (index == Path1_1.Count - 1)
                    {
                        transform.position = Path1_1[index];
                        IsSelected = false;
                        MoveEnd();
                    }
                    else
                    {
                        ++index;
                    }
                }
            }

            Color color = MT_Movie.GetColor("_Color");
            Vector4 deltal = Color.grey - color;
            if (deltal.magnitude < 0.1f)
            {
                MT_Movie.SetColor("_Color", Color.grey);
                SR.color = Color.grey;
            }
            else
            {
                deltal = deltal.normalized * 4;
                color = new Color(color.r + deltal.x * Time.deltaTime, color.g + deltal.y * Time.deltaTime, color.b + deltal.z * Time.deltaTime, 1);
                MT_Movie.SetColor("_Color", color);
                SR.color = color;
            }
           
        }
        else
        {
            if (IsLeft)
            {
                Vector3 nextPos = Path1_0[index];
                Vector3 dir = nextPos - transform.position;
                if (dir.magnitude > 0.1f)
                {
                    transform.position += dir.normalized * Time.deltaTime * 4;
                }
                else
                {
                    if (index == Path1_0.Count - 1)
                    {
                        transform.position = Path1_0[index];
                        IsSelected = true;
                        MoveEnd();
                    }
                    else
                        ++index;
                }
            }
            else
            {
                Vector3 nexPos = Path0_1[index];
                Vector3 dir = nexPos - transform.position;
                if (dir.magnitude > 0.1f)
                {
                    transform.position += dir.normalized * Time.deltaTime * 4;
                }
                else
                {
                    if (index == Path0_1.Count - 1)
                    {
                        transform.position = Path0_1[index];
                        IsSelected = true;
                        MoveEnd();
                    }
                    else
                    {
                        ++index;
                    }
                }
            }

            Color color = MT_Movie.GetColor("_Color");
            Vector4 deltal = Color.white - color;
            if (deltal.magnitude < 0.1f)
            {
                MT_Movie.SetColor("_Color", Color.white);
                SR.color = Color.white;
            }
            else
            {
                deltal = deltal.normalized * 4;
                color = new Color(color.r + deltal.x * Time.deltaTime, color.g + deltal.y * Time.deltaTime, color.b + deltal.z * Time.deltaTime, 1);
                MT_Movie.SetColor("_Color", color);
                SR.color = color;
            }
           
        }
    }

    private void SetMaterialColor()
    {
        if (IsSelected)
        {
            MT_Movie.SetColor("_Color", Color.white);
            SR.color = Color.white;
        }
        else
        {
            MT_Movie.SetColor("_Color", Color.grey);
            SR.color = Color.grey;
        }
    }

    private void MoveEnd()
    {
        CanMove = false;
        if (IsSelected)
        {
            if (IsCity)
            {
                CityDes.SetActive(true);
                GorgeDes.SetActive(false);
            }
            else
            {
                CityDes.SetActive(false);
                GorgeDes.SetActive(true);
            }
            MT.Play();
            SelectEffect.SetActive(true);
        }
        else
        {
            MT.Pause();
            SelectEffect.SetActive(false);
        }
    }
}
