using UnityEngine;
using GameAbilitySystem;
using Spine.Unity;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;

public abstract class Monster : MonoBehaviour, IAbilitySystem
{
    public AbilitySystem asc { get; private set; }

    [SerializeField] private MonsterSO monsterData;
    public MonsterSO Data => monsterData;

    [SerializeField] private string abilitySystemPath = "";
    private SkeletonMecanim _skeletonMecanim;
    private float prevHp;
    private Coroutine _flashCoroutine;
    private Color _originalColor;
    
    protected bool isDead = false;
    
    protected int FacingDir
    {
        get
        {
            // _movement가 없으면 기본값 1(오른쪽)
            return _movement != null ? _movement.HorizontalDir : 1;
        }
    }
    
    public bool isAggro { get; private set; } = false; // hp바 띄우는 것 때문에 넣어둠
    
    protected MonsterMovement _movement;
    private Transform playerTransform;
    public Transform PlayerTransform => playerTransform;
    
    private Player _player;
    private LiverExtraction _liverExtraction; 

    private MeshRenderer _meshRenderer;
    private Material _originalMaterial;
    private Material _flashMaterial;
    private Coroutine _activeFlashCoroutine;
    
    private void Awake()
    {
        // ASC 초기화
        asc = new AbilitySystem();
        asc.SetSceneState(this.gameObject);
        asc.Init(abilitySystemPath);
        asc.GrantAllAbilities();

        prevHp = asc.Attribute.Attributes["HP"].CurrentValue.Value;
        _skeletonMecanim = GetComponent<SkeletonMecanim>();
        _movement = GetComponent<MonsterMovement>(); 
        playerTransform = GameObject.FindWithTag("Player")?.transform;
        _meshRenderer = GetComponent<MeshRenderer>();
        
        if (_skeletonMecanim != null)
        {
            _originalColor = _skeletonMecanim.Skeleton.GetColor();
        }
        
        var playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            _player = playerObj.GetComponent<Player>();
            
            var monsterCollider = GetComponent<Collider2D>();
            var playerCollider = playerObj.GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(monsterCollider, playerCollider, true);
        }
        
