using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Serialization;

public class SpineAnimationHandler : MonoBehaviour
{
    private SkeletonAnimation skeletonAnimation;
    //상태명(string)과 애니메이션을 매핑한 자료구조
    
    void Awake()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
    }

    //상태에 따라 애니메이션을 실행하는 함수
    public void AddAnimation(string animationName, bool loop, float delay)
    {
        skeletonAnimation.AnimationState.AddAnimation(0, animationName, loop, delay);
    }

    public void SetAnimation(string animationName, bool loop)
    {
        skeletonAnimation.AnimationState.SetAnimation(0, animationName, loop);
    }
}
