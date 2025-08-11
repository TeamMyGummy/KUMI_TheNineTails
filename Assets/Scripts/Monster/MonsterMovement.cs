using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MovePattern
{
    Patrol,
    Aggro,
    Return
}

public class MonsterMovement : MonoBehaviour, IMovement
{
    private CharacterMovement _cm;
    private Monster _monster;
    private Collider2D _monsterCollider;

    public int HorizontalDir { private set; get; } //좌우판정용 
    private Vector2 _spawnPos;
    private Vector2 _patrolPos;
    private MovePattern _moveState = MovePattern.Patrol;

    private bool _isPaused = false;
    private float _pauseTimer = 0;
    
    private Transform _player;
    private Transform _headPivot; //몬3용도

    [SerializeField] private LayerMask platformLayer; //SerializeField없애야
    
    private bool _canChangeDirection = true;
    private float _directionCheckTimer = 0f;
    private readonly float _directionCheckDelay = 0.5f;
    private const float ReturnArrivalThreshold = 0.1f; //끝지점 도착 판정 거리


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

        float dist = Vector2.Distance(transform.position, _player.position);

        // 일시정지 상태 처리?
        if (_isPaused)
        {
            _cm.Move(Vector2.zero);
            return;
        }
        
        if (_monster.isAggro)
        {
            _moveState = MovePattern.Aggro;
        }
        else if (_moveState == MovePattern.Aggro && dist >= _monster.Data.AggroReleaseRange)
        {
            _moveState = MovePattern.Return;
        }

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
        }
    }
    
    public void ChangeMovePattern(MovePattern pattern)
    {
        _moveState = pattern;
    }
    
    private void PatrolMove()
    {
        if (_isPaused)
        {
            _pauseTimer += Time.deltaTime;
            if (_pauseTimer >= _monster.Data.PausedTime)
            {
                _isPaused = false;
                _pauseTimer = 0;
                HorizontalDir *= -1;
                _patrolPos = transform.position;
            }

            _cm.Move(Vector2.zero);
            return;
        }
        
        // 지상몬스터: 발판 있으면 떨어지지 않게 멈추기 
        if (!_monster.Data.IsFlying && !CheckGroundAhead())
        {
            _isPaused = true;
            _cm.Move(Vector2.zero);
            return;
        }
        
        //이동 적용
        _cm.Move(GetDirection());

        float movedDistance = Mathf.Abs(transform.position.x - _patrolPos.x);
        if (movedDistance >= _monster.Data.PatrolRange)
        {
            HorizontalDir *= -1;
            _patrolPos = transform.position;
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
        if (_monster.IsPlayerInAttackRange() && !IsPlayerMoving())
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

        // 도착 판정
        float dist = Vector2.Distance(transform.position, _spawnPos);
        if (dist <= ReturnArrivalThreshold)
        {
            _patrolPos = _spawnPos + new Vector2(-HorizontalDir * _monster.Data.PatrolRange / 2f, 0);
            
            _isPaused = false;
            _moveState = MovePattern.Patrol;
        }
    }
    
    
    public void SetPaused(bool paused)
    {
        _isPaused = paused;

        if (paused)
            _cm.Move(Vector2.zero);
    }
    
    public bool CheckGroundAhead()
    {
        if (_monster.Data.IsFlying) return true;
        
        Vector2 checkPos = (Vector2)transform.position + new Vector2(HorizontalDir * 0.8f, -0.2f);
        float checkDistance = 1.2f;

        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, checkDistance, platformLayer);
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
        if (_monster.Data.IsFlying)
        {
            switch (_moveState)
            {
                case MovePattern.Aggro:
                {
                    //HeadPivot보다 살짝 아래 따라가게
                    Vector2 dirVec = ((Vector2)_headPivot.position - (Vector2)transform.position + new Vector2(0f, -0.3f)).normalized;
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