using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MovePattern
{
    Patrol,
    Aggro,
    Return,
    Retreat,
    Flee,
}

public class MonsterMovement : MonoBehaviour, IMovement
{
    private CharacterMovement _cm;
    private Monster _monster;
    private Collider2D _monsterCollider;

    public int HorizontalDir { private set; get; } //좌우판정용 
    
    //Patrol / Return 관련 변수
    private Vector2 _spawnPos;
    private Vector2 _patrolPos;
    private Vector2 _leftPatrolLimit;
    private Vector2 _rightPatrolLimit;

    private MovePattern _moveState = MovePattern.Patrol;

    //멈춤 관련 변수
    private bool _isPaused = false;
    private bool _isTimerPaused = false;
    private float _pauseTimer = 0f;
    private float _pauseTimerTarget = 0f;
    private bool _shouldFlipAfterPause = false; //PauseForSeconds 이후에 방향 전환되는지
    //더좋은방법 없을까


    private Transform _player;
    private Transform _headPivot; //몬3용도

    [SerializeField] private LayerMask platformLayer; //SerializeField없애야
    
    private bool _canChangeDirection = true;
    private float _directionCheckTimer = 0f;
    private readonly float _directionCheckDelay = 0.5f;
    private const float ReturnArrivalThreshold = 0.9f; //끝지점 도착 판정 거리
    
    // 원거리 몬스터 후퇴 관련 변수
    private Vector2 _retreatStartPos;
    private Vector2 _retreatTargetPos;
    private bool _hasReachedRetreatTarget = false;
    private const float RetreatDistance = 2f;            // 후퇴할 거리
    private const float CloseRangeThreshold = 3f;        // 플레이어가 가까이 있다고 판단하는 거리
    private const float FleeRangeThreshold = 4f;         // 도망을 멈추는 거리
    private const float RetreatArrivalThreshold = 0.2f;  // 후퇴 목표점 도착 판정
    private const float RetreatSpeedMultiplier = 3.0f;


    private void Awake()
    {
        _cm = GetComponent<CharacterMovement>();
        _monster = GetComponent<Monster>();
        _monsterCollider = GetComponent<Collider2D>();
    }
    
    Vector2 IMovement.Direction => new Vector2(HorizontalDir, 0f);

    private void Start()
    {
        HorizontalDir = 1;
        _spawnPos = transform.position;
        _patrolPos = _spawnPos + new Vector2(-_monster.Data.PatrolRange / 2f, 0);
        _leftPatrolLimit = _spawnPos + Vector2.left * (_monster.Data.PatrolRange / 2f);
        _rightPatrolLimit = _spawnPos + Vector2.right * (_monster.Data.PatrolRange / 2f);

        //플레이어 오브젝트 찾기
        _player = _monster.Player;

        if (_player == null)
            _player = GameObject.FindWithTag("Player")?.transform;
        
        // 비행몹일때: headPivot 찾기 
        if (_monster.Data.IsFlying && _player != null)
        {
            _headPivot = _player.Find("HeadPivot");
            if (_headPivot == null)
            {
                Debug.LogWarning("[MonsterMovementUnified] HeadPivot == null");
            }
        }
        
        if (_player == null)
        {
            Debug.LogError("[MonsterMovement] Player not found");
        }

    }

    private void Update()
    {
        if (_player == null) return;

        HandlePauseTimer();
        if (_isPaused) return;

        float dist = Vector2.Distance(transform.position, _player.position);

        // 상태 전환 로직
        HandleStateTransition(dist);

        switch (_moveState)
        {
            case MovePattern.Patrol:
                PatrolMove();
                break;
            case MovePattern.Aggro:
                AggroMove();
                break;
            case MovePattern.Return:
                ReturnMove();
                break;
            case MovePattern.Retreat:
                RetreatMove();
                break;
            case MovePattern.Flee:
                FleeMove();
                break;
        }
    }

