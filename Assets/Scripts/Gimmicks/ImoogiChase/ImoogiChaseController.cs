using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
public sealed class ImoogiChaseController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;        // 비우면 태그 Player 자동 탐색
    [SerializeField] private Rigidbody2D rb;          // Kinematic 권장
    [SerializeField] private Collider2D headTrigger;  // 머리(Trigger) - 플레이어 닿으면 잡힘

    [Header("Speed")]
    [SerializeField] private float baseSpeed;
    [SerializeField] private float minSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float accel;        // 가/감속 부드럽게

    [Header("Rubberbanding")]
    [SerializeField] private float boostDist;    // 멀어지면 가속
    [SerializeField] private float slowDist;    // 너무 붙으면 감속

    [Header("Rail (연출용, 지금은 OFF 권장)")]
    [SerializeField] private bool lockYToRail = false;
    [SerializeField] private float railY = 0f;
    [SerializeField] private float railLerp = 8f;

    [Header("State")]
    [SerializeField] private bool chasing = false;      // StartChase()로 true
    [SerializeField] private bool faceRight = true;     // 좌우 반전

    [Header("Events")]
    public UnityEvent onCaughtPlayer;                   // 세이브 복귀 등
    public UnityEvent<bool> onChaseToggle;              // 추격 on/off 알림

    float _curSpeed;

    void Reset()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        if (!player) TryAutoFindPlayer();
        if (headTrigger) headTrigger.isTrigger = true;
        _curSpeed = baseSpeed;
    }

    void Update()
    {
        // 움직임이 없을 때 로그로 상태 확인
        if (!chasing || !player)
        {
            // 필요할 때만 켜서 확인
            // Debug.Log($"[Imoogi] idle: chasing={chasing}, player={(player!=null)}");
            return;
        }

        // 목표 속도 계산(거리 기반)
        float dist = Vector2.Distance(transform.position, player.position);
        float target = baseSpeed;
        if (dist > boostDist)      target *= 1.10f;
        else if (dist < slowDist)  target *= 0.90f;
        target = Mathf.Clamp(target, minSpeed, maxSpeed);

        // 가/감속
        _curSpeed = Mathf.MoveTowards(_curSpeed, target, accel * Time.deltaTime);

        // 방향 계산
        Vector2 dir = ((Vector2)player.position - rb.position).normalized;

        // 이동 목표
        Vector2 desired = rb.position + dir * (_curSpeed * Time.deltaTime);

        // (연출용) 레일 Y 고정
        if (lockYToRail)
            desired.y = Mathf.Lerp(rb.position.y, railY, Time.deltaTime * railLerp);

        rb.MovePosition(desired);

        // 좌우 반전(필요시)
        if (faceRight && dir.x < -0.01f) Flip(false);
        else if (!faceRight && dir.x >  0.01f) Flip(true);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[Imoogi] TriggerEnter with {other.name} (tag={other.tag})");

        if (!chasing) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("[Imoogi] Player caught!");
            onCaughtPlayer?.Invoke(); // 세이브 복귀 등
        }
    }


    // 외부에서 호출
    public void StartChase()
    {
        if (chasing) return;
        chasing = true;
        onChaseToggle?.Invoke(true);
        // Debug.Log("[Imoogi] StartChase()");
    }

    public void StopChase()
    {
        if (!chasing) return;
        chasing = false;
        onChaseToggle?.Invoke(false);
        // Debug.Log("[Imoogi] StopChase()");
    }

    public void SetRailY(float y) => railY = y;

    void TryAutoFindPlayer()
    {
        var pl = GameObject.FindWithTag("Player");
        if (pl) player = pl.transform;
    }

    void Flip(bool toRight)
    {
        faceRight = toRight;
        var s = transform.localScale;
        s.x = Mathf.Abs(s.x) * (toRight ? 1f : -1f);
        transform.localScale = s;
    }

    #if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;   Gizmos.DrawWireSphere(transform.position, slowDist);
            Gizmos.color = Color.magenta;Gizmos.DrawWireSphere(transform.position, boostDist);
        }
    #endif
}
