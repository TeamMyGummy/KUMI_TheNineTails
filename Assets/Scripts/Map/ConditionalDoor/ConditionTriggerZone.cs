using UnityEngine;

// 조건부문(door) 예시용
public class ConditionTriggerZone : MonoBehaviour
{
    [SerializeField] private ConditionKey triggerKey;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"플레이어가 트리거에 들어옴, 조건 {triggerKey.name} 발동!");
            ConditionEventBus.Raise(triggerKey);
        }
    }
}