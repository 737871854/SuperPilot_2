using UnityEngine;
using System.Collections;
using DG.Tweening;
using DG.Tweening.Core;
using Need.Mx;

public class BossCameraBehaviour : MonoBehaviour {

    public Transform Tran0;
    public Transform Tran1;

    private Transform TargetTran0;
    private Transform TargetTran1;
    private bool LookAtTarget;

    void OnEnable()
    {
        LookAtTarget = false;
        ioo.gameMode.Player.Pause = true;

        transform.position = Tran0.position;
        transform.localEulerAngles = Tran0.localEulerAngles;

        TargetTran0 = GameObject.FindGameObjectWithTag("DragonCamera0").transform;
        TargetTran1 = GameObject.FindGameObjectWithTag("DragonCamera1").transform;

        Sequence sequence   = DOTween.Sequence();
        sequence.Append(transform.DOMove(TargetTran0.position, 1));
        sequence.Join(transform.DORotate(TargetTran0.localEulerAngles, 1));
        sequence.OnComplete(OnStep0End);
        EventDispatcher.TriggerEvent(EventDefine.Event_Gorge_Boss_Fly);
    }

    private void OnStep0End()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(TargetTran1.position, 0.5f));
        sequence.Join(transform.DORotate(TargetTran1.localEulerAngles, 0.5f));

        sequence.OnComplete(OnStep1End);
    }

    private void OnStep1End()
    {
        LookAtTarget = true;

        DG.Tweening.ShortcutExtensions.DOShakePosition(transform, 1.5f, Vector3.one * 2, 30, 1, true);
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(2.5f);
        sequence.Append(transform.DOMove(Tran1.position, 1.5f));
        sequence.OnComplete(OnEnd);
    }

    private void OnEnd()
    {
        ioo.gameMode.Player.Pause = false;
        EventDispatcher.TriggerEvent(EventDefine.Event_Can_Attack_Gorge_Boss);
    }

    void OnDisable()
    {

    }
	
	// Update is called once per frame
	void Update () 
    {
        if (!LookAtTarget)
            return;

        Quaternion toRotation = Quaternion.LookRotation(ioo.gameMode.Boss.ViewPoint.position - transform.position);

        transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, Time.deltaTime * 20);
	}
}
