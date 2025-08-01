using UnityEngine;
using GameAbilitySystem;

[CreateAssetMenu(menuName = "Ability/MonsterRushSO")]

public class MonsterRushSO : BlockAbilitySO
{
    [Header("공격 관련")]
    public float AttackRangeX = 1f;
    public float AttackRangeY = 1f;
    public AttackDirection AttackDir = AttackDirection.Front;
    public float RushDistance = 5f;
    public GameObject AttackHitboxPrefab;
    
}