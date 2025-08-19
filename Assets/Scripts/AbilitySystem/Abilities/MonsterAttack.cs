using GameAbilitySystem;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

public class MonsterAttack : BlockAbility<MonsterAttackSO>
{
    protected GameObject _actor;
    protected MonsterMovement _movement;
    protected Monster _monster;
    protected MonsterAttackSO _attackData;
    
    private bool _isAttacking;
    private CancellationTokenSource _attackCts;

    public override void InitAbility(GameObject actor, AbilitySystem asc, GameplayAbilitySO abilitySo)
    {
        base.InitAbility(actor, asc, abilitySo);
        CanReuse = true;
        
        _actor = actor;
        _movement = actor.GetComponent<MonsterMovement>();
        _monster = actor.GetComponent<Monster>();
        _attackData = abilitySo as MonsterAttackSO;
    }

    protected override bool CanActivate()
    {
        return !Asc.TagContainer.Has(GameplayTags.BlockRunningAbility);
    }

    /// <summary>
    /// 몬스터 공격 활성화
    ///
    /// *공격 전 딜레이 - 공격 - 공격 후 딜레이 (그동안 움직임은 계속 정지)
    /// **공격 중복 실행 방지를 위해 --> 취소토큰 추가, 시간측정 일정하게 수정(UniTask.Delay-Unscaled)
    /// </summary>
    protected async override void Activate()
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

            // 공격(히트박스 생성)
            Attack();

            // 공격 유효시간만큼 딜레이(히트박스 파괴될 때까지) 
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
            // (예외처리) 몬스터 사망 or 공격 취소될 경우 --> 그냥 지나가기 
        }
        finally
        {
            _movement?.SetPaused(false); // 정지 해제 
            _isAttacking = false;
            linkedCts.Dispose();
        }
    }

    protected virtual void Attack()
    {
        SpawnHitbox();
    }

    // 히트박스 생성
    protected virtual void SpawnHitbox()
    {
        GameObject prefab = _attackData.AttackHitboxPrefab;
        float attackRangeX = _attackData.AttackRangeX;
        float attackRangeY = _attackData.AttackRangeY;

        if (prefab != null)
        {
            int facingDir = _movement != null ? _movement.HorizontalDir : 1;
            Vector2 offset = new Vector2(_attackData.HitboxOffset.x * facingDir, _attackData.HitboxOffset.y);
            Vector2 spawnPos = (Vector2)_monster.transform.position + offset;

            GameObject hitbox = ResourcesManager.Instance.Instantiate(prefab);
            hitbox.GetComponent<Hitbox>()?.SetAttacker(_actor);
            hitbox.transform.position = spawnPos;

            var box = hitbox.GetComponent<BoxCollider2D>();
            if (box != null)
                box.size = new Vector2(attackRangeX, attackRangeY);

            var sr = hitbox.GetComponent<SpriteRenderer>();
            if (sr != null && sr.drawMode != SpriteDrawMode.Simple)
                sr.size = new Vector2(attackRangeX, attackRangeY);

            ResourcesManager.Instance.Destroy(hitbox, _attackData.ActiveTime);
        }
    }

#if UNITY_EDITOR
    // 히트박스 기즈모 표시
    protected virtual void OnDrawGizmosSelected()
    {
        if (_monster == null || _attackData == null) return;

        int facingDir = _movement != null ? _movement.HorizontalDir : 1;
        Vector2 offset = new Vector2(_attackData.HitboxOffset.x * facingDir, _attackData.HitboxOffset.y);
        Vector2 spawnPos = (Vector2)_monster.transform.position + offset;

        Gizmos.color = Color.blue; // 파란색 기즈모
        Gizmos.DrawWireCube(spawnPos, new Vector3(_attackData.AttackRangeX, _attackData.AttackRangeY, 0.1f));
    }
#endif
}
