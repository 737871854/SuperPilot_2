/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   BugAttack.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/8/29 9:08:52
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;

public class BugAttack : MonoBehaviour
{
    [System.NonSerialized]
    public bool Chase;

    [System.NonSerialized]
    public Transform ShootPoint;

    public Vector3 Offset;

    private Bugs Parent;

    private Vector3 OrgLocalPos;

    void Start()
    {
        Parent = transform.parent.GetComponent<Bugs>();
        ShootPoint = transform.Find("ShootPoint").transform;
        OrgLocalPos = transform.localPosition + Offset;
    }
  
    public void OnHurt(float damage)
    {
        Parent.OnHurt(damage);
    }

    public void OnDead()
    {
        EffectManager.Instance.Spawn(EffectName.Effect_mine_bomb, transform);
        ioo.audioManager.PlaySound2D("SFX_Sound_Trigger_Damage");
        gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.tag.Equals(GameTag.Player))
            return;

        Parent.OnExplosed();
     
    }

    // Update is called once per frame
    void Update()
    {
        if (Chase)
        {
            Vector3 dir = transform.localPosition - ioo.gameMode.Player.Offset;
            if (dir.magnitude > 0.1f)
            {
                transform.localPosition -= dir.normalized * 10 * Time.deltaTime;
            }
            else
            {
                transform.localPosition = ioo.gameMode.Player.Offset;
            }
        }
        else
        {
            Vector3 dir = transform.localPosition - OrgLocalPos;
            if (dir.magnitude > 0.1f)
            {
                transform.localPosition -= dir.normalized * 10 * Time.deltaTime;
            }
            else
            {
                transform.localPosition = OrgLocalPos;
            }
        }
    }
}
