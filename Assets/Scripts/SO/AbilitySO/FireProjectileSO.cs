using GameAbilitySystem;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability/FireProjectile")]
public class FireProjectileSO : BlockAbilitySO
{
    public Projectile projectile;

    public bool isStoppWhileAttack;
}
