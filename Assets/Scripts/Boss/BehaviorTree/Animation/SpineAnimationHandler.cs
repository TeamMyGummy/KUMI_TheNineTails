using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Serialization;

//State 변환에 따라 SpineAnimation 실행
[System.Serializable]
public struct BlendDefinition
{
    [SerializeField] [SpineAnimation] public string from;
    [SerializeField] [SpineAnimation] public string to;
    [SerializeField] public float blendingTime;
}
public class SpineAnimationHandler : MonoBehaviour
{
    private SkeletonAnimation skeletonAnimation;
    void Awake()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
    }
    
    void Start()
    {
        skeletonAnimation.AnimationState.Data.DefaultMix = 0.2f;
    }

    public void StopAnimation()
    {
        Spine.TrackEntry currentTrackEntry = skeletonAnimation.AnimationState.GetCurrent(0);

        if (currentTrackEntry != null)
        {
            currentTrackEntry.TimeScale = 0f;
        }
    }

    //상태에 따라 애니메이션을 실행하는 함수
    public TrackEntry AddAnimation(string animationName, bool loop, float delay)
    {
        return skeletonAnimation.AnimationState.AddAnimation(0, animationName, loop, delay);
    }

    public TrackEntry SetAnimation(string animationName, bool loop)
    {
        DebugUtil.Log(animationName + " 가 실행중입니다. ");
        return skeletonAnimation.AnimationState.SetAnimation(0, animationName, loop);
    }
}
