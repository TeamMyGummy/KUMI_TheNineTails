using UnityEngine;
using GameAbilitySystem;

public abstract class Monster : MonoBehaviour
{
    public AbilitySystem asc { get; private set; }

    [SerializeField] private MonsterSO monsterData;
    public MonsterSO Data => monsterData;

    [SerializeField] private string abilitySystemPath = "";
    private SpriteRenderer spriteRenderer;
    private float prevHp;
    
    private bool isDead = false;
    
    public bool isAggro { get; private set; } = false; //hp바 띄우는 것 때문에 넣어둠

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
            StartCoroutine(Flash()); //공격받았을 때 반짝 이펙트
        prevHp = currHp;

        // 사망 처리
        if (currHp <= 0f)
        {
            Die();
        }

        if (player == null) return;
        
        


        float dist = Vector2.Distance(transform.position, player.position);
        bool inSight = IsPlayerInSight();
        //플레이어가 시야에 들어오면 어그로 처리
        if (dist <= Data.AggroRange && inSight && monsterData.IsTriggerAttack)
        {
            EnterAttackRange();
        }
        else if (dist <= Data.AggroRange && inSight)
        {
            isAggro = true;
        }
        else if (isAggro && dist >= Data.AggroReleaseRange)
        {
            isAggro = false;
        }

        //어그로 걸려 있고, 공격범위 안에 들어오면 Ability 실행
        if (isAggro && IsPlayerInAttackRange() && !monsterData.IsTriggerAttack)
        {
            EnterAttackRange();
        }
    }

    protected abstract void EnterAttackRange();

    //플레이어가 시야 안에 들어오는지 체크 (부채꼴 범위)
    //---------------------------------------------------
    public bool IsPlayerInSight()
    {
        if (player == null) return false;

        Vector2 toPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;
        angleToPlayer = NormalizeAngle(angleToPlayer);

        float startAngle = Data.ViewStartAngle;
        float viewAngle = Data.ViewSight;
        
        //int --> Vector2 수정
        int dir = _movement != null ? _movement.HorizontalDir : 1;

        if (dir == -1)
        {
            startAngle = 180f - (startAngle + viewAngle);
        }

        startAngle = NormalizeAngle(startAngle);
        float endAngle = NormalizeAngle(startAngle + viewAngle);

        return IsAngleInRange(angleToPlayer, startAngle, endAngle);
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
    //---------------------------------------------------

    
    
    //플레이어가 공격 범위 안에 들어오는지 체크 (사각 범위)
    //---------------------------------------------------
    private bool IsPlayerInAttackRange()
    {
        if (player == null) return false;

        float detectX = Data.DetectRangeX;
        float detectY = Data.DetectRangeY;
        int facingDir = _movement != null ? _movement.HorizontalDir : 1;

        Vector2 offset = new Vector2(Data.DetectOffset.x * facingDir, Data.DetectOffset.y);
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


    //---------------------------------------------------

    //피격 시 반짝 이펙트 / 공격 직전 반짝 이펙트
    public System.Collections.IEnumerator Flash()
    {
        var prev = spriteRenderer.color;
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = prev;
    }

    protected void Die()
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

        // 공격 인식 범위
        Vector2 offset = new Vector2(monsterData.DetectOffset.x * facingDir, monsterData.DetectOffset.y);
        Vector2 origin = (Vector2)transform.position + offset;

        Gizmos.color = new Color(1f, 0.3f, 0.2f, 0.4f);
        Gizmos.DrawCube(origin, new Vector3(monsterData.DetectRangeX, monsterData.DetectRangeY, 0.1f));

        // 시야 범위
        float startAngle = monsterData.ViewStartAngle;
        float viewAngle = monsterData.ViewSight;

        if (facingDir == -1)
        {
            startAngle = 180f - (startAngle + viewAngle);
        }
        startAngle %= 360f;
        UnityEditor.Handles.color = new Color(1f, 1f, 0f, 0.25f);
        UnityEditor.Handles.DrawSolidArc(transform.position, Vector3.forward, Quaternion.Euler(0, 0, startAngle) * Vector3.right, viewAngle, monsterData.AggroRange);
    }
#endif


}
