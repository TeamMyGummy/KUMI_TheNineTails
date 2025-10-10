using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DoorCloseZone2D : MonoBehaviour
{
    [Tooltip("이 존이 담당하는 문 (비우면 부모에서 자동)")]
    [SerializeField] private ConditionalDoor door;

    [SerializeField] private string playerTag = "Player";

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;

        if (door == null) door = GetComponentInParent<ConditionalDoor>();
    }

    private void Awake()
    {
        if (door == null) door = GetComponentInParent<ConditionalDoor>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (door == null) return;
        if (other.CompareTag(playerTag))
        {
            door.Close();
        }
    }
}