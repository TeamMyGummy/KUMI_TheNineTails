using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;


public class MonsterMovement : MonoBehaviour, IMovement
{
    public CharacterMovement _characterMovement { private set; get; }
    public Monster _monster { private set; get; }
    private Collider2D _monsterCollider;
    public Animator _animator{ private set; get; }
    private SkeletonMecanim _skeletonMecanim;
    
    
    public int HorizontalDir { set; get; } //좌우판정용 (set 추가)

    //몬스터 이동 상태 클래스
    private IMonsterMovementState _currentState;
    public IMonsterMovementState CurrentState => _currentState;
    private StoppedState _stoppedState;
    private PatrolState _patrolState;
    public AggroState _aggroState;
    private ReturnState _returnState;
    private RetreatState _retreatState;
    private FleeState _fleeState;
    private ParriedState _parriedState;
    private OuchState _ouchState;
    
    //Patrol / Return 관련 변수
    public Vector2 _spawnPos { private set; get; }
    public Vector2 _leftPatrolLimit { private set; get; }
    public Vector2 _rightPatrolLimit { private set; get; }

    //멈춤 관련 변수
    private bool _isPaused = false;
    private bool _isTimerPaused = false;
    private float _pauseTimer = 0f;
    private float _pauseTimerTarget = 0f;
    private bool _shouldFlipAfterPause = false;

    public Transform _player { private set; get; }
    public Transform _headPivot { private set; get; } //몬3용도

    [SerializeField] public LayerMask platformLayer;

    [HideInInspector] public bool _canChangeDirection = true;
    [HideInInspector] public float _directionCheckTimer = 0f;
    public readonly float _directionCheckDelay = 0.5f;
    public const float ReturnArrivalThreshold = 0.9f;

    // 원거리 몬스터 후퇴 관련 변수
    [HideInInspector] public Vector2 _retreatStartPos;
    [HideInInspector] public Vector2 _retreatTargetPos;
    [HideInInspector] public bool _hasReachedRetreatTarget = false;
    public const float RetreatDistance = 2f;
    private const float CloseRangeThreshold = 3f;
    private const float FleeRangeThreshold = 4f;
    public const float RetreatArrivalThreshold = 0.2f;
    public const float RetreatSpeedMultiplier = 3.0f;
    
    // 애니메이션 파라미터
    public static readonly int WalkID = Animator.StringToHash("Walk");
    public static readonly int ChaseID = Animator.StringToHash("Chase");
    public static readonly int HurtID = Animator.StringToHash("Hurt");
    public static readonly int ParriedID = Animator.StringToHash("Parried");


    private void Awake()
    {
        _characterMovement = GetComponent<CharacterMovement>();
        _monster = GetComponent<Monster>();
        _monsterCollider = GetComponent<Collider2D>();
        _animator = GetComponent<Animator>();
        _skeletonMecanim = GetComponent<SkeletonMecanim>();
    }

    Vector2 IMovement.Direction => new Vector2(HorizontalDir, 0f);

    private void Start()
    {
        HorizontalDir = UnityEngine.Random.value < 0.5f ? -1 : 1;
        _spawnPos = transform.position;
        _leftPatrolLimit = _spawnPos + Vector2.left * (_monster.Data.PatrolRange / 2f);
        _rightPatrolLimit = _spawnPos + Vector2.right * (_monster.Data.PatrolRange / 2f);

        _player = _monster.PlayerTransform;
        if (_player == null)
            _player = GameObject.FindWithTag("Player")?.transform;

        if (_monster.Data.IsFlying && _player != null)
        {
            _headPivot = _player.Find("HeadPivot");
            if (_headPivot == null) Debug.LogWarning("[MonsterMovementUnified] HeadPivot == null");
        }
        
        if (_player == null) Debug.LogError("[MonsterMovement] Player not found");

        _stoppedState = new StoppedState();
        _patrolState = new PatrolState();
        _aggroState = new AggroState();
        _returnState = new ReturnState();
        _retreatState = new RetreatState();
        _fleeState = new FleeState();
        _parriedState = new ParriedState();
        _ouchState = new OuchState();

        _currentState = _patrolState;
        _currentState.Enter(this);
    }

    private void Update()
    {
        if (_player == null) return;

        HandlePauseTimer();
        if (_isPaused) return;

        _currentState.UpdateState();
        HandleStateTransition();

        // 애니메이션 처리
        Vector2 dir = GetDirection();
        if (_animator != null && _skeletonMecanim != null)
        {
            _skeletonMecanim.Skeleton.ScaleX = (dir.x > 0 ? -1f : 1f) * Mathf.Abs(_skeletonMecanim.Skeleton.ScaleX);
        }
    }

