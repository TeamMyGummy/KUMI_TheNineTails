using Cysharp.Threading.Tasks;
using System;
using System.Threading;

public class MonsterDoubleAttack : MonsterAttack
{
    private bool _isAttacking;
    private CancellationTokenSource _attackCts;
    protected override async void Activate()
    {
        // 0) 중복 방지: 이미 공격 중이면 무시
        if (_isAttacking) return;

        // 1) 이전 공격 취소(혹시 남아있다면)
        _attackCts?.Cancel();
        _attackCts?.Dispose();
        _attackCts = new CancellationTokenSource();

        // 2) 파괴/디스에이블 연동 + 수동취소를 합친 토큰
        var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
            _monster.GetCancellationTokenOnDestroy(),
            _attackCts.Token
        );
        var tk = linkedCts.Token;

        _isAttacking = true;

        
        base.Activate();

        _movement?.SetPaused(true); // 공격준비-공격-공격후 내내 정지

        try
        {
            // 공격 전 딜레이 
            if (_attackData.PreAttackDelay > 0f)
                await UniTask.Delay(
                    TimeSpan.FromSeconds(_attackData.PreAttackDelay),
                    delayType: DelayType.UnscaledDeltaTime,
                    cancellationToken: tk
                );

            // 공격(1타)
            Attack();
            if (_attackData.ActiveTime > 0f)
                await UniTask.Delay(
                    TimeSpan.FromSeconds(_attackData.ActiveTime),
                    delayType: DelayType.UnscaledDeltaTime,
                    cancellationToken: tk
                );
            
            // 연속공격 텀
            if (_attackData.ActiveTime > 0f)
                await UniTask.Delay(
                    TimeSpan.FromSeconds(_attackData.BetweenAttackDelay),
                    delayType: DelayType.UnscaledDeltaTime,
                    cancellationToken: tk
                );
            
            // 공격(2타)
            Attack();
            if (_attackData.ActiveTime > 0f)
                await UniTask.Delay(
                    TimeSpan.FromSeconds(_attackData.ActiveTime),
                    delayType: DelayType.UnscaledDeltaTime,
                    cancellationToken: tk
                );

            // 공격 후 딜레이 
            if (_attackData.PostAttackDelay > 0f)
                await UniTask.Delay(
                    TimeSpan.FromSeconds(_attackData.PostAttackDelay),
                    delayType: DelayType.UnscaledDeltaTime,
                    cancellationToken: tk
                );
        }
        catch (OperationCanceledException)
        {
            // (예외처리) 몬스터 사망 or 공격 취소될 경우 --> 무시
        }
        finally
        {
            _movement?.SetPaused(false); // 정지 해제 
            _isAttacking = false;
            linkedCts.Dispose();
        }
    }
}

