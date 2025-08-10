using System.Collections;
using System.Collections.Generic;
using GameAbilitySystem;
using UnityEngine;

public class FireProjectile : GameplayAbility<FireProjectileSO>
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
        GameObject go = ResourcesManager.Instance.Instantiate(_so.projectile.gameObject);
        Vector2 direction = _movement.Direction;
        go.GetComponent<IProjectile>().FireProjectile(Actor, direction);
    }
}