    public void ChangeState(IMonsterMovementState newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState?.Enter(this);
    }

    private void HandleStateTransition()
    {
        float dist = Vector2.Distance(transform.position, _player.position);

        if (_currentState is PatrolState)
        {
            if (_monster.isAggro)
            {
                ChangeState(_aggroState);
                return;
            }
        }
        // 멈춤 타이머가 아직 끝나지 않았더라도 시야에 들어오면 어그로 on
        else if (_currentState is StoppedState)
        {
            if (_monster.isAggro) ChangeState(_aggroState);
        }
        else if (_currentState is AggroState)
        {
            if (!_monster.isAggro && dist >= _monster.Data.AggroReleaseRange) ChangeState(_returnState);
            else if (_monster.Data.IsRanged && dist <= CloseRangeThreshold) ChangeState(_retreatState);
        }
        else if (_currentState is RetreatState)
        {
            if (_hasReachedRetreatTarget) ChangeState(_fleeState);
        }
        else if (_currentState is FleeState)
        {
            if (dist >= FleeRangeThreshold) ChangeState(_aggroState);
            else if (!_monster.isAggro && dist >= _monster.Data.AggroReleaseRange) ChangeState(_returnState);
        }
        else if (_currentState is ReturnState)
        {
            float returnDist = Vector2.Distance(transform.position, _spawnPos);
            if (returnDist <= ReturnArrivalThreshold)
            {
                HorizontalDir = 1;
                ChangeState(_patrolState);
            }
            if (_monster.isAggro) ChangeState(_aggroState);
        }
    }
    
    public void EnterParriedState()
    {
        ChangeState(_parriedState);
    }
    public void EnterOuchState()
    {
        ChangeState(_ouchState);
    }
    
    public Vector2 GetDirection()
    {
        if (_monster.Data.IsFlying)
        {
            if (_currentState is AggroState)
            {
                Vector2 dirVec = ((Vector2)_headPivot.position - (Vector2)transform.position + new Vector2(0f, -0.1f)).normalized;
                HorizontalDir = dirVec.x < 0 ? -1 : 1;
                return dirVec;
            }
            else if (_currentState is ReturnState)
            {
                Vector2 dirVec = (_spawnPos - (Vector2)transform.position).normalized;
                HorizontalDir = dirVec.x < 0 ? -1 : 1;
                return dirVec;
            }
            else
            {
                return Vector2.right * HorizontalDir;
            }
        }
        else 
        {
            if (_currentState is ReturnState)
            {
                Vector2 dirVec = (_spawnPos - (Vector2)transform.position).normalized;
                HorizontalDir = dirVec.x < 0 ? -1 : 1;
                return Vector2.right * HorizontalDir;
            }
            else
            {
                return Vector2.right * HorizontalDir;
            }
        }
    }

    public void SetPaused(bool paused)
    {
        _isPaused = paused;
        if (paused) _characterMovement.Move(Vector2.zero);
    }

    public void PauseForSeconds(float time, bool flipAfterPause = false)
    {
        _isTimerPaused = true;
        _pauseTimer = 0f;
        _pauseTimerTarget = time;
        SetPaused(true);
        _shouldFlipAfterPause = flipAfterPause;
    }

    private void HandlePauseTimer()
    {
        if (!_isTimerPaused) return;
        _pauseTimer += Time.deltaTime;
        if (_pauseTimer >= _pauseTimerTarget)
        {
            _isTimerPaused = false;
            SetPaused(false);
            if (_shouldFlipAfterPause)
            {
                HorizontalDir *= -1;
                _shouldFlipAfterPause = false;
            }
            if (_currentState is StoppedState)
            {
                if (_monster.isAggro)
                {
                    ChangeState(_aggroState);
                }
                else
                {
                    ChangeState(_patrolState);
                }
            }
        }
    }
    
