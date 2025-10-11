using System.Collections;
using GameAbilitySystem;
using UnityEngine;

public class PlayerRespawnController : MonoBehaviour
{
    private Vector2 _respawnPosition;
    private bool _hasRespawnPoint;

    private AbilitySystem _asc;
    private PlayerController _pc;

    private void Awake()
    {
        DomainFactory.Instance.GetDomain(DomainKey.Player, out _asc);
        _pc  = GetComponent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("RespawnPoint"))
        {
            Transform yTransform = other.transform.Find("Y");
            if (yTransform == null)
            {
                Debug.LogError("[RespawnPoint] 자식 오브젝트 'Y'를 찾을 수 없습니다.");
                return;
            }

            BoxCollider2D yCol = yTransform.GetComponent<BoxCollider2D>();
            if (yCol == null)
            {
                Debug.LogError("[RespawnPoint] 'Y' 오브젝트에 BoxCollider2D가 없습니다.");
                return;
            }

            float y = yCol.bounds.max.y;
            float x = other.bounds.center.x;

            _respawnPosition = new Vector2(x, y);
            _hasRespawnPoint = true;
            Debug.Log($"[RespawnPoint] 저장된 위치: {_respawnPosition}");
        }
    }

    /// <summary>
    /// Spike/Laser 등에서 호출: 딜레이 뒤 리스폰. 무적/입력잠금 포함.
    /// </summary>
    public void StartRespawn(float delaySeconds)
    {
        StopAllCoroutines(); // 중복 호출 방지
        StartCoroutine(RespawnFlow(delaySeconds));
    }

    private IEnumerator RespawnFlow(float delay)
    {
        if (!_hasRespawnPoint)
        {
            _respawnPosition = transform.position;
            Debug.LogWarning("[Respawn] 저장된 리스폰 포인트가 없어 현재 위치로 대체합니다.");
        }

        // 1) 즉시 무적 ON
        if (_asc != null && !_asc.TagContainer.Has(GameplayTags.Invincibility))
            _asc.TagContainer.Add(GameplayTags.Invincibility);

        // 2) 입력 OFF
        _pc?.OnDisableAllInput();

        // 3) 리스폰 대기
        yield return new WaitForSeconds(delay);

        // 4) 위치 이동
        transform.position = _respawnPosition;
        Debug.Log($"[Player] 리스폰 위치로 이동: {_respawnPosition}");

        // 5) 무적 해제 + 입력 ON
        _asc?.TagContainer.Remove(GameplayTags.Invincibility);
        _pc?.OnEnableAllInput();
    }
}
