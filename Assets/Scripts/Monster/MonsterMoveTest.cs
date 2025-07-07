using UnityEngine;

public class MonsterMoveTest : MonoBehaviour
{
    public float moveDistance = 2f;     // 이동 거리 (왼쪽~오른쪽)
    public float moveSpeed = 2f;        // 이동 속도

    private Vector3 startPos;
    private int direction = 1;          // 1이면 오른쪽, -1이면 왼쪽

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        transform.Translate(Vector3.right * direction * moveSpeed * Time.deltaTime);

        // 거리를 초과하면 방향 전환
        if (Mathf.Abs(transform.position.x - startPos.x) > moveDistance)
        {
            direction *= -1;

            // 방향 바꿀 때 정확히 맞춰줌 (튐 방지)
            float clampedX = Mathf.Clamp(transform.position.x, startPos.x - moveDistance, startPos.x + moveDistance);
            transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
        }
    }
}
