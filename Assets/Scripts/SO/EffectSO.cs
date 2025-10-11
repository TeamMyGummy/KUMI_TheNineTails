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
    
    [Header("VFX")]
    public GameObject hitEffectPrefab;
    public float effectDuration;
    
    [Header("Block Movement")]
    public bool useBlockInput;
    public float blockInputDuration;
}
