using System;
using System.Collections;
using GameAbilitySystem;
using Spine.Unity;
using UnityEngine;
using R3;

public class SpineMaterialFlash : MonoBehaviour
{
    [Header("Visual Settings")]
    public string flashShaderName = "Custom/WhiteFlash"; 
    public float flashDuration = 0.15f;

    private SkeletonAnimation _skeletonAnimation;
    private MeshRenderer _meshRenderer;
    
    private Material _originalMaterial;
    private Material _flashMaterial;
    private Coroutine _flashRoutine;

    // HP 감지용 변수
    private IDisposable _hpSubscription;
    private float _previousHp;

    private void Start()
    {
        _skeletonAnimation = GetComponent<SkeletonAnimation>();
        _meshRenderer = GetComponent<MeshRenderer>();

        // 1. 반짝이는 재질(Material) 준비
        PrepareFlashMaterial();
        // 2. AbilitySystem 연결 (UI_BossHp 로직 응용)
        BindAbilitySystem();
    }

    private void OnDisable()
    {
        _hpSubscription?.Dispose();
    }

    private void BindAbilitySystem()
    {
        var abilityInterface = GetComponent<IAbilitySystem>() 
                               ?? GetComponentInChildren<IAbilitySystem>();

        if (abilityInterface == null || abilityInterface.asc == null) 
        {
            Debug.LogWarning($"[SpineFlash] {name}: IAbilitySystem을 찾을 수 없습니다.");
            return;
        }
        
        if (abilityInterface.asc.Attribute.Attributes.TryGetValue("HP", out var hpAttr))
        {
            _previousHp = hpAttr.CurrentValue.Value;
            _hpSubscription = hpAttr.CurrentValue.Subscribe(currentHp => 
            {
                if (currentHp < _previousHp)
                {
                    Flash();
                }
                _previousHp = currentHp;
            });
        }
    }
    private void PrepareFlashMaterial()
    {
        if (_meshRenderer == null) return;

        _originalMaterial = _meshRenderer.sharedMaterial; 
        Shader flashShader = Shader.Find(flashShaderName);
        if (flashShader == null) flashShader = Shader.Find("Spine/Skeleton"); 

        if (flashShader != null && _originalMaterial != null)
        {
            _flashMaterial = new Material(flashShader);
            if (_originalMaterial.HasProperty("_MainTex"))
                _flashMaterial.mainTexture = _originalMaterial.mainTexture;
            
            if (_flashMaterial.HasProperty("_FlashColor"))
                _flashMaterial.SetColor("_FlashColor", Color.white);
            else if (_flashMaterial.HasProperty("_Color"))
                _flashMaterial.SetColor("_Color", Color.white);
        }
    }

    public void Flash()
    {
        if (_flashRoutine != null) StopCoroutine(_flashRoutine);
        _flashRoutine = StartCoroutine(FlashRoutine(flashDuration));
    }

    private IEnumerator FlashRoutine(float duration)
    {
        if (_skeletonAnimation == null || _flashMaterial == null || _originalMaterial == null) yield break;

        _skeletonAnimation.CustomMaterialOverride[_originalMaterial] = _flashMaterial;
        yield return new WaitForSeconds(duration);
        _skeletonAnimation.CustomMaterialOverride.Remove(_originalMaterial);
        
        _flashRoutine = null;
    }
}