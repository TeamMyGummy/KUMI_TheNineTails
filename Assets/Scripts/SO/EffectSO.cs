using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EffectSO")]
public class EffectSO : ScriptableObject
{
    [Header("Camera Shake")]
    public bool useCameraShake;
    public float shakeIntensity;
    public float shakeDuration;
    
    [Header("Time Slow")]
    public bool useTimeSlow;
    public float timeScale;
    public float slowDuration;

    [Header("SFX")] 
    public bool useSound;
    public SFXName sfxName;
    
    [Header("VFX")]
    public GameObject hitEffectPrefab;
    public float effectDuration;
    
    [Header("Block Movement")]
    public bool useBlockInput;
    public float blockInputDuration;
}
