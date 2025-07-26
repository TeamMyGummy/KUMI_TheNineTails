using GameAbilitySystem;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class MonsterAttack : GameplayAbility, ITickable
{
    private MonsterMovement _movement;
    private Monster _monster;
    private float _cooltime = 0f;

    public override void InitAbility(GameObject actor, AbilitySystem asc, GameplayAbilitySO abilitySo)
    {
        base.InitAbility(actor, asc, abilitySo);
        IsTickable = true;
        CanReuse = true;

        _movement = actor.GetComponent<MonsterMovement>();
        _monster = actor.GetComponent<Monster>();
    }

    protected override bool CanActivate()
    {
        if (_cooltime > 0f) return false;
        if (_monster == null || _monster.Data == null) return false;

        Transform player = GameObject.FindWithTag("Player")?.transform;
        if (player == null) return false;

        float attackRangeX = _monster.Data.AttackRangeX * 0.75f;
        float attackRangeY = _monster.Data.AttackRangeY * 0.75f;
        int attackDirCode = _monster.Data.AttackDir;
        int facingDir = _movement != null ? _movement.GetDirection() : 1;

        float attackDirDeg = attackDirCode switch
        {
            1 => 0f,
            2 => 270f,
            _ => 0f
        };

        if ((attackDirCode == 1 || attackDirCode == 4) && facingDir == -1)
            attackDirDeg = 180f - attackDirDeg;

        float angleRad = attackDirDeg * Mathf.Deg2Rad;
        Vector2 attackDir = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)).normalized;

        // ✅ 살짝 더 앞으로 검사
        Vector2 offset = attackDir * new Vector2(attackRangeX / 2f + 0.2f, attackRangeY / 2f);
        Vector2 origin = (Vector2)Actor.transform.position + offset;

        // ✅ 여러 개 체크해서 직접 플레이어 탐색
        Collider2D[] hits = Physics2D.OverlapBoxAll(origin, new Vector2(attackRangeX, attackRangeY), 0f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log("[MonsterAttack] ✅ 플레이어 범위 안에 감지됨!");
                return true;
            }
        }

        // 혹시 모르니 한 번 더 → 몬스터 자기 위치 기준으로도 검사
        Vector2 fallbackOrigin = (Vector2)Actor.transform.position;
        Collider2D[] fallbackHits = Physics2D.OverlapBoxAll(fallbackOrigin, new Vector2(attackRangeX, attackRangeY), 0f);
        foreach (var hit in fallbackHits)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log("[MonsterAttack] ✅ Fallback 검사에서도 감지됨!");
                return true;
            }
        }

        return false;
    }



    protected override void Activate()
    {
        if (_monster == null || _monster.Data == null) return;

        var data = _monster.Data;
        float attackRangeX = data.AttackRangeX;
        float attackRangeY = data.AttackRangeY;
        int attackDirCode = data.AttackDir;

        int facingDir = _movement != null ? _movement.GetDirection() : 1;

        float attackDirDeg = attackDirCode switch
        {
            1 => 0f,
            2 => 270f,
        };

        if ((attackDirCode == 1 || attackDirCode == 4) && facingDir == -1)
            attackDirDeg = 180f - attackDirDeg;

        float angleRad = attackDirDeg * Mathf.Deg2Rad;
        Vector2 attackDir = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)).normalized;
        Vector2 offset = attackDir * new Vector2(attackRangeX / 2f, attackRangeY / 2f);
        Vector2 spawnPos = (Vector2)Actor.transform.position + offset;

        GameObject prefab = _monster.Data.AttackHitboxPrefab;
        if (prefab != null)
        {
            GameObject hitbox = GameObject.Instantiate(prefab, spawnPos, Quaternion.identity);

            var box = hitbox.GetComponent<BoxCollider2D>();
            if (box != null)
                box.size = new Vector2(attackRangeX, attackRangeY);

            var sr = hitbox.GetComponent<SpriteRenderer>();
            if (sr != null && sr.drawMode != SpriteDrawMode.Simple)
                sr.size = new Vector2(attackRangeX, attackRangeY);

            GameObject.Destroy(hitbox, 0.2f); //임의 설정
        }
        _cooltime = 1.5f;
    }

    private Vector2 GetFacingDirection()
    {
        int dir = _movement != null ? _movement.GetDirection() : 1;
        return new Vector2(dir, 0f);
    }

    public void Update()
    {
        _cooltime -= Time.deltaTime;

        if (_cooltime <= 0f && CanActivate())
        {
            Activate();
        }

        AbilityFactory.Instance.RegisterTickable(this);
    }

    public void FixedUpdate() { }
}
