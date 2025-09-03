using UnityEngine;

public sealed class HeadTriggerRelay : MonoBehaviour
{
    [SerializeField] private ImoogiChaseController controller;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!controller) return;
        if (other.CompareTag("Player"))
            controller.onCaughtPlayer?.Invoke(); // 즉시 패배 처리
    }
}
