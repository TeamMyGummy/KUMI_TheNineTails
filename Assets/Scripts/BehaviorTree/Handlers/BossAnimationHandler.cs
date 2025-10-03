using System.Collections.Generic;
using BehaviorTree;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Serialization;

public class BossAnimationHandler : ActionHandler
{
    private SpineAnimationHandler animHandler;
    private string _currentAnimation;
    private EAnimationControl _howToPlay;
    private bool _loop;
    private float _delay = 0f;

    void Awake()
    {
        animHandler = GetComponent<SpineAnimationHandler>();
    }
    
    public void ChooseAnimation(string animation, EAnimationControl howToPlay, bool loop, float delay)
    {
        _currentAnimation = animation;
        _howToPlay = howToPlay;
        _loop = loop;
        _delay = delay;
    }
    //SpineAnimationHandler에 요청하여 실행
    protected override NodeState OnStartAction()
    {
        switch (_howToPlay)
        {
            case EAnimationControl.Add:
                animHandler.AddAnimation(_currentAnimation, _loop, _delay);
                break;
            case EAnimationControl.Set:
                animHandler.SetAnimation(_currentAnimation, _loop);
                break;
            case EAnimationControl.Stop:
                animHandler.StopAnimation();
                break;
            default:
                DebugUtil.AssertLog();
                break;
        }
        return NodeState.Success;
    }

    protected override NodeState OnUpdateAction()
    {
        DebugUtil.AssertLog();
        return NodeState.Success;
    }
}
