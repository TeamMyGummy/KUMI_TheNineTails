// ImoogiChaseRailVolume.cs
using UnityEngine;

[DisallowMultipleComponent]
public sealed class ImoogiChaseRailVolume : MonoBehaviour
{
    public float railCenterY;             // 이 복도의 중앙 Y
    public float optionalSpeedScale = 1f; // 필요 시 속도 미세조정

    [SerializeField] ImoogiChaseController controller;
    [SerializeField] AudioSource bgm;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (controller) controller.SetRailY(railCenterY);
        if (controller && optionalSpeedScale != 1f)
        {
            // 필요하면 여기서 controller.baseSpeed *= optionalSpeedScale; 식으로 조정
        }
        // BGM은 시작/종료에서만 바꾸면 충분. (여긴 보통 X)
    }
}
