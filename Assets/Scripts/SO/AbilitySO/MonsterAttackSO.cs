using UnityEngine;
using GameAbilitySystem;

public enum AttackDirection
{
    Front = 1,
    Up = 2,
    Down = 3,
    myself = 4,
}
[CreateAssetMenu(menuName = "Ability/MonsterAttackSO")]

public class MonsterAttackSO : BlockAbilitySO
{
    [Header("공격 관련")]
    public float AttackRangeX = 2f;
    public float AttackRangeY = 2f;
    public AttackDirection AttackDir = AttackDirection.Front;
    public GameObject AttackHitboxPrefab;
    
    public bool isStoppingWhileAttack = false;
    
}