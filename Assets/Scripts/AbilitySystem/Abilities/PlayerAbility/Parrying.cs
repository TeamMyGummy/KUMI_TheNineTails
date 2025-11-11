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

    public override void InitAbility(GameObject actor, AbilitySystem asc, GameplayAbilitySO abilitySo)
    {
        base.InitAbility(actor, asc, abilitySo);
        _so = abilitySo as ParryingSO;
        
        _hitbox = ResourcesManager.Instance.Instantiate(_so.Hitbox, actor.transform);
        _hitbox.SetActive(false);
        Transform canvasTransform = GameObject.FindWithTag("BaseCanvas").transform;
        _gaugeBar = ResourcesManager.Instance.Instantiate(_so.GaugeBar.gameObject, canvasTransform);
        _gaugeImage = _gaugeBar.transform.Find("hp_bar").GetComponent<Image>();
        _gaugeBar.SetActive(false);

        _playerController = actor.GetComponent<PlayerController>();
        _playerController.OnParryingCanceled -= Canceled;
        _playerController.OnParryingCanceled += Canceled;
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
        float interpolation = Mathf.Approximately(_playerController.Direction.x, 1) ? 0f : 0.5f; 
        _hitbox.transform.position = Actor.transform.position + new Vector3(_playerController.Direction.x * _so.HitboxOffset.x + interpolation, _so.HitboxOffset.y, 0f);
    }

    private void SetGaugeBar() //몬스터 hp바와 동일하게 수정했음
    {
        _gaugeImage.fillAmount = _currentGauge / MaxGauge;
        RectTransform barRT = _gaugeBar.transform as RectTransform;
        Canvas canvas = _gaugeBar.GetComponentInParent<Canvas>();
        
        if (canvas == null) return; 

        RectTransform canvasRectTransform = canvas.transform as RectTransform;
        Camera canvasCamera = canvas.worldCamera; 
        Camera targetCamera = Camera.main;

        foreach (var c in GameObject.FindGameObjectsWithTag("MainCamera"))
        {
            Camera potentialCam = c.GetComponent<Camera>();
            if (potentialCam != null && c.name == "BossCamera")
            {
                targetCamera = potentialCam;
                break;
            }
        }
    
        if (targetCamera == null) return;

        Vector3 worldPos = Actor.transform.position + Vector3.up * 2f; 
        Vector2 screenPoint = targetCamera.WorldToScreenPoint(worldPos);

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform,
            screenPoint,
            canvasCamera, 
            out localPoint
        );
        barRT.anchoredPosition = localPoint;
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
}