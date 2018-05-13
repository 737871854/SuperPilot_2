/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   LoadingOp.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/8/15 10:38:53
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine.UI;

public class LoadingOp : MonoBehaviour
{
    public Animator Anim0;
    public Animator Anim1;

    private float Timer;

    public Image Fire;
    public Image Missile;

    public GameObject Effect0;
    public GameObject Effect1;

    public Material MT0;
    public Material MT1;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(DoAnimation());
    }

    void Destroy()
    {

    }

    IEnumerator DoAnimation()
    {
        yield return new WaitForSeconds(2.0f);
        Anim0.SetInteger("State", 1);
        yield return new WaitForSeconds(1.6f);
        Fire.gameObject.SetActive(true);
        Anim0.SetInteger("State", 2);
        Anim0.transform.DOLocalRotate(new Vector3(0, 310, 0), 0.2f);
        yield return new WaitForSeconds(2.2f);
        Missile.gameObject.SetActive(true);
        Anim0.transform.DOLocalRotate(new Vector3(0, 184, 0), 0.2f);
        Anim0.SetInteger("State", 3);
        yield return new WaitForSeconds(2.1f);
        Anim1.SetBool("State", true);
        Anim0.SetInteger("State", 4);
        Effect0.SetActive(false);
        Effect1.SetActive(true);
    }
}
