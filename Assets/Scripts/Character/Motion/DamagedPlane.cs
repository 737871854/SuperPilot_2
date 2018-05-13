/*
 * Copyright (广州纷享游艺设备有限公司-研发视频组) 
 * 
 * 文件名称：   DamagedPlane.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Pancake 2017/7/18 10:42:21
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;

public class DamagedPlane : MonoBehaviour
{
    public bool ExploseInTarget;
    public float CheckDistance = 0.2f;
    public float PercentOffset = 0.004f;
    public bool FixedBorn = false;
    public float Speed = 100;
    private Vector3 Direction;
    private Vector3 TargetPos;

    private float Life;

    private Vector3 StartPos;
    private Vector3 UpDir;
    void Awake()
    {
        StartPos = transform.position;
    }

    // Use this for initialization
    void OnEnable()
    {
        if (!FixedBorn)
        {
            transform.position = PathManager.Instance.PathInfo1.GetPos(ioo.gameMode.Player.Percent + 0.01f) + Vector3.up * 20;
        }
        else
        {
            transform.position = StartPos;
        }

        Life = 1;

        // 指定
        float percent = ioo.gameMode.Player.Percent + PercentOffset; ;
        TargetPos = PathManager.Instance.PathInfo1.GetPos(percent);
        UpDir = PathManager.Instance.PathInfo1.GetUpPos(percent) - TargetPos;
        UpDir.Normalize();
        Vector3 targetRight = PathManager.Instance.PathInfo1.GetRightPos(percent);

        Vector3 offset = ioo.gameMode.Player.Offset;
        TargetPos += offset;
        targetRight += offset;

        //float rand = UnityEngine.Random.Range(-0.5f, 0.5f);
        Vector3 rightDir = targetRight - TargetPos;
        rightDir.Normalize();
        //TargetPos += rightDir * rand;
        Direction = TargetPos - transform.position;
        Direction.Normalize();

        //ioo.audioManager.PlayMusicOnObj("Music_Damage_Plane_Coming", gameObject);
    }

    void OnDisable()
    {
        //ioo.audioManager.StopBackMusic("Music_Damage_Plane_Coming");
        EffectManager.Instance.Spawn(EffectName.Effect_Enemy_2_baoz, transform.position);
    }

    void Destroy()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(GameTag.Player))
        {
            ToDestroy();
            return;
        }

        if (!other.tag.Equals(GameTag.Weapon))
            return;

        WeaponBehaviour wb = other.GetComponent<WeaponBehaviour>();

        if (wb.Owner != GameTag.Player)
            return;

        OnDamage(wb.DamageValue);
        WeaponManager.Instance.AddDespawnWeapon(wb);
    }

    private void OnDamage(float damage)
    {
        Life += damage;
        if (Life <= 0)
        {
            Life = 0;
            ToDestroy();
        }
    }

    private void ToDestroy()
    {
        gameObject.SetActive(false);
        ioo.audioManager.PlaySoundOnPoint("SFX_Sound_Enemy_Plane_Destroy", transform.position);
        EffectManager.Instance.Spawn(EffectName.Effect_diji_Plane_baoz, transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Direction * Time.deltaTime * Speed;

        Vector3 nowDir = TargetPos - transform.position;
        if (Vector3.Angle(Direction, nowDir) > 150 && nowDir.magnitude > 10)
        {
            gameObject.SetActive(false);
        }

        if (!ExploseInTarget)
            return;

        if (Vector3.Distance(TargetPos, transform.position) < CheckDistance)
        {
            gameObject.SetActive(false);
            EffectManager.Instance.Spawn(EffectName.Effect_Aerolite_baoz, TargetPos);
        }
    }
}
