using GameAbilitySystem;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability/FireProjectile")]
public class FireProjectileSO : BlockAbilitySO
{
    public Projectile projectile;
    public SFXName AttackSound;
    
    [SerializeField] private float preAttackDelay;   // 공격 전 딜레이
    public bool isStoppWhileAttack;
    
    public float PreDelay => preAttackDelay;
}
