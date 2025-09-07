using GameAbilitySystem;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class MonsterAttack2 : BlockAbility<MonsterAttackSO>
{
    protected GameObject _actor;
    protected MonsterMovement _movement;
    protected Monster _monster;
    protected MonsterAttackSO _attackData;
    
    protected virtual bool UseBasicAttack => true;

    public override void InitAbility(GameObject actor, AbilitySystem asc, GameplayAbilitySO abilitySo)
    {
        base.InitAbility(actor, asc, abilitySo);
        CanReuse = true;
        
        _actor = actor;
        _movement = actor.GetComponent<MonsterMovement>();
        _monster = actor.GetComponent<Monster>();
        _attackData = abilitySo as MonsterAttackSO;
    }

    public override bool CanActivate()
    {
        return !Asc.TagContainer.Has(GameplayTags.BlockRunningAbility);
    }

    /// <summary>
    /// 몬스터의 공격을 실행하는 메서드
    /// 1. BlockTimer를 계산해서 다른 스킬 실행을 일정 시간 동안 막음
    /// 2. UseBasicAttack == true일 경우
    ///    - 몬스터 이동을 일시 중지하고
    ///    - PreDelay → 공격 실행(Attack) → ActiveTime → PostDelay만큼 시간이 지나면
    ///    - 이동 중지 해제
    /// 3. UseBasicAttack == false일 경우:
    ///     공격 부분 실행x. BlockAbility만 필요한 경우 사용 (ex. 더블어택코드)
    /// 
    /// **base.Activate()를 반드시 호출해야 BlockAbility 쪽의 블락 기능이 적용됨
    /// </summary>
    protected async override void Activate()
    {
        if (_attackData == null) return;
        
        // 자식이 미리 BlockTimer를 설정했다면 그대로 사용, 아니면 기본계산
        float block =  (_attackData.BlockTimer > 0f)           
            ? _attackData.BlockTimer               
            : Mathf.Max(0f, _attackData.PreDelay) +
            Mathf.Max(0f, _attackData.ActiveTime) +
            Mathf.Max(0f, _attackData.PostDelay);        
        _attackData.BlockTimer = block;  
        
        base.Activate();
        
        if (!UseBasicAttack) return; 
        
        
        _movement?.SetPaused(true);               
        try
        {
            if (_attackData.PreDelay > 0f)
                await UniTask.Delay(TimeSpan.FromSeconds(_attackData.PreDelay), delayType: DelayType.DeltaTime); 

            Attack();

            if (_attackData.ActiveTime > 0f)
                await UniTask.Delay(TimeSpan.FromSeconds(_attackData.ActiveTime), delayType: DelayType.DeltaTime);

            if (_attackData.PostDelay > 0f)
                await UniTask.Delay(TimeSpan.FromSeconds(_attackData.PostDelay), delayType: DelayType.DeltaTime); 
        }
        finally
        {
            _movement?.SetPaused(false);                      
        }
    }

    protected virtual void Attack()
    {
        SpawnHitbox(_attackData.ActiveTime);
    }

    // 히트박스 생성
    protected virtual void SpawnHitbox(float activeTime)
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

            if (activeTime <= 0f) return;
            ResourcesManager.Instance.Destroy(hitbox, activeTime);
        }
    }

    public Vector2 GetPosition()
    {
        return Actor.transform.position;
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
