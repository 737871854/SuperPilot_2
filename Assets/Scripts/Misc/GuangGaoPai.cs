/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   GuangGaoPai.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/8/4 10:05:04
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;

public class GuangGaoPai : MonoBehaviour
{
    public GameObject GuangGao;
    public Animation EffectAnim;
    // Use this for initialization
    void Start()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(GameTag.Player))
        {
            GuangGao.SetActive(false);
            EffectAnim.gameObject.SetActive(true);
            EffectAnim.CrossFade("Take 001");
            StartCoroutine(OnEndGuangGaoPai());
        }
    }

    IEnumerator OnEndGuangGaoPai()
    {
        yield return new WaitForSeconds(5);
        GuangGao.SetActive(true);
        EffectAnim.gameObject.SetActive(false);
    }
}
