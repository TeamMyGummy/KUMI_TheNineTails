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
    
    public Vector2 HitboxOffset = Vector2.zero;
    public GameObject AttackHitboxPrefab;
    
    [SerializeField] private float preAttackDelay = 0.5f;   // 공격 전 딜레이
    [SerializeField] private float activeTime     = 0.2f; // 공격 유효시간(히트박스 유지시간)
    [SerializeField] private float postAttackDelay= 0.5f; // 공격 후 딜레이
    [SerializeField] private float betweenAttackDelay = 0.3f; // 연속공격 텀
    
    
    // 읽기 전용 (외부에서 get 가능, set 불가)
    public float PreAttackDelay => preAttackDelay;
    public float ActiveTime => activeTime;
    public float PostAttackDelay => postAttackDelay;
    public float BetweenAttackDelay => betweenAttackDelay;
}