using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AbilitySequenceSO")]
public class AbilitySequenceSO : ScriptableObject
{
    [System.Serializable]
    public struct AnimationSq
    {
        public AnimationClip source;
        public float delay;
    }
    
    [System.Serializable]
    public struct SoundSq
    {
        public AudioClip source;
        public float delay;
    }
    
    [System.Serializable]
    public struct EffectSq
    {
        public GameObject source;
        public Vector2 spawnPosition;
        public float delay;
    }
    
    [System.Serializable]
    public struct SlowSq
    {
        public float targetTimeScale;
        public float duration;
        public float delay;
    }
    
    [System.Serializable]
    public struct CameraSq
    {
        public CameraTask cameraTask;
        public float delay;
    }
    
    // + 스킬 연출에 추가되어야 할 것
    // 카메라 무빙?
    // 애니메이션, 이펙트 애니메이션 분리?
    // 스킬 연출에 필요한 것
    // 애니메이션, 이펙트, 사운드, 슬로우, 카메라 무빙
    
    [Header("Animation")]
    public List<AnimationSq> animations;
    
    [Header("Sound")]
    public List<SoundSq> sounds;

    [Header("Effect")]
    public List<EffectSq> effects;

    [Header("Slow")] 
    public List<SlowSq> slows;
    
    [Header("Camera Movement")]
    public List<CameraSq> cameras;
}