    private void HandleStateTransition(float dist)
    {
        if (!_monster.Data.IsRanged)
        {
            if (_monster.isAggro && _moveState != MovePattern.Aggro)
            {
                _moveState = MovePattern.Aggro;
            }
            else if (!_monster.isAggro && _moveState == MovePattern.Aggro && dist >= _monster.Data.AggroReleaseRange)
            {
                _moveState = MovePattern.Return;
            }
            else if (_moveState == MovePattern.Return && _monster.isAggro)
            {
                _moveState = MovePattern.Aggro;
            }

            return;
        }

        switch (_moveState)
        {
            case MovePattern.Patrol:
                if (_monster.isAggro)
                {
                    _moveState = MovePattern.Aggro;
                }

                break;
            case MovePattern.Aggro:
                if (!_monster.isAggro && dist >= _monster.Data.AggroReleaseRange)
                {
                    _moveState = MovePattern.Return;
                }
                else if (dist <= CloseRangeThreshold)
                {
                    StartRetreat();
                }
                break;
            case MovePattern.Retreat:
                if (_hasReachedRetreatTarget)
                {
                    _moveState = MovePattern.Flee;
                }
                break;
            case MovePattern.Flee:
                // 플레이어가 충분히 멀어지면 일반 aggro 상태로 복귀
                if (dist >= FleeRangeThreshold)
                {
                    _moveState = MovePattern.Aggro;
                }
                // aggro가 해제되면 복귀 상태로
                else if (!_monster.isAggro && dist >= _monster.Data.AggroReleaseRange)
                {
                    _moveState = MovePattern.Return;
                }
                break;
            case MovePattern.Return:
                if (_monster.isAggro)
                {
                    _moveState = MovePattern.Aggro;
                }
                break;
        }
    }

    private void StartRetreat()
    {
        _moveState = MovePattern.Retreat;
        _retreatStartPos = transform.position;
        _hasReachedRetreatTarget = false;
        
        // 플레이어 반대 방향으로 후퇴 목표점 설정
        Vector2 retreatDirection = _player.position.x > transform.position.x ? Vector2.left : Vector2.right;
        _retreatTargetPos = _retreatStartPos + retreatDirection * RetreatDistance;
        
        // 플레이어를 바라보는 방향을 설정 (후퇴하기 전에)
        HorizontalDir = _player.position.x > transform.position.x ? 1 : -1;
    }
    
    private void RetreatMove()
    {
        // 후퇴 목표점까지의 방향 계산
        Vector2 retreatDir = (_retreatTargetPos - (Vector2)transform.position).normalized;
        
        // 플레이어를 바라보는 방향은 유지 (후퇴하면서도 플레이어를 봄)
        HorizontalDir = _player.position.x > transform.position.x ? 1 : -1;
        
        // 지상몬스터인 경우 낭떠러지 체크
        if (!_monster.Data.IsFlying && !CheckGroundAhead())
        {
            // 후퇴할 수 없는 상황이면 즉시 도망 상태로 전환
            _hasReachedRetreatTarget = true;
            return;
        }
        
        // 후퇴 이동
        if (_monster.Data.IsFlying)
        {
            _cm.Move(retreatDir * RetreatSpeedMultiplier);
        }
        else
        {
            // 지상 몬스터는 좌우로만 이동
            HorizontalDir = retreatDir.x < 0 ? -1 : 1;
            _cm.Move(Vector2.right * HorizontalDir * RetreatSpeedMultiplier);
        }
        
        // 후퇴 목표점 도달 체크
        float distToTarget = Vector2.Distance(transform.position, _retreatTargetPos);
        if (distToTarget <= RetreatArrivalThreshold)
        {
            _hasReachedRetreatTarget = true;
        }
    }
    
    private void FleeMove()
    {
        // 플레이어 반대 방향으로 도망
        Vector2 fleeDirection;
        
        if (_monster.Data.IsFlying)
        {
            // 비행 몬스터는 플레이어 반대 방향으로 도망
            fleeDirection = ((Vector2)transform.position - (Vector2)_player.position).normalized;
            HorizontalDir = fleeDirection.x < 0 ? -1 : 1;
        }
        else
        {
            // 지상 몬스터는 좌우로만 도망
            HorizontalDir = _player.position.x > transform.position.x ? -1 : 1;
            fleeDirection = Vector2.right * HorizontalDir;
            
            // 지상몬스터: 발판 있으면 떨어지지 않게 멈추기 
            if (!CheckGroundAhead())
            {
                _cm.Move(Vector2.zero);
                return;
            }
        }
        
        _cm.Move(fleeDirection);
    }
    
    public void ChangeMovePattern(MovePattern pattern)
    {
        _moveState = pattern;
    }

    private void PatrolMove()
    {
        if (CheckWallAhead() || (!_monster.Data.IsFlying && !CheckGroundAhead()))
        {
            if (!_isTimerPaused)
            {
                PauseForSeconds(_monster.Data.PausedTime, true);
            }
            return;
        }


        // 이동 적용
        _cm.Move(GetDirection());

        if ((HorizontalDir == 1 && transform.position.x >= _rightPatrolLimit.x) ||
            (HorizontalDir == -1 && transform.position.x <= _leftPatrolLimit.x))
        {
            HorizontalDir *= -1;
        }
    }


