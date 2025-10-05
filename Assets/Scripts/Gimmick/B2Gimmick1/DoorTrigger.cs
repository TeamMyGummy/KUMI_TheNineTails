using UnityEngine;

public class DropDoor : MonoBehaviour
{
    public Transform door;
    public float closedY = 0f; //문 닫혔을 때의 y좌표
    public float closeSpeed = 5f;
    public Transform player;

    private bool isClosing = false;
    private bool isClosed = false;
    private int playerCount = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerCount++;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerCount--;
        // 플레이어가 문보다 오른쪽으로 나갔을 때만 닫기 시작
        if (playerCount <= 0 && !isClosed)
        {
            if (player.position.x > transform.position.x)
                isClosing = true;
        }
    }

    private void Update()
    {
        if (!isClosing || isClosed) return;

        Vector3 pos = door.position;
        pos.y = Mathf.MoveTowards(pos.y, closedY, closeSpeed * Time.deltaTime);
        door.position = pos;

        if (Mathf.Abs(pos.y - closedY) < 0.01f)
        {
            isClosed = true;
            isClosing = false;
        }
    }
}