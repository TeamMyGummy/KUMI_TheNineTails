using UnityEngine;
using UltEvents;
using System.Collections;

/// <summary>
/// 특정 영역에 진입하고 나갈 때 연출 시퀀스를 재생하는 트리거입니다.
/// FSM(상태 머신) 기반으로 동작하며, 최초 한 번만 메인 시퀀스를 실행하는 기능을 포함합니다.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class SectionTrigger : MonoBehaviour
{
    // 내부 상태를 정의하는 FSM 상태 열거형
    private enum ZoneState
    {
        Idle,           // 플레이어가 밖에 있고 대기 중인 상태
        PreEnter,       // 진입 연출 (Pre-Sequence) 진행 중
        Active,         // 플레이어가 존 내부에 완전히 진입한 상태
        PostExit        // 퇴장 연출 (Post-Sequence) 진행 중
    }

    [Header("Configuration")]
    [SerializeField]
    private string playerTag = "Player";
    [Tooltip("체크하면 메인 연출 시퀀스가 최초 한 번만 실행됩니다.")]
    [SerializeField]
    private bool triggerOnce = true;

    [Header("Cinematic Sequences")]
    [Tooltip("Pre-Enter 연출의 지속 시간입니다. 실제 연출(타임라인, 애니메이션 등) 길이에 맞춰 조절하세요.")]
    [SerializeField]
    private float preEnterSequenceDuration = 2.0f;
    [Tooltip("Post-Exit 연출의 지속 시간입니다.")]
    [SerializeField]
    private float postExitSequenceDuration = 2.0f;

    [Header("Events")]
    [Space(10)]
    [Tooltip("플레이어가 영역에 진입하는 순간 호출됩니다. (Pre-Enter 연출 시작 전)")]
    public UltEvent onPreEnterStart;

    [Tooltip("Pre-Enter 연출이 끝난 후 호출됩니다. (본게임 로직 시작 시점)")]
    public UltEvent onMainGameplayStart;

    [Tooltip("플레이어가 영역을 나간 후, Post-Exit 연출이 시작될 때 호출됩니다.")]
    public UltEvent onPostExitStart;

    [Tooltip("Post-Exit 연출이 모두 끝난 후 호출됩니다.")]
    public UltEvent onPostExitEnd;


    private ZoneState currentState = ZoneState.Idle;
    private Collider2D zoneCollider;
    private bool hasBeenTriggered = false;

    private void Awake()
    {
        zoneCollider = GetComponent<Collider2D>();
        if (!zoneCollider.isTrigger)
        {
            zoneCollider.isTrigger = true;
            Debug.LogWarning($"CinematicZoneTrigger on '{gameObject.name}': Collider2D was not set as a trigger. It has been set automatically.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어가 아니고, 상태가 Idle이 아니면 무시
        if (!other.CompareTag(playerTag) || currentState != ZoneState.Idle)
        {
            return;
        }

        // 한 번만 실행 옵션이 켜져 있고, 이미 실행되었다면 무시
        if (triggerOnce && hasBeenTriggered)
        {
            Debug.Log($"Zone '{gameObject.name}' has already been triggered.");
            return;
        }

        // 진입 시퀀스 시작
        StartCoroutine(EnterSequence());
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 플레이어가 아니고, 상태가 Active가 아니면 무시
        if (!other.CompareTag(playerTag) || currentState != ZoneState.Active)
        {
            return;
        }
        
        // 퇴장 시퀀스 시작
        StartCoroutine(ExitSequence());
    }

    /// <summary>
    /// 진입 시퀀스를 처리하는 코루틴
    /// </summary>
    private IEnumerator EnterSequence()
    {
        // 1. 상태 변경 및 Pre-Enter 이벤트 호출
        currentState = ZoneState.PreEnter;
        Debug.Log("State: PreEnter - Pre-Enter sequence started.");
        onPreEnterStart?.Invoke();
        hasBeenTriggered = true;

        // --- 여기에 Pre-Enter 연출 로직을 넣으세요. (예: 카메라 무빙, 플레이어 조작 제한, UI 등장) ---
        // 예시: 지정된 시간만큼 대기
        yield return new WaitForSeconds(preEnterSequenceDuration);
        // --- 연출 종료 ---

        // 2. 상태 변경 및 본게임 진입 이벤트 호출
        currentState = ZoneState.Active;
        Debug.Log("State: Active - Main gameplay started.");
        onMainGameplayStart?.Invoke();
    }

    /// <summary>
    /// 퇴장 시퀀스를 처리하는 코루틴
    /// </summary>
    private IEnumerator ExitSequence()
    {
        // 1. 상태 변경 및 Post-Exit 이벤트 호출
        currentState = ZoneState.PostExit;
        Debug.Log("State: PostExit - Post-Exit sequence started.");
        onPostExitStart?.Invoke();

        // --- 여기에 Post-Exit 연출 로직을 넣으세요. (예: 문 닫히는 애니메이션, 다음 구역 암시) ---
        // 예시: 지정된 시간만큼 대기
        yield return new WaitForSeconds(postExitSequenceDuration);
        // --- 연출 종료 ---

        // 2. 상태 변경 및 최종 이벤트 호출
        currentState = ZoneState.Idle;
        Debug.Log("State: Idle - Zone sequence complete.");
        onPostExitEnd?.Invoke();
    }
    
    // 기즈모를 그려 씬 뷰에서 영역을 쉽게 식별하도록 합니다.
    private void OnDrawGizmos()
    {
        if (zoneCollider == null) zoneCollider = GetComponent<Collider2D>();

        Color gizmoColor = Color.cyan;
        gizmoColor.a = 0.3f;
        Gizmos.color = gizmoColor;

        Gizmos.matrix = transform.localToWorldMatrix;
        
        if (zoneCollider is BoxCollider2D box)
        {
            Gizmos.DrawCube(box.offset, box.size);
        }
        else if(zoneCollider is CircleCollider2D circle)
        {
            Gizmos.DrawSphere(circle.offset, circle.radius);
        }
    }
}