using UnityEngine;
using GameAbilitySystem;

public abstract class Monster : MonoBehaviour, IAbilitySystem
{
    public AbilitySystem asc { get; private set; }

    [SerializeField] private MonsterSO monsterData;
    public MonsterSO Data => monsterData;

    [SerializeField] private string abilitySystemPath = "";
    private SpriteRenderer spriteRenderer;
    private float prevHp;
    
    protected bool isDead = false;
    
    public bool isAggro { get; private set; } = false; // hp바 띄우는 것 때문에 넣어둠

    private MonsterMovement _movement;
    private Transform player;
    public Transform Player => player;

    private void Awake()
    {
        // ASC 초기화
        asc = new AbilitySystem();
        asc.SetSceneState(this.gameObject);
        asc.Init(abilitySystemPath);
        asc.GrantAllAbilities();

        prevHp = asc.Attribute.Attributes["HP"].CurrentValue.Value;
        spriteRenderer = GetComponent<SpriteRenderer>();
        _movement = GetComponent<MonsterMovement>(); 
        player = GameObject.FindWithTag("Player")?.transform;
        
        var playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            var monsterCollider = GetComponent<Collider2D>();
            var playerCollider = playerObj.GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(monsterCollider, playerCollider, true);
        }
    }

    private void Update()
    {
        float currHp = asc.Attribute.Attributes["HP"].CurrentValue.Value;
        if (currHp < prevHp)
            StartCoroutine(Flash()); // 공격받았을 때 반짝 이펙트
        prevHp = currHp;

        // 사망 처리
        if (currHp <= 0f)
        {
            Die();
        }
        
        if (player == null) return;
        float dist = Vector2.Distance(transform.position, player.position);
        bool inSight = IsPlayerInSight();

        if (dist <= Data.AggroRange && inSight && monsterData.IsTriggerAttack)
        {
            EnterShortAttackRange();
            isAggro = true;
        }
        
        // 근거리 공격 조건 체크
        if (isAggro && IsPlayerInShortRange())
        {
            EnterShortAttackRange();
        }
        
        // 원거리 공격 조건 체크
        if (isAggro && IsPlayerInLongRange())
        {
            // Debug.Log("원거리 공격 가능 범위에 진입");
            EnterLongAttackRange();
        }
        
        if (dist <= Data.AggroRange && inSight)
        {
            isAggro = true;
        }
        else if (isAggro)
        {
            if (dist >= Data.AggroReleaseRange)
            {
                isAggro = false;
                _movement.ChangeMovePattern(MovePattern.Return);
            }
        }
        
    }




    protected abstract void EnterShortAttackRange();
    protected abstract void EnterLongAttackRange();
    
    // 플레이어가 시야 안에 들어오는지 체크 (부채꼴 범위)
    public bool IsPlayerInSight()
    {
        if (player == null) return false;

        Vector2 toPlayer = (player.position - transform.position);
        float distanceToPlayer = toPlayer.magnitude;

        if (distanceToPlayer > Data.AggroRange)
            return false;

        Vector2 toPlayerNormalized = toPlayer.normalized;

        float angleToPlayer = Mathf.Atan2(toPlayerNormalized.y, toPlayerNormalized.x) * Mathf.Rad2Deg;
        angleToPlayer = NormalizeAngle(angleToPlayer);

        float startAngle = Data.ViewStartAngle;
        float viewAngle = Data.ViewSight;

        int dir = _movement != null ? _movement.HorizontalDir : 1;

        if (dir == -1)
        {
            startAngle = 180f - (startAngle + viewAngle);
        }
        startAngle = NormalizeAngle(startAngle);
        float endAngle = NormalizeAngle(startAngle + viewAngle);

        if (!IsAngleInRange(angleToPlayer, startAngle, endAngle))
            return false;

        /*
        // 장애물 체크
        RaycastHit2D hit = Physics2D.Raycast(transform.position, toPlayerNormalized, distanceToPlayer, LayerMask.GetMask("Platform"));
        if (hit.collider != null)
            return false;
        */
        return true;
    }
    

    private float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle < 0f) angle += 360f;
        return angle;
    }

    private bool IsAngleInRange(float angle, float start, float end)
    {
        if (start < end)
            return angle >= start && angle <= end;
        else
            return angle >= start || angle <= end;
    }
    
    /*
    // 앞에 벽 있는지 체크(어그로 해제용)
    private bool IsWallInFront()
    {
        Vector2 origin = transform.position;
        Vector2 target = player.position;
        Vector2 direction = target - origin;
        float distance = direction.magnitude;
        
        RaycastHit2D hit = Physics2D.Raycast(origin, direction.normalized, distance, LayerMask.GetMask("Platform"));
        return hit.collider != null;
    }
    */
    
    
    // 플레이어가 근거리 공격 범위 안에 들어오는지 체크 (사각 범위)
    public bool IsPlayerInShortRange()
    {
        if (player == null) return false;

        float detectX = Data.DetectShortRangeX;
        float detectY = Data.DetectShortRangeY;
        int facingDir = _movement != null ? _movement.HorizontalDir : 1;

        Vector2 offset = new Vector2(Data.DetectShortOffset.x * facingDir, Data.DetectShortOffset.y);
        Vector2 origin = (Vector2)transform.position + offset;

        Collider2D[] hits = Physics2D.OverlapBoxAll(origin, new Vector2(detectX, detectY), 0f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }
    
    // 원거리 공격 범위 감지 (부채꼴)
    public bool IsPlayerInLongRange()
    {
        if (player == null) return false;

        // 플레이어까지 벡터 및 거리 계산
        Vector2 toPlayer = (player.position - transform.position);
        float distanceToPlayer = toPlayer.magnitude;

        // 원거리 사거리 체크
        if (distanceToPlayer > Data.DetectLongRange)
            return false;

        // 방향 벡터 정규화
        Vector2 toPlayerNormalized = toPlayer.normalized;

        // 플레이어 방향 각도 계산
        float angleToPlayer = Mathf.Atan2(toPlayerNormalized.y, toPlayerNormalized.x) * Mathf.Rad2Deg;
        angleToPlayer = NormalizeAngle(angleToPlayer);

        // SO에서 가져온 원거리 시작각 / 시야각
        float startAngle = Data.DetectLongStartAngle;
        float viewAngle = Data.DetectLongSight;

        // 바라보는 방향(facingDir)에 따라 시야 각도 보정
        int dir = _movement != null ? _movement.HorizontalDir : 1;
        if (dir == -1)
        {
            startAngle = 180f - (startAngle + viewAngle);
        }
        startAngle = NormalizeAngle(startAngle);
        float endAngle = NormalizeAngle(startAngle + viewAngle);

        // 각도 범위 안에 있는지 확인
        if (!IsAngleInRange(angleToPlayer, startAngle, endAngle))
            return false;

        /*
        // 원거리 감지 시 장애물 체크를 하고 싶다면 활성화
        RaycastHit2D hit = Physics2D.Raycast(transform.position, toPlayerNormalized, distanceToPlayer, LayerMask.GetMask("Platform"));
        if (hit.collider != null)
            return false;
        */

        return true;
    }


    public System.Collections.IEnumerator Flash()
    {
        var prev = spriteRenderer.color;
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = prev;
    }

    protected virtual void Die()
    {
        Debug.Log("[Monster] 처치");
        isDead = true;

        float spacing = 0.5f; 
        float startX = -(monsterData.DropCount - 1) * spacing * 0.5f;
        for (int i = 0; i < monsterData.DropCount; i++)
        {
            Vector3 spawnPos = transform.position + new Vector3(startX + i * spacing, 0f, 0f);
            Instantiate(monsterData.HonbulPrefab, spawnPos, Quaternion.identity);
        }

        Destroy(gameObject);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (monsterData == null) return;

        int facingDir = Application.isPlaying && _movement != null ? _movement.HorizontalDir : 1;

        // [근거리 공격 인식 범위] - 붉은색
        Vector2 offset = new Vector2(monsterData.DetectShortOffset.x * facingDir, monsterData.DetectShortOffset.y);
        Vector2 origin = (Vector2)transform.position + offset;

        Gizmos.color = new Color(1f, 0.3f, 0.2f, 0.4f);
        Gizmos.DrawCube(origin, new Vector3(monsterData.DetectShortRangeX, monsterData.DetectShortRangeY, 0.1f));

        // [기본 시야 범위] - 노란색
        float startAngle = monsterData.ViewStartAngle;
        float viewAngle = monsterData.ViewSight;

        if (facingDir == -1)
        {
            startAngle = 180f - (startAngle + viewAngle);
        }
        startAngle %= 360f;
        UnityEditor.Handles.color = new Color(1f, 1f, 0f, 0.25f);
        UnityEditor.Handles.DrawSolidArc(transform.position, Vector3.forward,
            Quaternion.Euler(0, 0, startAngle) * Vector3.right, viewAngle, monsterData.AggroRange);

            // [원거리 공격 범위] - 파란색
            float longStartAngle = monsterData.DetectLongStartAngle;
            float longViewAngle = monsterData.DetectLongSight;

            if (facingDir == -1)
            {
                longStartAngle = 180f - (longStartAngle + longViewAngle);
            }
            longStartAngle %= 360f;
            UnityEditor.Handles.color = new Color(0f, 0.5f, 1f, 0.25f); // 파란색 반투명
            UnityEditor.Handles.DrawSolidArc(transform.position, Vector3.forward, 
                Quaternion.Euler(0, 0, longStartAngle) * Vector3.right, longViewAngle, monsterData.DetectLongRange);
        
        
    }
#endif

}
