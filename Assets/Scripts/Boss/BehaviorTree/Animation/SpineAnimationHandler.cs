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
    
    [Header("Blend Settings")]
    [SerializeField] private float defaultMixDuration = 0.2f;
    [SerializeField] private List<BlendDefinition> blendDefinitions = new List<BlendDefinition>();
    
    void Awake()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
    }
    
    void Start()
    {
        SetupBlending();
    }

    private void SetupBlending()
    {
        var stateData = skeletonAnimation.AnimationState.Data;
        stateData.DefaultMix = defaultMixDuration;
        
        // 정의된 블렌딩 규칙 적용
        foreach (var blend in blendDefinitions)
        {
            stateData.SetMix(blend.from, blend.to, blend.blendingTime);
        }
    }

    public void StopAnimation()
    {
        Spine.TrackEntry currentTrackEntry = skeletonAnimation.AnimationState.GetCurrent(0);

        if (currentTrackEntry != null)
        {
            currentTrackEntry.TimeScale = 0f;
        }
    }

    public TrackEntry AddAnimation(string animationName, bool loop, float delay)
    {
        return skeletonAnimation.AnimationState.AddAnimation(0, animationName, loop, delay);
    }

    public TrackEntry SetAnimation(string animationName, bool loop)
    {
        return skeletonAnimation.AnimationState.SetAnimation(0, animationName, loop);
    }
    
    public TrackEntry SetAnimation(string animationName, bool loop, float customMix)
    {
        var trackEntry = skeletonAnimation.AnimationState.SetAnimation(0, animationName, loop);
        
        if (customMix >= 0)
        {
            trackEntry.MixDuration = customMix;
        }
        
        return trackEntry;
    }
    
    public void SetAnimation(string animationName)
    {
        skeletonAnimation.AnimationState.SetAnimation(0, animationName, false);
    }
}