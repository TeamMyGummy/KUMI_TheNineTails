using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameAbilitySystem;
using UnityEngine;
using Random = UnityEngine.Random;

//유연하지 않은 지점(Ex. Offset 설정, 등...)이 존재함. 변경해야 할 일이 생길 경우 _so 자체를 새로 파서 수정할 것
//(강승연 호출)
public class FireVariousProjectile : GameplayAbility<FireVariousProjectileSO>, IBossAbility
{
    protected FireVariousProjectileSO _so;

    private CancellationTokenSource _cts; // UniTask 취소를 관리하는 객체

    public override void InitAbility(GameObject actor, AbilitySystem asc, GameplayAbilitySO abilitySo)
    {
        base.InitAbility(actor, asc, abilitySo);
        _so = abilitySo as FireVariousProjectileSO;
    }

    protected override void Activate()
    {
        // CancellationTokenSource를 새로 생성하고, 이 토큰을 비동기 메서드로 넘겨줍니다.
        _cts = new CancellationTokenSource();
        StartProjectileSequence(_cts.Token).Forget();
    }

    /// <summary>
    /// UniTask 시퀀스를 시작하고, 정상 종료 또는 취소 시 후처리를 담당합니다.
    /// </summary>
    private async UniTaskVoid StartProjectileSequence(CancellationToken token)
    {
        try
        {
            // FireRoutine 작업이 끝날 때까지 기다립니다.
            await FireRoutine(token);
            
            // 루프가 중간에 취소되지 않고 끝까지 실행됐다면 OnNormalEnd를 호출합니다.
            OnNormalEnd?.Invoke();
        }
        catch (OperationCanceledException)
        {
            // CancelAbility가 호출되면 여기에 OperationCanceledException이 발생합니다.
            // 의도된 취소이므로, 디버그 로그만 남깁니다.
            Debug.Log("Ability sequence was cancelled.");
        }
    }

    /// <summary>
    /// 주석에 명시된 내용대로, 정해진 시간 동안 투사체를 발사하는 코루틴입니다.
    /// </summary>
    private async UniTask FireRoutine(CancellationToken token)
    {
        Vector2 direction = _so.direction.normalized;
        // 각 투사체 사이의 지연 시간을 계산합니다.
        float delayPerShot = _so.totalDuration / _so.projectileCount;

        for (int i = 0; i < _so.projectileCount; i++)
        {
            // 투사체를 생성하고 발사합니다.
            Projectile projectile = ResourcesManager.Instance.Instantiate(_so.projectile.gameObject).GetComponent<Projectile>();
            projectile.SetOffset(new Vector3(Random.Range(-_so.delta.x, _so.delta.x), _so.delta.y, 0f));
            projectile.FireProjectile(Actor, direction);
            
            // 다음 투사체를 발사하기 전까지 계산된 시간만큼 대기합니다.
            // 대기 중에 토큰이 취소되면 OperationCanceledException이 발생하며 루프가 중단됩니다.
            await UniTask.Delay(TimeSpan.FromSeconds(delayPerShot), cancellationToken: token);
        }
    }

    public Action OnNormalEnd { get; set; } //정상 종료 시 실행

    public void CancelAbility()
    {
        // CancellationTokenSource에 취소를 요청합니다.
        // 실행 중인 UniTask.Delay가 즉시 예외를 발생시키고 중단됩니다.
        _cts?.Cancel();
    }
    
    // 이 스크립트가 파괴될 때 CancellationTokenSource를 정리하여 메모리 누수를 방지합니다.
    private void OnDestroy()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }
}