using GameAbilitySystem;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class MonsterAttack : BlockAbility<MonsterAttackSO>
{
    protected MonsterMovement _movement;
    protected Monster _monster;
    protected MonsterAttackSO _attackData;

    public override void InitAbility(GameObject actor, AbilitySystem asc, GameplayAbilitySO abilitySo)
    {
        base.InitAbility(actor, asc, abilitySo);
        CanReuse = true;

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
        int facingDir = _movement != null ? _movement.GetDirection() : 1;

        Vector2 attackDir = GetAttackDirection(facingDir);

        Vector2 spawnPos = HitboxSpawnPos(attackDir);

        SpawnHitbox(spawnPos);
    }

    // 공격 방향 계산
    protected virtual Vector2 GetAttackDirection(int facingDir)
    {
        float attackDirDeg = 0f;
        switch (_attackData.AttackDir)
        {
            case AttackDirection.Front:
                attackDirDeg = (facingDir == 1) ? 0f : 180f;
                break;
            case AttackDirection.Up:
                attackDirDeg = 90f;
                break;
            case AttackDirection.Down:
                attackDirDeg = 270f;
                break;
            case AttackDirection.myself:
                attackDirDeg = 0f;
                break;
        }

        float angleRad = attackDirDeg * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)).normalized;
    }

    // 공격 위치 계산
    protected virtual Vector2 HitboxSpawnPos(Vector2 attackDir)
    {
        float attackRangeX = _attackData.AttackRangeX;
        float attackRangeY = _attackData.AttackRangeY;

        Vector2 offset;
        float monsterHeight = _monster.GetComponent<BoxCollider2D>().size.y;

        if (_attackData.AttackDir == AttackDirection.myself)
            offset = new Vector2(0f, (attackRangeY - monsterHeight) / 2f);
        else
            offset = attackDir * new Vector2(attackRangeX / 2f, attackRangeY / 2f);

        return (Vector2)_monster.transform.position + offset;
    }

    // 히트박스 생성
    protected virtual void SpawnHitbox(Vector2 spawnPos)
    {
        GameObject prefab = _attackData.AttackHitboxPrefab;
        float attackRangeX = _attackData.AttackRangeX;
        float attackRangeY = _attackData.AttackRangeY;

        if (prefab != null)
        {
            GameObject hitbox = ResourcesManager.Instance.Instantiate(prefab);
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
}