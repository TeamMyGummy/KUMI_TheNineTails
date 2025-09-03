using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public sealed class ImoogiChaseStartTrigger : MonoBehaviour
{
    [SerializeField] private ImoogiChaseController controller;
    [SerializeField] private float introDelay = 0.7f;

    [Header("Events (선택 연결)")]
    public UnityEvent onEnterBeforeDelay; // 입력잠금, UI숨김, 등장애니 등
    public UnityEvent onAfterDelay;       // BGM pitch=1.5 등
    public UnityEvent onChaseStart;       // 필요 시 추가 연출

    bool _fired;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_fired) return;
        if (!other.CompareTag("Player")) return;
        _fired = true;
        StartCoroutine(BeginRoutine());
    }

    System.Collections.IEnumerator BeginRoutine()
    {
        onEnterBeforeDelay?.Invoke();
        yield return new WaitForSeconds(introDelay);
        onAfterDelay?.Invoke();
        controller?.StartChase();
        onChaseStart?.Invoke();
    }
}
