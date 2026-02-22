using BehaviorTree;
using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Imoogi : MonoBehaviour
{
    public TriggerHandler handler;
    [SerializeField] private BoxCollider2D hitBox;

    public SkeletonAnimation skeletonAnimation;

    public void Start()
    {
        skeletonAnimation.AnimationState.Event += OnAttackEvent;
    }

    public void GetParried()
    {
        handler.OnTrigger();
    }

    public void OnAttackEvent(TrackEntry trackEntry, Spine.Event e)
    {
        // 이미 실행 중인 히트박스 루틴이 있다면 꼬이지 않게 정지 후 새로 시작
        StopCoroutine(nameof(HitBoxRoutine));
        StartCoroutine(nameof(HitBoxRoutine));
    }
    private IEnumerator HitBoxRoutine()
    {
        if (hitBox != null) hitBox.enabled = true;

        yield return new WaitForSeconds(0.2f);

        if (hitBox != null) hitBox.enabled = false;
    }
}
