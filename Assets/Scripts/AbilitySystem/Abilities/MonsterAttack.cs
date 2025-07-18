using GameAbilitySystem;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class MonsterAttack : GameplayAbility, ITickable
{
    private MonsterMovement _movement;
    private float _cooltime = 0f;

    public override void InitAbility(GameObject actor, AbilitySystem asc, GameplayAbilitySO abilitySo)
    {
        base.InitAbility(actor, asc, abilitySo);
        IsTickable = true;

        _movement = actor.GetComponent<MonsterMovement>();
    }

    protected override bool CanActivate()
    {
        return true;
    }

    protected override void Activate()
    {
        // 쿨타임 중이면 실행 취소
        if (_cooltime > 0f)
        {
            return;
        }

        var attr = Asc.Attribute.Attributes;

        float attackRangeX = attr["AttackRangeX"].CurrentValue.Value;
        float attackRangeY = attr["AttackRangeY"].CurrentValue.Value;
        float attackSpeed = attr["AttackSpeed"].CurrentValue.Value;
        float attackPower = attr["Attack"].CurrentValue.Value;

        //임시로 정해둠
        float duration = 2f / attackSpeed;
        _cooltime = duration;

        int dir = _movement != null ? _movement.GetDirection() : 1;
        Vector2 origin = Actor.transform.position;
        Vector2 size = new Vector2(attackRangeX, attackRangeY);
        Vector2 center = origin + new Vector2(dir * size.x / 2f, 0f);

        Collider2D hit = Physics2D.OverlapBox(center, size, 0f);
        if (hit && hit.CompareTag("Player"))
        {
            if (hit.TryGetComponent(out Damageable damageable))
            {
                damageable.GetDamage(DomainKey.Player, attackPower);
                Debug.Log($"[MonsterAttack] 플레이어 공격 → {attackPower} 데미지 적용!");
            }
        }
        else
        {
            Debug.Log("[MonsterAttack] 범위 안에 플레이어 없음");
        }

       
        AttackEnd(duration).Forget();
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