    public bool CheckGroundAhead()
    {
        if (_monster.Data.IsFlying) return true;
        
        Vector2 checkPos = (Vector2)transform.position + new Vector2(HorizontalDir * 0.8f, -0.2f);
        float checkDistance = 1.2f;

        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, checkDistance, platformLayer);
        return hit.collider != null;
    }

    public bool CheckWallAhead()
    {
        float width = 0.1f; // 감지 폭
        float height = _monsterCollider.bounds.size.y * 0.9f; // 세로 범위
        Vector2 origin = (Vector2)transform.position + new Vector2(HorizontalDir * (width/2 + _monsterCollider.bounds.extents.x), 1f);

        RaycastHit2D hit = Physics2D.BoxCast(origin, new Vector2(width, height), 0f, Vector2.right * HorizontalDir, 0.1f, platformLayer);
        Debug.DrawRay(origin, Vector2.right * HorizontalDir * 0.1f, Color.red);

        return hit.collider != null;
    }


    public bool IsPlayerMoving()
    {
        if (_player == null) return false;
        Rigidbody2D rb = _player.GetComponent<Rigidbody2D>();
        if (rb == null) return true;
        return rb.velocity.magnitude > 0.1f;
    }
    
    public void LockDirection(bool lockDir)
    {
        _canChangeDirection = !lockDir;
    }
    
    public float CalculateSafeRetreatDistance(Vector2 retreatDirection)
    {
        float maxSafeDistance = 0f;
        float checkStep = 0.2f; // 0.2유닛씩 체크
        
        for (float distance = checkStep; distance <= RetreatDistance; distance += checkStep)
        {
            Vector2 checkPos = _retreatStartPos + retreatDirection * distance;
            
            // 해당 위치에서 바닥이 있는지 체크
            Vector2 groundCheckPos = checkPos + new Vector2(0, -0.2f);
            RaycastHit2D hit = Physics2D.Raycast(groundCheckPos, Vector2.down, 1.2f, platformLayer);
            
            if (hit.collider != null)
            {
                maxSafeDistance = distance;
            }
            else
            {
                break; // 바닥이 없으면 더 이상 후퇴하지 않음
            }
        }
        
        // 최소 0.5유닛은 후퇴할 수 있도록 보장
        return Mathf.Max(maxSafeDistance, 0.5f);
    }
}

//========== state 클래스들 ==============

/// <summary>
/// StoppedState (Idle) 멈춰있는 상태, 패트롤 중 멈출 일이 있을 때만 stoppedstate라고 보면 됨
/// 공격 중 멈추는 것은 ㄴㄴ.. 공격 중엔 그냥 aggro 상태임
/// </summary>
public class StoppedState : IMonsterMovementState
{
    private MonsterMovement _mm;
    
    public void Enter(MonsterMovement monsterMovement)
    {
        _mm = monsterMovement;
        _mm.PauseForSeconds(_mm._monster.Data.PausedTime, true);
        _mm._animator.SetBool(MonsterMovement.WalkID, false);
    }

    public void UpdateState()
    {
    }

    public void Exit()
    {
    }
}

/// <summary>
/// PatrolState : 순찰중인 상태
/// 왕복 이동 중에 낭떠러지/벽 만나면 3초 정지 후 방향 바꿈. => (StoppedState로 전환)
/// </summary>
public class PatrolState : IMonsterMovementState
{
    private MonsterMovement _mm;

    public void Enter(MonsterMovement monsterMovement)
    {
        _mm = monsterMovement;
        _mm._animator.SetBool(MonsterMovement.WalkID, true);
    }

    public void UpdateState()
    {
        if (_mm.CheckWallAhead() || (!_mm._monster.Data.IsFlying && !_mm.CheckGroundAhead()))
        {
            _mm.ChangeState(new StoppedState());
            return;
        }
        _mm._characterMovement.Move(_mm.GetDirection());

        if ((_mm.HorizontalDir == 1 && _mm.transform.position.x >= _mm._rightPatrolLimit.x) ||
            (_mm.HorizontalDir == -1 && _mm.transform.position.x <= _mm._leftPatrolLimit.x))
        {
            _mm.HorizontalDir *= -1;
        }
    }

    public void Exit()
    {
        //_mm._animator.SetBool(MonsterMovement.WalkID, false);
    }
}

/// <summary>
/// AggroState : 어그로 끌린 상태
/// 플레이어를 쫓아감
/// </summary>
public class AggroState : IMonsterMovementState
{
    private MonsterMovement _mm;

    public void Enter(MonsterMovement monsterMovement)
    {
        _mm = monsterMovement;
        _mm._animator.SetBool(MonsterMovement.WalkID, true);
    }

