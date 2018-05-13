/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   Around.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/9/5 15:06:39
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;
using DG.Tweening;
using DG.Tweening.Core;

public class Around0 : MonoBehaviour
{
    public bool Down;
    private Transform Center;
    private Camera camera;
    // Use this for initialization
    void Start()
    {
        Center = transform.parent;
        camera = GetComponent<Camera>();
        camera.enabled = false;
    }

    private bool enter;
    private float last;
    private float angle;
    // Update is called once per frame
    void Update()
    {
        if (ioo.gameMode.Player == null || ioo.gameMode.Player.PathType != Player.E_Path.Game)
            return;

        if (enter)
            return;

        enter = true;

        camera.enabled = true;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(Center.DORotate(new Vector3(Center.localEulerAngles.x, Center.localEulerAngles.y + 360, Center.localEulerAngles.z), 6, RotateMode.FastBeyond360));
        sequence.Join(Center.DOMoveY(Center.position.y - 40, 6));
        sequence.OnComplete(OnEnd);

        //if (angle < 360)
        //{
        //    angle += Time.deltaTime;
        //    Center.RotateAroundLocal(Vector3.up, Time.deltaTime);
        //    if (Down)
        //    {
        //        Center.position -= Time.deltaTime * 10 * Vector3.up;
        //    }
        //    transform.LookAt(Center.position);
        //}
        //else
        //{
        //    transform.position += transform.forward * Time.deltaTime;
        //}

        last += Time.deltaTime;
        if (last > 12)
        {
            camera.enabled = false;
        }
    }

    private void OnEnd()
    {
        Center.DOMoveZ(Center.position.z + 200, 4).OnComplete(ToGame);
    }

    private void ToGame()
    {
        camera.enabled = false;
    }
}
