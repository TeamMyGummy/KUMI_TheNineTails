using System.Collections;
using System.Collections.Generic;
using GameAbilitySystem;
using UnityEngine;

public class FireProjectile : GameplayAbility<FireProjectileSO>
{
    protected FireProjectileSO _so;
    protected PlayerController _pc;
    public override void InitAbility(GameObject actor, AbilitySystem asc, GameplayAbilitySO abilitySo)
    {
        base.InitAbility(actor, asc, abilitySo);
        _so = abilitySo as FireProjectileSO;
        _pc = actor.GetComponent<PlayerController>();
    }

    protected override void Activate()
    {
        GameObject go = ResourcesManager.Instance.Instantiate(_so.projectile.gameObject);
        Vector2 direction = _pc.Direction;
        go.GetComponent<Projectile>().InitProjectile(Actor, direction);
    }
}
