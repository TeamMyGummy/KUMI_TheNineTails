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

        _movement = actor.GetComponent<MonsterMovement>();
        _monster = actor.GetComponent<Monster>();
    }

    protected override bool CanActivate()
    {
        if (_monster == null || _monster.Data == null) return false;

        float viewSight = _monster.Data.ViewSight;
        float attackRangeX = _monster.Data.AttackRangeX;

        Transform player = GameObject.FindWithTag("Player")?.transform;
        if (player == null) return false;

        Vector2 toPlayer = (Vector2)(player.position - Actor.transform.position);
        float angle = Vector2.Angle(GetFacingDirection(), toPlayer);
        float distance = toPlayer.magnitude;

        return angle <= viewSight * 0.5f && distance <= attackRangeX;
    }

    protected override void Activate()
    {
        if (_cooltime > 0f) return;
        if (_monster == null || _monster.Data == null) return;

        var data = _monster.Data;

        float attackRangeX = data.AttackRangeX;
        float attackRangeY = data.AttackRangeY;
        int attackDirCode = data.AttackDir;

        if (!Asc.Attribute.Attributes.TryGetValue("AttackSpeed", out var speedAttr) ||
            !Asc.Attribute.Attributes.TryGetValue("Attack", out var powerAttr)) return;

        float attackSpeed = speedAttr.CurrentValue.Value;
        float attackPower = powerAttr.CurrentValue.Value;

        float duration = 2f / attackSpeed;
        _cooltime = duration;

        int facingDir = _movement != null ? _movement.GetDirection() : 1;

        float attackDirDeg = attackDirCode switch
        {
            1 => 0f,     // 전방
            2 => 270f,   // 수직 아래
        };

        if ((attackDirCode == 1 || attackDirCode == 4) && facingDir == -1)
            attackDirDeg = 180f - attackDirDeg;

        float angleRad = attackDirDeg * Mathf.Deg2Rad;
        Vector2 attackDir = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)).normalized;

        Vector2 size = new Vector2(attackRangeX, attackRangeY);
        Vector2 offset = new Vector2(attackDir.x * attackRangeX / 2f, attackDir.y * attackRangeY / 2f);
        Vector2 origin = (Vector2)Actor.transform.position + offset;

        Collider2D hit = Physics2D.OverlapBox(origin, size, 0f);
        if (hit && hit.CompareTag("Player"))
        {
            if (hit.TryGetComponent(out Damageable damageable))
            {
                damageable.GetDamage(DomainKey.Player, attackPower);
                Debug.Log($"[MonsterAttack] 플레이어 공격! {attackPower} 데미지");
            }
        }

        AttackEnd(duration).Forget();
    }

    private Vector2 GetFacingDirection()
    {
        int dir = _movement != null ? _movement.GetDirection() : 1;
        return new Vector2(dir, 0f);
    }

    private async UniTask AttackEnd(float duration)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(duration));
        _cooltime = 0f;
        AbilityFactory.Instance.EndAbility(this);
    }

    public void Update()
    {
        if (_cooltime > 0f)
            _cooltime -= Time.deltaTime;
    }

    public void FixedUpdate() { }
}
