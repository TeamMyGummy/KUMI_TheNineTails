using System;
using System.Collections;
using GameAbilitySystem;
using Spine.Unity;
using UnityEngine;
using R3;

[RequireComponent(typeof(SkeletonAnimation))]
public class SpineMaterialFlash : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] private Shader flashShader;   // Inspector에 Custom/WhiteFlash 할당
    [SerializeField] private float flashDuration = 0.15f;

    private SkeletonAnimation _skeletonAnimation;

    // ★ Spine이 실제로 사용하는 원본 Atlas Material
    private Material _originalMaterial;
    private Material _flashMaterial;

    private Coroutine _flashRoutine;

    // HP 감지
    private IDisposable _hpSubscription;
    private float _previousHp;

    private void Awake()
    {
        _skeletonAnimation = GetComponent<SkeletonAnimation>();
    }

    private void Start()
    {
        PrepareFlashMaterial();
        BindAbilitySystem();
    }

    private void OnDisable()
    {
        _hpSubscription?.Dispose();
        _hpSubscription = null;
    }

    // -------------------------
    // AbilitySystem 바인딩
    // -------------------------
    private void BindAbilitySystem()
    {
        var abilityInterface =
            GetComponent<IAbilitySystem>() ??
            GetComponentInChildren<IAbilitySystem>();

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
                    Flash();

                _previousHp = currentHp;
            });
        }
    }

    // -------------------------
    // Flash Material 준비 (★ 핵심 수정 포인트)
    // -------------------------
    private void PrepareFlashMaterial()
    {
        if (_skeletonAnimation == null)
            return;

        if (flashShader == null)
        {
            Debug.LogError($"[SpineFlash] {name}: Flash Shader가 할당되지 않았습니다.");
            return;
        }

        // ★ Spine Atlas에서 원본 Material 추출
        var skeletonData = _skeletonAnimation.Skeleton.Data;
        var atlasAssets = _skeletonAnimation.SkeletonDataAsset.atlasAssets;

        if (atlasAssets == null || atlasAssets.Length == 0)
        {
            Debug.LogError($"[SpineFlash] {name}: AtlasAsset을 찾을 수 없습니다.");
            return;
        }

        _originalMaterial = atlasAssets[0].PrimaryMaterial;

        if (_originalMaterial == null)
        {
            Debug.LogError($"[SpineFlash] {name}: Original Material null");
            return;
        }

        _flashMaterial = new Material(flashShader);

        if (_originalMaterial.HasProperty("_MainTex"))
            _flashMaterial.mainTexture = _originalMaterial.mainTexture;

        if (_flashMaterial.HasProperty("_FlashColor"))
            _flashMaterial.SetColor("_FlashColor", Color.white);
        else if (_flashMaterial.HasProperty("_Color"))
            _flashMaterial.SetColor("_Color", Color.white);
    }

    // -------------------------
    // 외부 호출
    // -------------------------
    public void Flash()
    {
        if (_flashRoutine != null)
            StopCoroutine(_flashRoutine);

        _flashRoutine = StartCoroutine(FlashRoutine(flashDuration));
    }

    // -------------------------
    // Flash 코루틴
    // -------------------------
    private IEnumerator FlashRoutine(float duration)
    {
        if (_originalMaterial == null || _flashMaterial == null)
            yield break;

        var overrideDict = _skeletonAnimation.CustomMaterialOverride;

        overrideDict[_originalMaterial] = _flashMaterial;
        yield return new WaitForSeconds(duration);
        overrideDict.Remove(_originalMaterial);

        _flashRoutine = null;
    }
}