    private void AggroMove()
    {
        if (_canChangeDirection)
        {
            _directionCheckTimer -= Time.deltaTime;
            if (_directionCheckTimer <= 0f)
            {
                _directionCheckTimer = _directionCheckDelay;
                
                //지상몬스터: 좌우 방향만 결정
                if (!_monster.Data.IsFlying)
                {
                    HorizontalDir = _player.position.x > transform.position.x ? 1 : -1;
                }
            }
        }
        
        //플레이어가 공격범위 안에 있고, 플레이어가 움직이지 않으면 몬스터도 멈춰서 때리게
        if (_monster.IsPlayerInShortRange() && !IsPlayerMoving())
        {
            _cm.Move(Vector2.zero);
            return;
        }
        
        // 지상몬스터: 발판 있으면 떨어지지 않게 멈추기 
        if (!_monster.Data.IsFlying && !CheckGroundAhead())
        {
            _cm.Move(Vector2.zero);
            return;
        }
        
        // 움직임 적용
        _cm.Move(GetDirection());
    }


    private void ReturnMove()
    {
        _cm.Move(GetDirection());

        float dist = Vector2.Distance(transform.position, _spawnPos);
        
        if (dist <= ReturnArrivalThreshold)
        {
            _spawnPos = transform.position;
            _patrolPos = _spawnPos + new Vector2(-_monster.Data.PatrolRange / 2f, 0);
            HorizontalDir = 1;
            _isPaused = false;
            _moveState = MovePattern.Patrol;

            Debug.Log("ReturnMove 완료, Patrol 시작");
        }
    }

    public void SetPaused(bool paused)
    {
        _isPaused = paused;

        if (paused)
            _cm.Move(Vector2.zero);
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
        _cm.Move(Vector2.zero);

        if (_pauseTimer >= _pauseTimerTarget)
        {
            _isTimerPaused = false;
            SetPaused(false);

            if (_shouldFlipAfterPause)
            {
                HorizontalDir *= -1;
                _shouldFlipAfterPause = false;
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

        Vector2 checkPos = (Vector2)transform.position + new Vector2(HorizontalDir * 0.5f, 0);
        float checkDistance = 0.5f;

        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.right * HorizontalDir, checkDistance, platformLayer);
        return hit.collider != null;
    }
    
    /*public Vector2 GetDirection()
    {
        return _monster.Data.IsFlying ? _directionToPlayer : Vector2.right * _dir;
    }*/
    
    /// <summary>
    /// HorizontalDir: 몬스터가 바라보는 방향(int) 저장
    /// </summary>
    /// <returns>몬스터가 바라보는 방향 (2d벡터)</returns>
    private Vector2 GetDirection()
    {
        //비행몹(몬3)
        if (_monster.Data.IsFlying)
        {
            switch (_moveState)
            {
                case MovePattern.Aggro:
                {
                    //HeadPivot보다 살짝 아래 따라가게
                    Vector2 dirVec = ((Vector2)_headPivot.position - (Vector2)transform.position + new Vector2(0f, -0.1f)).normalized;
                    HorizontalDir = dirVec.x < 0 ? -1 : 1;
                    return dirVec;
                }
                case MovePattern.Return:
                {
                    Vector2 dirVec = (_spawnPos - (Vector2)transform.position).normalized;
                    HorizontalDir = dirVec.x < 0 ? -1 : 1;
                    return dirVec;
                }
                case MovePattern.Patrol:
                    return Vector2.right * HorizontalDir;
            }
        }
        else
        {
            switch (_moveState)
            {
                case MovePattern.Aggro:
                {
                    return Vector2.right * HorizontalDir;
                }
                case MovePattern.Return:
                {
                    Vector2 dirVec = (_spawnPos - (Vector2)transform.position).normalized;
                    HorizontalDir = dirVec.x < 0 ? -1 : 1;
                    return Vector2.right * HorizontalDir;
                }
                case MovePattern.Patrol:
                    return Vector2.right * HorizontalDir;
            }
        }

        return Vector2.zero;
    }

    
    public void LockDirection(bool lockDir)
    {
        _canChangeDirection = !lockDir;
    }

    //플레이어 멈춰있는지 체크 (이동속도로)
    private bool IsPlayerMoving()
    {
        if (_player == null) return false;

        Rigidbody2D rb = _player.GetComponent<Rigidbody2D>();
        if (rb == null) return true;

        return rb.velocity.magnitude > 0.1f;
    }

}