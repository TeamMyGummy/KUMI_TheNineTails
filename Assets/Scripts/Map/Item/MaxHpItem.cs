using UnityEngine;
using GameAbilitySystem;
using Game.Inventory;

public class MaxHpItem : MonoBehaviour
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
            Debug.LogError("[MaxHpItem] Player 도메인 획득 실패");
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
        controller?.SetMaxHpItem(this);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (_isUsed) return;
        if (!other.CompareTag("Player")) return;

        _playerInRange = false;
        interactionUI?.SetActive(false);

        var controller = other.GetComponent<PlayerController>();
        controller?.SetMaxHpItem(null); // 참조 해제
    }

    private void OnDestroy()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        var controller = player ? player.GetComponent<PlayerController>() : null;
        controller?.SetMaxHpItem(null);
    }

    public void ApplyMaxHpIncrease()
    {
        if (_isUsed || !_playerInRange) return;

        if (_playerModel == null || _playerModel.Attribute == null)
        {
            Debug.LogError("[MaxHpItem] Player 모델/Attribute 없음");
            return;
        }

        var attr = _playerModel.Attribute;
        if (attr.Attributes.TryGetValue("HP", out var hp) && hp != null)
        {
            float prevMax = hp.MaxValue;
            float newMax = prevMax + increaseAmount;

            float current = hp.CurrentValue.Value;
            if (current > newMax) current = newMax;

            hp.SetMaxValue(newMax);
            hp.SetCurrentValue(current);
            Debug.Log($"[MaxHpItem] 최대 HP: {prevMax} → {newMax}, 현재 HP 유지: {current}");
        }
        else
        {
            Debug.LogWarning("[MaxHpItem] HP 속성 없음");
            return;
        }

        InventoryDomain inv;
        DomainFactory.Instance.GetDomain(DomainKey.Inventory, out inv);
        if (inv == null)
        {
            Debug.LogError("[MaxHpItem] Inventory 도메인 획득 실패");
            return;
        }

        inv.AddItem(ItemType.MaxHp, 1);

        _isUsed = true;
        interactionUI?.SetActive(false);
        Destroy(gameObject);
    }
}
