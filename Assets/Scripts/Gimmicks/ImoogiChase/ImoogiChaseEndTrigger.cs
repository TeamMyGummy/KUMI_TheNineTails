using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public sealed class ImoogiChaseEndTrigger : MonoBehaviour
{
    [SerializeField] private ImoogiChaseController controller;

    [Header("Events")]
    public UnityEvent onChaseStop;   // BGM pitch=1.0, UI 복원, 퇴장 연출 등
    public UnityEvent onHandoff;     // 보스맵 로딩/페이드 등

    bool _fired;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_fired) return;
        if (!other.CompareTag("Player")) return;
        _fired = true;

        controller?.StopChase();
        onChaseStop?.Invoke();
        onHandoff?.Invoke();
    }
}
