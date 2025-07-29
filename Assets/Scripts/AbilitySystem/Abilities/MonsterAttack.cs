using GameAbilitySystem;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class MonsterAttack : BlockAbility
{
    private MonsterMovement _movement;
    private Monster _monster;
    private MonsterAttackSO _attackData;

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
        //BlockAbility중이면 다시 못 씀
        return !Asc.TagContainer.Has(GameplayTags.BlockRunningAbility);
    }

    protected override void Activate()
    {
        base.Activate();
        
        if (_attackData == null) return;

        //실제 공격 범위
        float attackRangeX = _attackData.AttackRangeX;
        float attackRangeY = _attackData.AttackRangeY;
        //공격 방향 (전방, 하단) >> 방식을 수정하고싶은데... 일단 냅둠
        AttackDirection attackDirCode = _attackData.AttackDir;
        GameObject prefab = _attackData.AttackHitboxPrefab;
        int facingDir = _movement != null ? _movement.GetDirection() : 1;

        
        //공격 방향 계산
        //-------------------------------------------------------
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
        }
        float angleRad = attackDirDeg * Mathf.Deg2Rad;
        Vector2 attackDir = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)).normalized;

        Vector2 offset = attackDir * new Vector2(attackRangeX / 2f, attackRangeY / 2f);
        Vector2 spawnPos = (Vector2)_monster.transform.position + offset;
        //-------------------------------------------------------
        
        
        //히트박스 프리팹 생성
        //-------------------------------------------------------
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
        //-------------------------------------------------------
    }

}