        //Flash 때문에 추가
        if (_meshRenderer != null)
        {
            _originalMaterial = _meshRenderer.material;
            Shader flashShader = Shader.Find("Custom/WhiteFlash"); 
            
            if (flashShader != null)
            {
                _flashMaterial = new Material(flashShader);
                
                // 원래 텍스처(몬스터 모양)를 복사해서 넣어줌
                if (_originalMaterial.HasProperty("_MainTex"))
                {
                    _flashMaterial.mainTexture = _originalMaterial.mainTexture;
                }
                
                // ColorFlash 쉐이더를 쓴다면 색상을 흰색으로 설정
                if (_flashMaterial.HasProperty("_FlashColor"))
                {
                    _flashMaterial.SetColor("_FlashColor", Color.white);
                }
            }
        }
    }

    private void Update()
    {
        float currHp = asc.Attribute.Attributes["HP"].CurrentValue.Value;
        if (currHp < prevHp)
        {
            if (_flashCoroutine != null)
            {
                StopCoroutine(_flashCoroutine);
            }

            if (!isDead)
                isAggro = true;
            
            _flashCoroutine = StartCoroutine(Flash());
            if (playerTransform != null)
            {
                _movement.HorizontalDir = playerTransform.position.x > transform.position.x ? 1 : -1;
            }
        }
        prevHp = currHp;

        // 사망 처리
        if (currHp <= 0f)
        {
            Die();
        }
        
        if (playerTransform == null) return;
        float dist = Vector2.Distance(transform.position, playerTransform.position);
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
            }
        }
        
    }
    
    //패링당했을때 호출
    public void OnParried()
    {
        if (!Data.IsParryable) return;
        
        if (_movement.CurrentState is ParriedState) return;

        Debug.Log($"{name} has been parried!");
        
        StartWhiteFlash(0.1f);
        _movement.EnterParriedState();
    }


    protected abstract void EnterShortAttackRange();
    protected abstract void EnterLongAttackRange();
    
    // 플레이어가 시야 안에 들어오는지 체크 (부채꼴 범위)
    public bool IsPlayerInSight(int rayCount = 5)
    {
        if (playerTransform == null) return false;

        Vector2 origin = (Vector2)transform.position + Data.ViewOffset;
        Vector2 toPlayer = (Vector2)playerTransform.position - origin;
        float distanceToPlayer = toPlayer.magnitude;
        if (distanceToPlayer > Data.AggroRange) return false;

        float startAngle = Data.ViewStartAngle;
        float viewAngle = Data.ViewSight;
        int dir = _movement != null ? _movement.HorizontalDir : 1;
        if (dir == -1) startAngle = 180f - (startAngle + viewAngle);
    
        for (int i = 0; i < rayCount; i++)
        {
            float t = i / (float)(rayCount - 1);
            float angle = startAngle + t * viewAngle;
            Vector2 dirVec = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            RaycastHit2D hit = Physics2D.Raycast(origin, dirVec, Data.AggroRange, LayerMask.GetMask("Player", "Platform"));
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }

        return false;
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
    
    
    // 플레이어가 근거리 공격 범위 안에 들어오는지 체크 (사각 범위)
    public virtual bool IsPlayerInShortRange()
    {
        if (playerTransform == null) return false;

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
        if (playerTransform == null) return false;

        Vector2 origin = (Vector2)transform.position + Data.DetectLongOffset;

        Vector2 toPlayer = (Vector2)playerTransform.position - origin;
        float distanceToPlayer = toPlayer.magnitude;

        if (distanceToPlayer > Data.DetectLongRange)
            return false;

        Vector2 toPlayerNormalized = toPlayer.normalized;
        float angleToPlayer = Mathf.Atan2(toPlayerNormalized.y, toPlayerNormalized.x) * Mathf.Rad2Deg;
        angleToPlayer = NormalizeAngle(angleToPlayer);

        float startAngle = Data.DetectLongStartAngle;
        float viewAngle = Data.DetectLongSight;

        int dir = _movement != null ? _movement.HorizontalDir : 1;
        if (dir == -1)
        {
            startAngle = 180f - (startAngle + viewAngle);
        }
        startAngle = NormalizeAngle(startAngle);
        float endAngle = NormalizeAngle(startAngle + viewAngle);

        if (!IsAngleInRange(angleToPlayer, startAngle, endAngle))
            return false;


        return true;
    }


    public void StartWhiteFlash(float duration = 0.15f)
    {
        // 기존에 돌던 코루틴이 있으면 끄고 새로 시작 (안전장치)
        if (_activeFlashCoroutine != null) StopCoroutine(_activeFlashCoroutine);
        _activeFlashCoroutine = StartCoroutine(FlashRoutine(duration));
    }

    private IEnumerator FlashRoutine(float duration)
    {
        if (_meshRenderer == null || _flashMaterial == null) yield break;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _meshRenderer.material = _flashMaterial;
            yield return null;
        }
        _meshRenderer.material = _originalMaterial;
        _activeFlashCoroutine = null;
    }
    
    public IEnumerator Flash()
    {
        StartWhiteFlash(0.1f);
        yield break;
    }
    
    /* 기존 패링당할 시 호출하던 파랑이되는코드
     public System.Collections.IEnumerator ParriedFlash()
    {
        if (_skeletonMecanim == null) yield break;

        Color prevColor = _skeletonMecanim.Skeleton.GetColor();
        _skeletonMecanim.Skeleton.SetColor(Color.blue);
        yield return new WaitForSeconds(0.2f);
        _skeletonMecanim.Skeleton.SetColor(prevColor);
    }*/


    protected virtual void Die()
    {
        Debug.Log("[Monster] 처치");
        isDead = true;

        int dropCount = monsterData.DropCount;

        if (_liverExtraction == null)
        {
            var player = GameObject.FindWithTag("Player")?.GetComponent<Player>();
            _liverExtraction = player?.ASC?.GetAbility(AbilityKey.LiverExtraction) as LiverExtraction;
        }

        if (_liverExtraction != null && _liverExtraction.IsUsingLiverExtraction)
        {
            dropCount *= 2;
            Debug.Log($"간 빼기로 처치, 혼불 드랍 수 {dropCount}개");
        }

        float spacing = 0.5f;
        float startX = -(dropCount - 1) * spacing * 0.5f;
        for (int i = 0; i < dropCount; i++)
        {
            Vector3 spawnPos = transform.position + new Vector3(startX + i * spacing, 0.3f, 0f);
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

        float startAngle = monsterData.ViewStartAngle;
        float viewAngle = monsterData.ViewSight;

        if (facingDir == -1)
        {
            startAngle = 180f - (startAngle + viewAngle);
        }
        startAngle %= 360f;

        Vector2 viewOrigin = (Vector2)transform.position + monsterData.ViewOffset; // 오프셋 적용
        UnityEditor.Handles.color = new Color(1f, 1f, 0f, 0.25f);
        UnityEditor.Handles.DrawSolidArc(viewOrigin, Vector3.forward,
            Quaternion.Euler(0, 0, startAngle) * Vector3.right, viewAngle, monsterData.AggroRange);
        
        
        // [원거리 공격 범위] - 파란색
        float longStartAngle = monsterData.DetectLongStartAngle;
        float longViewAngle = monsterData.DetectLongSight;

        if (facingDir == -1)
        {
            longStartAngle = 180f - (longStartAngle + longViewAngle);
        }
        longStartAngle %= 360f;

        Vector2 longOrigin = (Vector2)transform.position + monsterData.DetectLongOffset; // 오프셋 적용
        UnityEditor.Handles.color = new Color(0f, 0.5f, 1f, 0.25f);
        UnityEditor.Handles.DrawSolidArc(longOrigin, Vector3.forward,
            Quaternion.Euler(0, 0, longStartAngle) * Vector3.right, longViewAngle, monsterData.DetectLongRange);

        
    }
#endif

}
