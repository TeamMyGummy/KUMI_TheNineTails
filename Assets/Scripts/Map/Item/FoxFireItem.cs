using UnityEngine;
using GameAbilitySystem;
using Game.Inventory;

public class FoxFireItem : MonoBehaviour
{
    [SerializeField] private GameObject interactionUI;
    [SerializeField] private int increaseAmount = 1;

    private bool _playerInRange = false;
    private bool _isUsed = false;
    private AbilitySystem _playerModel;

    private void Awake()
    {
        DomainFactory.Instance.GetDomain(DomainKey.Player, out _playerModel);
        if (_playerModel == null)
            Debug.LogError("[FoxFireItem] Player 도메인 획득 실패");
    }

    private void OnValidate()
    {
        if (increaseAmount < 0) increaseAmount = 0;
    }

    private void Start()
    {
        if (interactionUI != null)
            interactionUI.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isUsed) return;
        if (!other.CompareTag("Player")) return;

        _playerInRange = true;
        interactionUI?.SetActive(true);

        var controller = other.GetComponent<PlayerController>();
        controller?.SetFoxFireItem(this);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (_isUsed) return;
        if (!other.CompareTag("Player")) return;

        _playerInRange = false;
        interactionUI?.SetActive(false);

        var controller = other.GetComponent<PlayerController>();
        controller?.SetFoxFireItem(null);
    }

    private void OnDestroy()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        var controller = player ? player.GetComponent<PlayerController>() : null;
        controller?.SetFoxFireItem(null);
    }

    public void ApplyFoxFireIncrease()
    {
        if (_isUsed || !_playerInRange) return;

        if (_playerModel == null || _playerModel.Attribute == null)
        {
            Debug.LogError("[FoxFireItem] Player 모델/Attribute 없음");
            return;
        }

        var attributes = _playerModel.Attribute.Attributes;
        if (attributes.TryGetValue("FoxFireCount", out var foxfire) && foxfire != null)
        {
            float prevMax = foxfire.MaxValue;
            float newMax = prevMax + increaseAmount;

            float current = foxfire.CurrentValue.Value;
            if (current > newMax) current = newMax;

            foxfire.SetMaxValue(newMax);
            foxfire.SetCurrentValue(current);

            Debug.Log($"[FoxFireItem] 여우불 개수: Max {prevMax} → {newMax}, 현재 유지: {current}");
        }
        else
        {
            Debug.LogWarning("[FoxFireItem] FoxFireCount 속성 없음");
            return;
        }

        InventoryDomain inv;
        DomainFactory.Instance.GetDomain(DomainKey.Inventory, out inv);
        if (inv == null)
        {
            Debug.LogError("[FoxFireItem] Inventory 도메인 획득 실패");
            return;
        }

        inv.AddItem(ItemType.FoxFire, 1);

        _isUsed = true;
        interactionUI?.SetActive(false);
        Destroy(gameObject);
    }
}
