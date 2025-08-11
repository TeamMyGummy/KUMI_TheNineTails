using System.Collections;
using System.Collections.Generic;
using GameAbilitySystem;
using UnityEngine;

public class FireProjectile : BlockAbility<FireProjectileSO>
{
    protected FireProjectileSO _so;
    protected IMovement _movement;
    public override void InitAbility(GameObject actor, AbilitySystem asc, GameplayAbilitySO abilitySo)
    {
        base.InitAbility(actor, asc, abilitySo);
        _so = abilitySo as FireProjectileSO;
        _movement = actor.GetComponent<IMovement>();
    }

    protected override void Activate()
    {
        base.Activate();
        GameObject go = ResourcesManager.Instance.Instantiate(_so.projectile.gameObject);
        Vector2 direction = _movement.Direction;
        go.GetComponent<Projectile>().FireProjectile(Actor, direction);
    }
}
