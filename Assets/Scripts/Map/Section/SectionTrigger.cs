using UnityEngine;
using UltEvents;
using System.Collections;
using System.Collections.Generic; // List를 사용하기 위해 추가

/// <summary>
/// UltEvent와 지연 시간을 한 묶음으로 관리하는 데이터 클래스입니다.
/// </summary>
[System.Serializable]
public class DelayedUltEvent
{
    [Tooltip("인스펙터에서 알아보기 쉽도록 설명을 적어두세요.")]
    public string description;
    [Tooltip("이 액션이 시작되기까지의 지연 시간(초)입니다.")]
    public float delay;
    [Tooltip("지연 후 실행될 이벤트입니다.")]
    public UltEvent action;
}


/// <summary>
/// 특정 영역에 진입하고 나갈 때 연출 시퀀스를 재생하는 트리거입니다.
/// 하나의 트리거 시점에 여러 이벤트를 각기 다른 딜레이로 실행할 수 있습니다.
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
    [Tooltip("플레이어가 영역에 진입했을 때 개별 딜레이로 실행될 액션 목록입니다.")]
    public List<DelayedUltEvent> onPreEnterActions;

    [Tooltip("Pre-Enter 연출이 끝난 후 개별 딜레이로 실행될 액션 목록입니다.")]
    public List<DelayedUltEvent> onMainGameplayActions;

    [Tooltip("플레이어가 영역을 나갔을 때 개별 딜레이로 실행될 액션 목록입니다.")]
    public List<DelayedUltEvent> onPostExitActions;

    [Tooltip("Post-Exit 연출이 모두 끝난 후 개별 딜레이로 실행될 액션 목록입니다.")]
    public List<DelayedUltEvent> onPostExitEndActions;


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
        hasBeenTriggered = false;
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
        // BGM 변경
        SoundManager.Instance.PlayBGM(BGMName.이무기);
        
        currentState = ZoneState.PreEnter;
        Debug.Log("State: PreEnter - Pre-Enter sequence started.");
        hasBeenTriggered = true;
        
        StartCoroutine(WaitRoar());

        // onPreEnterActions 리스트의 모든 액션을 각자의 딜레이로 실행
        foreach (var delayedEvent in onPreEnterActions)
        {
            StartCoroutine(InvokeEventWithDelay(delayedEvent.action, delayedEvent.delay));
        }

        // Pre-Enter 연출 시간만큼 대기
        yield return new WaitForSeconds(preEnterSequenceDuration);
        
        currentState = ZoneState.Active;
        Debug.Log("State: Active - Main gameplay started.");

        // onMainGameplayActions 리스트의 모든 액션을 각자의 딜레이로 실행
        foreach (var delayedEvent in onMainGameplayActions)
        {
            StartCoroutine(InvokeEventWithDelay(delayedEvent.action, delayedEvent.delay));
        }
    }

    /// <summary>
    /// 퇴장 시퀀스를 처리하는 코루틴
    /// </summary>
    private IEnumerator ExitSequence()
    {
        currentState = ZoneState.PostExit;
        Debug.Log("State: PostExit - Post-Exit sequence started.");

        // onPostExitActions 리스트의 모든 액션을 각자의 딜레이로 실행
        foreach (var delayedEvent in onPostExitActions)
        {
            StartCoroutine(InvokeEventWithDelay(delayedEvent.action, delayedEvent.delay));
        }

        // Post-Exit 연출 시간만큼 대기
        yield return new WaitForSeconds(postExitSequenceDuration);
        
        currentState = ZoneState.Idle;
        Debug.Log("State: Idle - Zone sequence complete.");

        // onPostExitEndActions 리스트의 모든 액션을 각자의 딜레이로 실행
        foreach (var delayedEvent in onPostExitEndActions)
        {
            StartCoroutine(InvokeEventWithDelay(delayedEvent.action, delayedEvent.delay));
        }
    }
    
    /// <summary>
    /// UltEvent를 지정된 시간만큼 지연시킨 후 호출하는 헬퍼 코루틴입니다.
    /// </summary>
    private IEnumerator InvokeEventWithDelay(UltEvent targetEvent, float delay)
    {
        // 지연 시간이 0보다 클 때만 대기합니다.
        if (delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }
        
        // 이벤트 호출
        targetEvent?.Invoke();
    }

    private IEnumerator WaitRoar()
    {
        yield return new WaitForSeconds(0.5f);
        SoundManager.Instance.PlaySFX(SFXName.이무기_보스_포효_1);
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