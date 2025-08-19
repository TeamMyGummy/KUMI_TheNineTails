using GameAbilitySystem;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class MonsterAttack : BlockAbility<MonsterAttackSO>
{
    private GameObject _actor;
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

    protected override bool CanActivate()
    {
        return !Asc.TagContainer.Has(GameplayTags.BlockRunningAbility);
    }

    protected async override void Activate()
    {
        base.Activate();
        if (_attackData == null) return;
        
        if (_attackData.isStoppingWhileAttack) 
        {
            _movement?.SetPaused(true);
        }

        Attack();

        if (_attackData.isStoppingWhileAttack)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.3f));
            _movement?.SetPaused(false);
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

            ResourcesManager.Instance.Destroy(hitbox, 0.2f);
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
