using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// 카메라 움직임 제외 이펙트 관련 함수
/// </summary>
public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance { get; private set; }
    private GameObject _player;
    
    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        _player = GameObject.FindWithTag("Player");
    }

    /// <summary>
    /// EffectSO 정보를 가지고 이펙트를 실행하는 함수
    /// </summary>
    /// <param name="effectSO">EffectSO</param>
    /// <param name="position">Effect Prefab을 생성하는 위치</param>
    /// <param name="effectPrefab">Effect Prefab</param>
    public void AttackEffect(EffectSO effectSO, Vector2 position, GameObject effectPrefab = null)
    {
        // Camera Shake
        if (effectSO.useCameraShake)
        {
            CameraManager.Instance.Shake(effectSO.shakeIntensity, effectSO.shakeDuration);
        }

        // Slow
        if (effectSO.useTimeSlow)
        {
            SlowMotionTask(effectSO.timeScale, effectSO.slowDuration).Forget();
        }

        // Effect
        if (effectPrefab != null)
        {
            SpawnEffectTask(effectPrefab, position, effectSO.effectDuration).Forget();
        }
            
        // Input
        if (effectSO.useBlockInput)
        {
            DisableInputTask(effectSO.blockInputDuration).Forget();
        }
        
    }
        
    public async UniTask SlowMotionTask(float timeScale, float duration)
    {
        //await UniTask.Delay(TimeSpan.FromSeconds(0.05f)); 
        
        Time.timeScale = timeScale;
        Time.fixedDeltaTime = 0.02f * timeScale; // 물리 업데이트도 조정
        Debug.Log("슬로우 시작");
        
        await UniTask.Delay(TimeSpan.FromSeconds(duration), ignoreTimeScale: true); // 실제 시간 기준
        
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        Debug.Log("슬로우 끝");
    }

    private async UniTask SpawnEffectTask(GameObject effectPrefab, Vector2 position, float duration)
    {
        effectPrefab.SetActive(true);
        effectPrefab.transform.position = position;
        
        await UniTask.Delay(TimeSpan.FromSeconds(duration)); // 실제 시간 기준
            
        effectPrefab.SetActive(false);
    }

    /// <summary>
    /// Player의 움직임을 막는 함수. HitStop 효과
    /// </summary>
    /// <param name="duration">지속 시간</param>
    private async UniTask DisableInputTask(float duration)
    {
        _player.GetComponent<PlayerController>().OnDisableMove();
        
        await UniTask.Delay(TimeSpan.FromSeconds(duration));
        
        _player.GetComponent<PlayerController>().OnEnableMove();
    }
}
