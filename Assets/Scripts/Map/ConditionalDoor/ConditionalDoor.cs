using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ConditionalDoor : MonoBehaviour
{
    [Header("Initial")]
    [SerializeField] private bool startOpen = true;

    [Header("Condition Listening (SO)")]
    [Tooltip("이 문이 '닫혀 있을 때' 이 키가 오면 unlockTargets를 연다.")]
    [SerializeField] private ConditionKey conditionKey;

    [Header("Unlock Targets")]
    [Tooltip("조건 만족 시 동시에 열릴 문들(본인 포함 가능)")]
    [SerializeField] private List<ConditionalDoor> unlockTargets = new List<ConditionalDoor>();

    [Header("Slide Movement")]
    [Tooltip("열릴 때 Y축 이동 거리(+면 위로, -면 아래로)")]
    [SerializeField] private float moveDistance = 3f;
    [Tooltip("이동 속도(유닛/초)")]
    [SerializeField] private float moveSpeed = 5f;

    public bool IsOpen { get; private set; } = true;

    private Collider2D _solidCollider; // 항상 enabled (문이 실제로 이동)
    private Vector3 _basePos, _openPos, _closedPos;
    private Coroutine _moveRoutine;

    private void Reset()
    {
        _solidCollider = GetComponent<Collider2D>();
        if (_solidCollider != null)
        {
            _solidCollider.isTrigger = false; // 벽 역할
            _solidCollider.enabled = true;    // 항상 켜둠
        }
    }

    private void Awake()
    {
        _solidCollider = GetComponent<Collider2D>();
        if (_solidCollider != null)
        {
            _solidCollider.isTrigger = false;
            _solidCollider.enabled = true;
        }

        _basePos   = transform.position;
        _closedPos = _basePos;
        _openPos   = _basePos + Vector3.up * moveDistance;

        SetState(startOpen, instantly: true);
    }

    private void OnEnable()
    {
        ConditionEventBus.OnConditionMet += OnConditionMet;
    }

    private void OnDisable()
    {
        ConditionEventBus.OnConditionMet -= OnConditionMet;
    }

    private void OnConditionMet(ConditionKey key)
    {
        // 닫혀 있고, 내가 듣는 키(SO 레퍼런스 같음)면 → 타깃 문들 Open
        if (!IsOpen && conditionKey != null && key == conditionKey)
        {
            for (int i = 0; i < unlockTargets.Count; i++)
            {
                var t = unlockTargets[i];
                if (t != null) t.Open();
            }
        }
    }

    public void Open()  => SetState(true,  instantly: false);
    public void Close() => SetState(false, instantly: false);

    private void SetState(bool open, bool instantly)
    {
        IsOpen = open;
        var target = open ? _openPos : _closedPos;

        if (instantly)
        {
            if (_moveRoutine != null) StopCoroutine(_moveRoutine);
            transform.position = target;
            return;
        }

        if (_moveRoutine != null) StopCoroutine(_moveRoutine);
        _moveRoutine = StartCoroutine(MoveTo(target));
    }

    private System.Collections.IEnumerator MoveTo(Vector3 target)
    {
        while ((transform.position - target).sqrMagnitude > 0.0001f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = target;
        _moveRoutine = null;
    }
}