    public void UpdateState()
    {
        if (_mm._canChangeDirection)
        {
            _mm._directionCheckTimer -= Time.deltaTime;
            if (_mm._directionCheckTimer <= 0f)
            {
                _mm._directionCheckTimer = _mm._directionCheckDelay;
                if (!_mm._monster.Data.IsFlying)
                {
                    _mm.HorizontalDir = _mm._player.position.x > _mm.transform.position.x ? 1 : -1;
                }
            }
        }
        //플레이어가 이동중이지 않을 때만 공격 사이에 멈춰 있었어서, 수정
        bool inShortRange = _mm._monster.IsPlayerInShortRange();
        bool inLongRange = _mm._monster.Data.IsTriggerAttack && _mm._monster.IsPlayerInLongRange(); // 원거리 공격 가능하고 사거리 내라면

        if (inShortRange || inLongRange)
        {
            _mm._characterMovement.Move(Vector2.zero);
            _mm._animator.SetBool(MonsterMovement.WalkID, false); 
            return;
        }
        else
        {
            _mm._animator.SetBool(MonsterMovement.WalkID, true);
        }
        if (!_mm._monster.Data.IsFlying && !_mm.CheckGroundAhead())
        {
            _mm._characterMovement.Move(Vector2.zero);
            return;
        }
        _mm._characterMovement.Move(_mm.GetDirection());
    }

    public void Exit()
    {
        //_mm._animator.SetBool(MonsterMovement.ChaseID, false);
    }
}

/// <summary>
/// ReturnState : Aggro 중에 Aggro 풀렸을 때 스폰 지점으로 되돌아가기
/// </summary>
public class ReturnState : IMonsterMovementState
{
    private MonsterMovement _mm;

    public void Enter(MonsterMovement monsterMovement)
    {
        _mm = monsterMovement;
    }

    public void UpdateState()
    {
        _mm._characterMovement.Move(_mm.GetDirection());
        // 스폰 위치 도착 판단은 HandleStateTransition에서
    }

    public void Exit() { }
}

/// <summary>
/// RetreatState : (원거리몹) 거리 가까울 경우 후퇴
/// </summary>
public class RetreatState : IMonsterMovementState
{
    private MonsterMovement _mm;

    public void Enter(MonsterMovement monsterMovement)
    {
        _mm = monsterMovement;
        
        // StartRetreat() 로직 시작
        _mm._retreatStartPos = _mm.transform.position; 
        _mm._hasReachedRetreatTarget = false;
        
        // 플레이어 반대 방향으로 후퇴 목표점 설정
        Vector2 retreatDirection = _mm._player.position.x > _mm.transform.position.x ? Vector2.left : Vector2.right;
        
        // 지상 몬스터의 경우 후퇴 목표점이 낭떠러지가 아닌지 확인
        if (!_mm._monster.Data.IsFlying)
        {
            // 후퇴 방향으로 안전한 거리 계산
            float safeRetreatDistance = _mm.CalculateSafeRetreatDistance(retreatDirection); // 기존 MonsterMovement 메서드 사용
            _mm._retreatTargetPos = _mm._retreatStartPos + retreatDirection * safeRetreatDistance; 
        }
        else
        {
            _mm._retreatTargetPos = _mm._retreatStartPos + retreatDirection * MonsterMovement.RetreatDistance;
        }
        
        // 플레이어를 바라보는 방향을 설정 (후퇴하기 전에)
        _mm.HorizontalDir = _mm._player.position.x > _mm.transform.position.x ? 1 : -1;
    }

    public void UpdateState()
    {
        // 후퇴 목표점까지의 방향 계산
        Vector2 retreatDir = (_mm._retreatTargetPos - (Vector2)_mm.transform.position).normalized;
        
        // 플레이어를 바라보는 방향은 유지 (후퇴하면서도 플레이어를 봄)
        _mm.HorizontalDir = _mm._player.position.x > _mm.transform.position.x ? 1 : -1;

        // 지상몬스터인 경우 낭떠러지/벽 체크 - 후퇴 방향으로
        if (!_mm._monster.Data.IsFlying)
        {
            // 후퇴 방향으로 바닥이 있는지 체크
            int retreatHorizontalDir = retreatDir.x < 0 ? -1 : 1;
            Vector2 checkPos = (Vector2)_mm.transform.position + new Vector2(retreatHorizontalDir * 0.8f, -0.2f);
            RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, 1.2f, _mm.platformLayer);
            
            if (hit.collider == null)
            {
                // 후퇴할 수 없는 상황이면 즉시 도망 상태로 전환
                _mm._hasReachedRetreatTarget = true;
                return;
            }
            
            // 벽도 체크
            Vector2 wallCheckPos = (Vector2)_mm.transform.position + new Vector2(retreatHorizontalDir * 0.5f, 0);
            RaycastHit2D wallHit = Physics2D.Raycast(wallCheckPos, Vector2.right * retreatHorizontalDir, 0.5f, _mm.platformLayer);
            
            if (wallHit.collider != null)
            {
                // 벽이 있으면 즉시 도망 상태로 전환
                _mm._hasReachedRetreatTarget = true;
                return;
            }
        }
        
