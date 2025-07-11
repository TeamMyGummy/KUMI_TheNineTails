using UnityEngine;

public class MonsterMoveTest : MonoBehaviour
{
    public float moveDistance = 2f;     // �̵� �Ÿ� (����~������)
    public float moveSpeed = 2f;        // �̵� �ӵ�

    private Vector3 startPos;
    private int direction = 1;          // 1�̸� ������, -1�̸� ����

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        transform.Translate(Vector3.right * direction * moveSpeed * Time.deltaTime);

        // �Ÿ��� �ʰ��ϸ� ���� ��ȯ
        if (Mathf.Abs(transform.position.x - startPos.x) > moveDistance)
        {
            direction *= -1;

            // ���� �ٲ� �� ��Ȯ�� ������ (Ʀ ����)
            float clampedX = Mathf.Clamp(transform.position.x, startPos.x - moveDistance, startPos.x + moveDistance);
            transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
        }
    }
}
