using UnityEngine;

[CreateAssetMenu(menuName = "Monster/MonsterSO")]
public class MonsterSO : ScriptableObject
{
    [Header("기본 정보")]
    [SerializeField] private string monsterName = "";
    [SerializeField] private bool isParrying = true;
    

    public string MonsterName => monsterName;
    public bool IsParrying => isParrying;

    [Header("AI 시야 / 이동")]
    [SerializeField] private float viewSight = 90f;               // 시야각 부채꼴 각도
    [SerializeField] private float aggroRange = 8f;               // 부채꼴 반지름 (어그로 끌리는 거리)
    [SerializeField] private float aggroReleaseRange = 14f;       // 어그로 해제 거리
    [SerializeField] private bool isFlying = false;               // 공중 몬스터 여부
    [SerializeField] private float pausedTime = 3f;             

    public float ViewSight => viewSight;
    public float AggroRange => aggroRange;
    public float AggroReleaseRange => aggroReleaseRange;
    public bool IsFlying => isFlying;
    public float PausedTime => pausedTime;

    [Header("공격 관련")]
    [SerializeField] private float attackRangeX = 2f; // 공격 가로 범위
    [SerializeField] private float attackRangeY = 2f; // 공격 세로 범위
    [SerializeField] private int attackDir = 1; // 몹 중앙 기준 공격 방향 (1=전방, 2=하단)
    [SerializeField] private GameObject attackHitboxPrefab;
    public GameObject AttackHitboxPrefab => attackHitboxPrefab;

    public float AttackRangeX => attackRangeX;
    public float AttackRangeY => attackRangeY;
    public int AttackDir => attackDir;

    [Header("이동 설정")]
    [SerializeField] private float patrolRange = 6f; //어그로 안끌렸을때 이동 반경

    public float PatrolRange => patrolRange;
}