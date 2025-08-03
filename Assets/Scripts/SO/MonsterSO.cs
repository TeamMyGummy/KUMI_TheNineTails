using UnityEngine;

[CreateAssetMenu(menuName = "Monster/MonsterSO")]
public class MonsterSO : ScriptableObject
{
    [Header("기본 정보")]
    [SerializeField] private string monsterName = "";
    [SerializeField] private bool isFlying = false;               // 공중 몬스터 여부
    [SerializeField] private bool isBomb = false;
    [SerializeField] private bool isTriggerAttack =  false;       // 공격 범위가 따로 존재하지 않고 시야에 들어오면 공격하는 경우
    
    public string MonsterName => monsterName;
    public bool IsFlying => isFlying;
    public bool IsBomb => isBomb;
    public bool IsTriggerAttack => isTriggerAttack;

    [Header("시야각")]
    [SerializeField] private float viewStartAngle = -45f; // 시작 각도 (왼쪽부터)
    [SerializeField] private float viewSight = 90f;               // 시야각 부채꼴 각도
    
    [Header("어그로 / 해제 거리")]
    [SerializeField] private float aggroRange = 8f;               // 부채꼴 반지름 (어그로 끌리는 거리)
    [SerializeField] private float aggroReleaseRange = 14f;       // 어그로 해제 거리
    
    private float patrolRange = 6f;              //어그로 안끌렸을때 이동 반경
    
    private float pausedTime = 3f;             

    public float ViewStartAngle => viewStartAngle;
    public float ViewSight => viewSight;
    public float AggroRange => aggroRange;
    public float AggroReleaseRange => aggroReleaseRange;
    public float PausedTime => pausedTime;
    public float PatrolRange => patrolRange;

    
    [Header("공격 시작 인식 범위")]
    [SerializeField] private float detectRangeX = 1.5f;
    [SerializeField] private float detectRangeY = 1.5f;
    [SerializeField] private Vector2 detectOffset = Vector2.zero; 
    
    public float DetectRangeX => detectRangeX;
    public float DetectRangeY => detectRangeY;
    public Vector2 DetectOffset => detectOffset;
    
}