        // 후퇴 이동
        if (_mm._monster.Data.IsFlying)
        {
            _mm._characterMovement.Move(retreatDir * MonsterMovement.RetreatSpeedMultiplier);
        }
        else
        {
            // 지상 몬스터는 좌우로만 이동
            int moveDir = retreatDir.x < 0 ? -1 : 1;
            _mm._characterMovement.Move(Vector2.right * moveDir * MonsterMovement.RetreatSpeedMultiplier);
        }
        
        // 후퇴 목표점 도달 체크
        float distToTarget = Vector2.Distance(_mm.transform.position, _mm._retreatTargetPos);
        if (distToTarget <= MonsterMovement.RetreatArrivalThreshold)
        {
            _mm._hasReachedRetreatTarget = true;
        }
    }

    public void Exit() { }
}


/// <summary>
/// FleeState : (원거리몹) 도망...
/// </summary>
public class FleeState : IMonsterMovementState
{
    private MonsterMovement _mm;

    public void Enter(MonsterMovement monsterMovement)
    {
        _mm = monsterMovement;
    }

    public void UpdateState()
    {
        Vector2 fleeDirection;
        if (_mm._monster.Data.IsFlying)
        {
            fleeDirection = ((Vector2)_mm.transform.position - (Vector2)_mm._player.position).normalized;
            _mm.HorizontalDir = fleeDirection.x < 0 ? -1 : 1;
        }
        else
        {
            _mm.HorizontalDir = _mm._player.position.x > _mm.transform.position.x ? -1 : 1;
            fleeDirection = Vector2.right * _mm.HorizontalDir;

            if (!_mm.CheckGroundAhead())
            {
                _mm._characterMovement.Move(Vector2.zero);
                return;
            }
        }
        _mm._characterMovement.Move(fleeDirection);
    }

    public void Exit() { }
}

/// <summary>
/// 몬스터가 패링 당해 기절한 상태
/// </summary>
public class ParriedState : IMonsterMovementState
{
    private MonsterMovement _mm;
    private float _duration;
    private float _timer;

    public void Enter(MonsterMovement monsterMovement)
    {
        _mm = monsterMovement;

        _duration = 0.1f; //패링당한 모션 내보낼 시간
        _timer = 0f;
        
        _mm._characterMovement.Move(Vector2.zero);
        _mm._animator.SetTrigger(MonsterMovement.ParriedID);
    }

    public void UpdateState()
    {
        _timer += Time.deltaTime;
        if (_timer >= _duration)
        {
            _mm.ChangeState(_mm._aggroState);
        }
    }

    public void Exit()
    {
        
    }
}

/// <summary>
/// 몬스터가 공격 당한 상태
/// </summary>
public class OuchState : IMonsterMovementState
{
    private MonsterMovement _mm;
    private float _timer;
    private const float MinStunTime = 0.3f;

    public void Enter(MonsterMovement monsterMovement)
    {
        _mm = monsterMovement;
        _timer = 0f;
        
        _mm._characterMovement.Move(Vector2.zero);
        _mm._animator.SetTrigger(MonsterMovement.HurtID);
    }

    public void UpdateState()
    {
        _mm._characterMovement.Move(Vector2.zero);
        _timer += Time.deltaTime;
        
        if (_timer < MinStunTime) return;
        
        var curAnimStateInfo = _mm._animator.GetCurrentAnimatorStateInfo(0);
        var nextAnimStateInfo = _mm._animator.GetNextAnimatorStateInfo(0);
        
        if (curAnimStateInfo.IsName("Hurt") || nextAnimStateInfo.IsName("Hurt")) // 전이 중일 때 고려
        {
            if (curAnimStateInfo.normalizedTime >= 0.5f)
            {
                _mm.ChangeState(_mm._aggroState);
            }
        }
        else
        {
            _mm.ChangeState(_mm._aggroState);
        }

    }

    public void Exit()
    {
        
    }
}