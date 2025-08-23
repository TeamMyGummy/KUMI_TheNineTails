using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

// 캐릭터 중심 고정 위치 음파공격 클래스
public class SonicWave : MonoBehaviour
{
    [Header("Wave Settings")]
    [SerializeField] private float duration = 1.5f;
    [SerializeField] private float maxRadius = 5f;
    [SerializeField] private float waveAngle = 90f; // 음파 각도
    [SerializeField] private Vector3 offset = Vector3.zero;
    
    [Header("Animation Curves")]
    [SerializeField] private AnimationCurve radiusCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AnimationCurve alphaCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    
    [Header("Visual Effects")]
    [SerializeField] private LineRenderer waveRenderer;
    [SerializeField] private int visualResolution = 30;
    [SerializeField] private Color waveColor = Color.cyan;
    [SerializeField] private float lineWidth = 0.1f;
    
    [Header("Hit Detection")]
    [SerializeField] private LayerMask targetLayers = -1;
    [SerializeField] private bool penetrateTargets = true;
    [SerializeField] private string playerHitboxLayerName = "PlayerHitbox";
    [SerializeField] private string monsterLayerName = "Monster";
    
    private bool isActive = false;
    private Vector2 direction;
    private Vector3 centerPosition; // 음파 중심점 (캐릭터 위치 + 오프셋)
    private HashSet<Collider2D> hitTargets = new HashSet<Collider2D>();
    private Color originalColor;
    private Hitbox hitboxComponent; // 같은 GameObject의 Hitbox 컴포넌트
    private GameObject attacker; // 공격자 저장
    
    // 히트 감지용 변수들
    private float currentRadius = 0f;
    private float lastCheckedRadius = 0f;
    private const float HIT_CHECK_STEP = 0.2f; // 반지름 체크 간격

    private void Awake()
    {
        SetupComponents();
    }

    private void SetupComponents()
    {
        // Hitbox 컴포넌트 찾기
        hitboxComponent = GetComponent<Hitbox>();
        
        // Line Renderer 설정
        if (waveRenderer == null)
        {
            waveRenderer = gameObject.AddComponent<LineRenderer>();
        }
        
        waveRenderer.material = new Material(Shader.Find("Sprites/Default"));
        waveRenderer.material.color = waveColor;
        waveRenderer.startWidth = lineWidth;
        waveRenderer.endWidth = lineWidth;
        waveRenderer.useWorldSpace = true; // 월드 좌표 사용
        waveRenderer.positionCount = 0;
        
        originalColor = waveColor;
    }

