using System.Collections;
using UnityEngine;

public class PlayerRespawnController : MonoBehaviour
{
    [Header("Respawn")]
    [SerializeField] private float defaultRespawnDelay = 1.0f;

    [Header("Layer Swap")]
    [SerializeField] private string normalLayer = "Player";
    [SerializeField] private string invincibleLayer = "PlayerInvincible";

    private int _normalLayerId;
    private int _invincibleLayerId;

    private Vector2 _respawnPosition;
    private bool _hasRespawnPoint;

    private PlayerController _pc;

    /// <summary>
    /// 리스폰/무적 상태의 단일 소스(해저드에서 이 값만 체크)
    /// </summary>
    public bool IsRespawningOrInvincible { get; private set; }

    private void Awake()
    {
        _pc = GetComponent<PlayerController>();

        _normalLayerId = LayerMask.NameToLayer(normalLayer);
        _invincibleLayerId = LayerMask.NameToLayer(invincibleLayer);
        if (_normalLayerId < 0 || _invincibleLayerId < 0)
        {
            Debug.LogError("[Respawn] 지정한 레이어 이름을 찾을 수 없습니다. Project Settings > Tags and Layers 확인하세요.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("RespawnPoint")) return;

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

    /// <summary>
    /// Spike/Laser 등에서 호출: 딜레이 뒤 리스폰. 무적/입력잠금 포함.
    /// </summary>
    public void StartRespawn(float delaySeconds)
    {
        if (delaySeconds <= 0f) delaySeconds = defaultRespawnDelay;
        StopAllCoroutines();
        StartCoroutine(RespawnFlow(delaySeconds));
    }

    private IEnumerator RespawnFlow(float delay)
    {
        if (!_hasRespawnPoint)
        {
            _respawnPosition = transform.position;
            Debug.LogWarning("[Respawn] 저장된 리스폰 포인트가 없어 현재 위치로 대체합니다.");
        }

        // 1) 확실한 가드: 플래그 + 레이어 스왑
        IsRespawningOrInvincible = true;
        if (_invincibleLayerId >= 0) gameObject.layer = _invincibleLayerId;

        // 2) 입력 OFF
        _pc?.OnDisableAllInput();

        // 3) 리스폰 대기
        yield return new WaitForSeconds(delay);

        // 4) 위치 이동
        transform.position = _respawnPosition;
        Debug.Log($"[Player] 리스폰 위치로 이동: {_respawnPosition}");

        // 5) 입력 ON
        _pc?.OnEnableAllInput();

        // 6) 레이어 원복 + 플래그 해제 (마지막)
        if (_normalLayerId >= 0) gameObject.layer = _normalLayerId;
        IsRespawningOrInvincible = false;
    }
}
