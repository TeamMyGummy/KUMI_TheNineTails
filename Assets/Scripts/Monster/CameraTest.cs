using UnityEngine;

public class CameraTest : MonoBehaviour
{
    [SerializeField] private Transform target; // 따라갈 대상 (예: 플레이어)
    [SerializeField] private Vector3 offset = new Vector3(0f, 1f, -10f); // 기본 카메라 위치 오프셋
    [SerializeField] private float smoothSpeed = 5f; // 부드럽게 따라가기 속도

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.position = smoothedPosition;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}