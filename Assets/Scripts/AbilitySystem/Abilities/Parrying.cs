using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
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
        _gaugeBar = ResourcesManager.Instance.Instantiate(_so.GaugeBar.gameObject, Object.FindFirstObjectByType<Canvas>().gameObject.transform);
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
        _hitbox.transform.position = Actor.transform.position + new Vector3(_playerController.direction.x * 0.7f, 0.7f, 0f);
    }

    private void SetGaugeBar()
    {
        _gaugeImage.fillAmount = _currentGauge / 1f; 
        _gaugeBar.transform.position = Camera.main.WorldToScreenPoint(Actor.transform.position + new Vector3(0f, 1.5f, 0f));
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