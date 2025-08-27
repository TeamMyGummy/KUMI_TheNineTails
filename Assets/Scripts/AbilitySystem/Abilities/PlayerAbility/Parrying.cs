using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using GameAbilitySystem;
using TMPro;
using TMPro.SpriteAssetUtilities;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class Parrying : GameplayAbility<ParryingSO>
{
    private ParryingSO _so;

    private GameObject _hitbox;
    private GameObject _gaugeBar;
    private Image _gaugeImage;

    private const float MaxGauge = 1f;
    private float _currentGauge = MaxGauge;
    
    private CancellationTokenSource _reduceGaugeCts;
    private CancellationTokenSource _chargeGaugeCts;

    private PlayerController _playerController;
    
    private AbilitySequenceSO _sequenceSO;
    private AbilityTask _task;

    public override void InitAbility(GameObject actor, AbilitySystem asc, GameplayAbilitySO abilitySo)
    {
        base.InitAbility(actor, asc, abilitySo);
        _so = abilitySo as ParryingSO;
        
        _hitbox = ResourcesManager.Instance.Instantiate(_so.Hitbox, actor.transform);
        _hitbox.SetActive(false);
        _gaugeBar = ResourcesManager.Instance.Instantiate(_so.GaugeBar.gameObject, Object.FindFirstObjectByType<Canvas>().gameObject.transform);
        _gaugeImage = _gaugeBar.transform.Find("hp_bar").GetComponent<Image>();
        _gaugeBar.SetActive(false);

        _playerController = actor.GetComponent<PlayerController>();
        _playerController.OnParryingCanceled -= Canceled;
        _playerController.OnParryingCanceled += Canceled;
        
        // Task
        _sequenceSO = abilitySo.skillSequence;
        _task = new AbilityTask(actor, _sequenceSO);
    }

    protected override void Activate()
    {
        StartParrying();
    }

    public void Canceled()
    {
        CancelReduce();
    } 

    private void StartParrying()
    {
        Asc.TagContainer.Add(GameplayTags.BlockRunningAbility);
        SetHitbox();
        _gaugeBar.SetActive(true);
        _playerController.OnDisableMove();

        _reduceGaugeCts = new CancellationTokenSource();
        ReduceGauge(_reduceGaugeCts.Token).Forget();
    }

    private async UniTaskVoid ReduceGauge(CancellationToken token)
    {
        CancelCharge();
        float duration = _so.FullGaugeDuration;
        float elapsed = (MaxGauge - (_currentGauge / MaxGauge)) * duration;

        try
        {
            while (elapsed < duration)
            {
                token.ThrowIfCancellationRequested();

                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                _currentGauge = Mathf.Lerp(1f, 0f, t);

                SetGaugeBar();

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }

            _currentGauge = 0f;
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            _hitbox.SetActive(false);
            Asc.TagContainer.Remove(GameplayTags.BlockRunningAbility);
            _playerController.OnEnableMove();
            
            _chargeGaugeCts = new CancellationTokenSource();
            ChargeDamage(_chargeGaugeCts.Token).Forget();
        }
    }
    
    private async UniTaskVoid ChargeDamage(CancellationToken token)
    {
        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_so.PauseDuration), cancellationToken: token);

            float duration = _so.RecoveryDuration;
            float elapsed = (_currentGauge / MaxGauge) * duration;
            
            while (elapsed < duration)
            {
                token.ThrowIfCancellationRequested();

                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                _currentGauge = Mathf.Lerp(0f, 1f, t);

                SetGaugeBar();

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }

            _currentGauge = 1f;

            await UniTask.Delay(TimeSpan.FromSeconds(_so.EndDuration), cancellationToken: token);
            _gaugeBar.SetActive(false);
        }
        catch (OperationCanceledException)
        {
        }
    }

    private void SetHitbox()
    {
        _hitbox.SetActive(true);
        _hitbox.GetComponent<ParryingHitbox>().LiverExtraction -= OnEnableLiverExtraction;
        _hitbox.GetComponent<ParryingHitbox>().LiverExtraction += OnEnableLiverExtraction;
        _hitbox.transform.position = Actor.transform.position + new Vector3(_playerController.Direction.x * 0.7f, 0.7f, 0f);
    }

    private void SetGaugeBar()
    {
        _gaugeImage.fillAmount = _currentGauge / MaxGauge;

        RectTransform barRT = _gaugeBar.transform as RectTransform;

        // Actor 머리 위 월드 위치
        Vector3 worldPos = Actor.transform.position + Vector3.up * 2.5f;

        Camera camera = Camera.main;

        foreach (var c in GameObject.FindGameObjectsWithTag("MainCamera"))
        {
            if (c.name == "BossCamera")
                camera = c.GetComponent<Camera>();
        }
        // 스크린 좌표로 변환
        //이거 개선하기; 카메라 시스템을 만들어야 할 필요성을 느낌;;;;
        Vector3 screenPos = camera.WorldToScreenPoint(worldPos);

        // UI Canvas에 직접 위치 지정 (Overlay 모드 기준)
        barRT.position = screenPos;
    }
    
    private void CancelReduce()
    {
        _reduceGaugeCts?.Cancel();
        _reduceGaugeCts?.Dispose();
        _reduceGaugeCts = null;
    }
    private void CancelCharge()
    {
        _chargeGaugeCts?.Cancel();
        _chargeGaugeCts?.Dispose();
        _chargeGaugeCts = null;
    }

    private void OnEnableLiverExtraction()
    {
        _playerController.OnEnableLiverExtraction();
        Debug.Log("간 빼기 스킬 슉퐉쾅");
        // 슬로우 1초

        AbilitySequenceSO.SlowSq slow = new AbilitySequenceSO.SlowSq
        {
            targetTimeScale = 0.2f,
            duration = 1.0f,
            delay = 0.0f
        };
        _task.PlaySlow(slow).Play();
    }
}