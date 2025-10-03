using UnityEngine;
using UnityEngine.UI;

public class TimerUITracker : MonoBehaviour
{
    [Header("추적할 대상 (플레이어 머리)")]
    public Transform playerTransformToTrack; 

    [Header("오프셋 설정")]
    public Vector2 worldOffset = new Vector2(0, 0.5f); 
    
    private RectTransform rectTransform; 
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
        rectTransform = GetComponent<RectTransform>();
    }

    void LateUpdate()
    {
        if (playerTransformToTrack == null || rectTransform == null || mainCam == null) return; 

        Vector3 targetWorldPosition = playerTransformToTrack.position + (Vector3)worldOffset; 

        Vector3 screenPosition = mainCam.WorldToScreenPoint(targetWorldPosition);

        rectTransform.position = screenPosition; 
    }
}