    public void FireProjectile(GameObject actor, Vector2 direction)
    {
        this.direction = direction.normalized;
        this.centerPosition = actor.transform.position + offset;
        this.attacker = actor; // 공격자 저장
        transform.position = centerPosition; // 음파 중심을 캐릭터 위치로 고정
        
        // Hitbox 컴포넌트에 공격자 설정
        if (hitboxComponent != null)
        {
            hitboxComponent.SetAttacker(actor);
        }
        
        Fire().Forget();
    }

    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
    }

    private async UniTaskVoid Fire()
    {
        isActive = true;
        hitTargets.Clear();
        currentRadius = 0f;
        lastCheckedRadius = 0f;
        
        float timer = 0f;

        while (timer < duration && isActive)
        {
            float timeRatio = Mathf.Clamp01(timer / duration);
            
            // 반지름 확장
            float radiusProgress = radiusCurve.Evaluate(timeRatio);
            currentRadius = maxRadius * radiusProgress;
            
            // 히트 체크 (일정 간격마다)
            if (currentRadius - lastCheckedRadius >= HIT_CHECK_STEP)
            {
                CheckHitsInRange(lastCheckedRadius, currentRadius);
                lastCheckedRadius = currentRadius;
            }
            
            // 시각 효과 업데이트
            UpdateVisualEffects(currentRadius, timeRatio);

            await UniTask.Yield(PlayerLoopTiming.Update);
            timer += Time.deltaTime;
        }

        DestroyWave();
    }

    private void CheckHitsInRange(float innerRadius, float outerRadius)
    {
        // 현재 반지름 범위의 모든 콜라이더 검색
        Collider2D[] colliders = Physics2D.OverlapCircleAll(centerPosition, outerRadius, targetLayers);
        
        foreach (var collider in colliders)
        {
            // PlayerHitbox 레이어일 때 몬스터는 제외
            if (gameObject.layer == LayerMask.NameToLayer(playerHitboxLayerName))
            {
                if (collider.gameObject.layer == LayerMask.NameToLayer(monsterLayerName))
                    continue;
            }
            
            // 공격자 자신은 제외
            if (attacker != null && collider.gameObject == attacker)
                continue;
            
            // 이미 히트한 타겟이면 스킵 (관통하지 않는 경우)
            if (!penetrateTargets && hitTargets.Contains(collider))
                continue;
            
            Vector3 targetPos = collider.transform.position;
            float distanceFromCenter = Vector2.Distance(centerPosition, targetPos);
            
            // 오직 현재 호의 경계선 부분(outerRadius)에서만 히트 체크
            // innerRadius와 outerRadius 사이의 좁은 범위만 체크
            if (distanceFromCenter >= innerRadius && distanceFromCenter <= outerRadius)
            {
                // 음파 각도 범위 내에 있는지 체크
                if (IsInWaveRange(targetPos))
                {
                    ProcessHit(collider);
                }
            }
        }
    }

    private bool IsInWaveRange(Vector3 targetPosition)
    {
        Vector2 toTarget = (targetPosition - centerPosition).normalized;
        float angleToTarget = Vector2.Angle(direction, toTarget);
        
        return angleToTarget <= waveAngle * 0.5f;
    }

    private void ProcessHit(Collider2D target)
    {
        hitTargets.Add(target);
        
        Debug.Log($"{target.name}이 음파공격에 맞음!");
        
        // Hitbox 컴포넌트가 있다면 해당 로직 사용
        if (hitboxComponent != null)
        {
            // Hitbox의 OnTriggerEnter2D 로직을 직접 호출
            hitboxComponent.SendMessage("OnTriggerEnter2D", target, SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            // Hitbox가 없을 경우 기본 처리
            // 여기에 기본 데미지, 넉백 등의 로직 추가 가능
            Debug.Log("Hitbox 컴포넌트가 없어서 기본 처리됩니다.");
        }
    }

    private void UpdateVisualEffects(float radius, float timeRatio)
    {
        // 알파 페이드 효과
        float alpha = alphaCurve.Evaluate(timeRatio);
        Color currentColor = originalColor;
        currentColor.a = alpha;
        waveRenderer.material.color = currentColor;
        
        // 음파 모양 그리기
        DrawWaveShape(radius);
    }

    private void DrawWaveShape(float radius)
    {
        if (radius <= 0f)
        {
            waveRenderer.positionCount = 0;
            return;
        }

        List<Vector3> wavePoints = new List<Vector3>();
        float halfAngle = waveAngle * 0.5f * Mathf.Deg2Rad;
        
        // 호의 경계선만 그리기 (중심점 없이)
        for (int i = 0; i <= visualResolution; i++)
        {
            float angle = Mathf.Lerp(-halfAngle, halfAngle, (float)i / visualResolution);
            
            // direction을 기준으로 회전
            Vector2 baseDirection = new Vector2(direction.x, direction.y);
            Vector2 rotatedDirection = new Vector2(
                baseDirection.x * Mathf.Cos(angle) - baseDirection.y * Mathf.Sin(angle),
                baseDirection.x * Mathf.Sin(angle) + baseDirection.y * Mathf.Cos(angle)
            );
            
            Vector3 point = centerPosition + (Vector3)rotatedDirection * radius;
            wavePoints.Add(point);
        }
        
        waveRenderer.positionCount = wavePoints.Count;
        waveRenderer.SetPositions(wavePoints.ToArray());
    }

    private void DestroyWave()
    {
        isActive = false;
        ResourcesManager.Instance.Destroy(gameObject);
    }

    private void OnDisable()
    {
        isActive = false;
        hitTargets.Clear();
    }

    // 디버그용 기즈모
    private void OnDrawGizmosSelected()
    {
        if (!isActive) return;
        
        Gizmos.color = Color.yellow;
        
        // 현재 호의 경계선만 표시 (내부 원은 제거)
        // Gizmos.DrawWireCircle(centerPosition, currentRadius); // 제거
        
        // 음파 범위 시각화
        float halfAngle = waveAngle * 0.5f * Mathf.Deg2Rad;
        
        // 경계선 그리기
        Vector2 rightDir = new Vector2(
            direction.x * Mathf.Cos(-halfAngle) - direction.y * Mathf.Sin(-halfAngle),
            direction.x * Mathf.Sin(-halfAngle) + direction.y * Mathf.Cos(-halfAngle)
        );
        
        Vector2 leftDir = new Vector2(
            direction.x * Mathf.Cos(halfAngle) - direction.y * Mathf.Sin(halfAngle),
            direction.x * Mathf.Sin(halfAngle) + direction.y * Mathf.Cos(halfAngle)
        );
        
        Gizmos.color = Color.red;
        Gizmos.DrawRay(centerPosition, rightDir * currentRadius);
        Gizmos.DrawRay(centerPosition, leftDir * currentRadius);
        
        // 호 그리기
        Gizmos.color = Color.cyan;
        // 호의 경계선만 그리기
        Vector3 prevPoint = centerPosition + (Vector3)rightDir * currentRadius;
        
        for (int i = 1; i <= 20; i++)
        {
            float angle = Mathf.Lerp(-halfAngle, halfAngle, (float)i / 20);
            Vector2 rotatedDir = new Vector2(
                direction.x * Mathf.Cos(angle) - direction.y * Mathf.Sin(angle),
                direction.x * Mathf.Sin(angle) + direction.y * Mathf.Cos(angle)
            );
            Vector3 point = centerPosition + (Vector3)rotatedDir * currentRadius;
            Gizmos.DrawLine(prevPoint, point);
            prevPoint = point;
        }
    }
}