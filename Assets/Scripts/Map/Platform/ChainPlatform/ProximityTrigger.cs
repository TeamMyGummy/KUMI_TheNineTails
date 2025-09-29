using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ProximityRange 자식 오브젝트에 붙는다.
/// 플레이어가 근접 반경에 들어오고 나감을 ProximityPlatform에 전달.
/// </summary>
public class ProximityTrigger : MonoBehaviour
{
    [SerializeField] private ProximityPlatform prox;

    // 에디터에서 값 바뀌거나 프리팹 적용될 때 자동 연결
    private void OnValidate()
    {
#if UNITY_EDITOR
        if (prox == null)
            prox = GetComponentInParent<ProximityPlatform>();
#endif
    }

// 씬 실행 시에도 누락되었으면 보강 연결
    private void Awake()
    {
        if (prox == null)
            prox = GetComponentInParent<ProximityPlatform>();
    }
    
    
    
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) prox.SetPlayerInRange(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) prox.SetPlayerInRange(false);
    }
}

