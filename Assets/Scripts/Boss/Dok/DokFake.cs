using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorTree;
using BehaviorTree.Leaf;
using Cysharp.Threading.Tasks;
using GameAbilitySystem;
using Spine;
using Spine.Unity;
using UnityEngine;
using Random = UnityEngine.Random;

public class DokFake : FunctionHandler
{
    private AbilitySystem _ASC;
    [SerializeField] private SpineAnimationHandler skeletonAnimation;
    [SerializeField] private TeleportHandler _teleportHandler;
    [SerializeField] private FlipHandler _flipHandler;
    [SerializeField] private GameObject visual;

    //상태가 존재함. 한 인스턴스에 대해 동시에 두 함수 실행 불가능.
    private bool _isRunning = false;
    private bool _isAborted = false;
    
    void Awake()
    {
        _teleportHandler = GetComponent<TeleportHandler>();
        _flipHandler = GetComponent<FlipHandler>();
        _ASC = GetComponent<IAbilitySystem>().asc;
        FunctionMapping.Add("Idle", Idle);
        FunctionMapping.Add("딱밤", PipeAttack);
        FunctionMapping.Add("파도_오", RightHammerAttack);
        FunctionMapping.Add("파도_왼", LeftHammerAttack);
        FunctionMapping.Add("Complete", Complete);
    }
    
    public override NodeState OnStartAction()
    {
        Debug.Log(currentFunction);
        FunctionMapping[currentFunction].Invoke();
        _isRunning = true;
        if (!waitForEnd) return NodeState.Success;
        return NodeState.Running;
    }
    
    protected override NodeState OnUpdateAction()
    {
        if (_isRunning && !visual.activeSelf) return NodeState.Abort;
        if (!_isRunning) return NodeState.Success;
        return NodeState.Running;
    }

    private void Idle()
    {
        visual.SetActive(true);
        skeletonAnimation.SetAnimation("idle_fly", true);
        _isRunning = false;
    }

    private void PipeAttack()
    {
        int isRight = Random.Range(0, 2);
        if (isRight == 1)
        {
            _flipHandler.SetDirectionState(false);
            _teleportHandler.SetMovementPoint(new Vector3(-1f, 0f, 0f), EPositionType.TargetOffset, EPositionType.TargetOffset);
        }
        else
        {
            _flipHandler.SetDirectionState(true);
            _teleportHandler.SetMovementPoint(new Vector3(1f, 0f, 0f), EPositionType.TargetOffset, EPositionType.TargetOffset);
        }
        
        _teleportHandler.OnStartAction();
        _flipHandler.OnStartAction();
        
        visual.SetActive(true);
        TrackEntry trackEntry = skeletonAnimation.SetAnimation("Pipe attack_fake", false);
        trackEntry.Complete += CompleteBehavior;
        skeletonAnimation.AddAnimation("Start_idle", true, 0f);
    }

    private void RightHammerAttack()
    {
        int x = Random.Range(-1, -7);
        _teleportHandler.SetMovementPoint(new Vector3(x, -4f, 0f), EPositionType.TargetOffset, EPositionType.CameraOffset);
        _flipHandler.SetDirectionState(false);

        HammerAttack();
    }
    
    private void LeftHammerAttack()
    {
        int x = Random.Range(1, 7);
        _teleportHandler.SetMovementPoint(new Vector3(x, -4f, 0f), EPositionType.TargetOffset, EPositionType.CameraOffset);
        _flipHandler.SetDirectionState(true);
        
        HammerAttack();
    }
    
    private void HammerAttack()
    {
        _teleportHandler.OnStartAction();
        _flipHandler.OnStartAction();
        
        visual.SetActive(true);
        TrackEntry trackEntry = skeletonAnimation.SetAnimation("Hammer attack_fake", false);
        trackEntry.Complete += CompleteBehavior;
        skeletonAnimation.AddAnimation("Start_idle", true, 0f);
    }
    
    public void OnTriggerEnter2D(Collider2D other)
    {
        Complete();
        _ASC.Attributes["Count"].Modify(-1, EModOperation.Additive);
    }

    private async void CompleteBehavior(TrackEntry trackEntry)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(completeDelay));
        Complete();
    }

    private void Complete()
    {
        visual.SetActive(false);
        _isRunning = false;
    }
}
