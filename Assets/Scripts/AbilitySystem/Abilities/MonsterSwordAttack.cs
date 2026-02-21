using GameAbilitySystem;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class MonsterSwordAttack : BlockAbility<MonsterAttackSO>
{
    protected GameObject _actor;
    protected MonsterMovement _movement;
    protected Monster _monster;
    protected MonsterAttackSO _attackData;

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

    protected async override void Activate()
    {
        if (_attackData == null) return;
        
        float block =
            Mathf.Max(0f, _attackData.PreDelay) +      
            Mathf.Max(0f, _attackData.ActiveTime) +      
            Mathf.Max(0f, _attackData.PreDelay);    
        _attackData.BlockTimer = block; 

        base.Activate();

        _movement?.SetPaused(true); 
        try
        {
            // 전딜
            if (_attackData.PreDelay > 0f)
                await UniTask.Delay(
                    TimeSpan.FromSeconds(_attackData.PreDelay),
                    delayType: DelayType.DeltaTime);    

            // 공격(히트박스 생성)
            Attack();

            // 액티브타임
            if (_attackData.ActiveTime > 0f)
                await UniTask.Delay(
                    TimeSpan.FromSeconds(_attackData.ActiveTime),
                    delayType: DelayType.DeltaTime); 

            // 후딜
            if (_attackData.PreDelay > 0f)
                await UniTask.Delay(
                    TimeSpan.FromSeconds(_attackData.PreDelay),
                    delayType: DelayType.DeltaTime);  
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
    protected virtual void SpawnHitbox(float lifeTime)
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
            ParticleSystemRenderer renderer = hitbox.GetComponent<ParticleSystemRenderer>();
            if(renderer != null)
                renderer.flip = facingDir == 1 ? new Vector3(0f, renderer.flip.y, 0f) : new Vector3(1f, renderer.flip.y, 0f);
            hitbox.transform.position = spawnPos;

            var box = hitbox.GetComponent<BoxCollider2D>();
            if (box != null)
                box.size = new Vector2(attackRangeX, attackRangeY);

            var sr = hitbox.GetComponent<SpriteRenderer>();
            if (sr != null && sr.drawMode != SpriteDrawMode.Simple)
                sr.size = new Vector2(attackRangeX, attackRangeY);

            if (lifeTime <= 0f) return;  
            ResourcesManager.Instance.Destroy(hitbox, lifeTime);
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
