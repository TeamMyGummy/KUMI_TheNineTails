using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Monster/MonsterSO")]
public class MonsterSO : ScriptableObject
{
    [Header("기본 정보")]
    [SerializeField] private string monsterName = "";
    [SerializeField] private bool isFlying = false;               // 공중 몬스터 여부
    [SerializeField] private bool isBomb = false;
    [SerializeField] private bool isTriggerAttack =  false;       // 공격 범위가 따로 존재하지 않고 시야에 들어오면 공격하는 경우
    [SerializeField] private bool isRanged = false;               // 원거리 공격 몹일 때
    [SerializeField] private bool isParryable = true;
        public bool IsParryable => isParryable;
    
    [Header("드랍템")]
    [SerializeField] private int dropCount = 2;
    [SerializeField] private GameObject honbulPrefab;
    public string MonsterName => monsterName;
    public bool IsFlying => isFlying;
    public bool IsBomb => isBomb;
    public bool IsTriggerAttack => isTriggerAttack;
    public int DropCount => dropCount;
    public  GameObject HonbulPrefab => honbulPrefab;
    public bool IsRanged => isRanged;

    [Header("시야각")]
    [SerializeField] private float viewStartAngle = -45f; // 시작 각도 (왼쪽부터)
    [SerializeField] private float viewSight = 90f;
    [SerializeField] private Vector2 viewOffset;
    
    
    [Header("어그로 / 해제 거리")]
    [SerializeField] private float aggroRange = 8f;               // 부채꼴 반지름 (어그로 끌리는 거리)
    [SerializeField] private float aggroReleaseRange = 14f;       // 어그로 해제 거리
    
    private float patrolRange = 6f;              //어그로 안끌렸을때 이동 반경
    
    private float pausedTime = 3f;             

    public float ViewStartAngle => viewStartAngle;
    public float ViewSight => viewSight;
    public Vector2 ViewOffset => viewOffset;
    public float AggroRange => aggroRange;
    public float AggroReleaseRange => aggroReleaseRange;
    public float PausedTime => pausedTime;
    public float PatrolRange => patrolRange;

    
    [Header("근거리 공격 시작 인식 범위")]
    [SerializeField] private float detectShortRangeX = 1.5f;
    [SerializeField] private float detectShortRangeY = 1.5f;
    [SerializeField] private Vector2 detectShortOffset = Vector2.zero; 
    
    public float DetectShortRangeX => detectShortRangeX;
    public float DetectShortRangeY => detectShortRangeY;
    public Vector2 DetectShortOffset => detectShortOffset;
    
    [Header("원거리 공격 시작 인식 범위")]
    [SerializeField] private float detectLongStartAngle = 0f;
    [SerializeField] private float detectLongSight = 0f;
    [SerializeField] private float detectLongRange = 0f;
    [SerializeField] private Vector2 detectLongOffset;
    
    public float DetectLongStartAngle => detectLongStartAngle;
    public float DetectLongSight => detectLongSight;
    public float DetectLongRange => detectLongRange;
    public Vector2 DetectLongOffset => detectLongOffset;
    
    
    //
}