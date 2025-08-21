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
    }

    private void Start()
    {
        if (interactionUI != null)
            interactionUI.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("[MaxHpItem] Player와 충돌 감지됨");
        if (_isUsed) return;

        if (other.CompareTag("Player"))
        {
            _playerInRange = true;
            interactionUI?.SetActive(true);

            var controller = other.GetComponent<PlayerController>();
            controller?.SetMaxHpItem(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (_isUsed) return;

        if (other.CompareTag("Player"))
        {
            _playerInRange = false;
            interactionUI?.SetActive(false);
        }
    }

    public void ApplyMaxHpIncrease()
    {
        if (_isUsed || !_playerInRange) return;

        var attr = _playerModel.Attribute;
        if (attr.Attributes.TryGetValue("HP", out var hp))
        {
            float prevMax = hp.MaxValue;
            float newMax = prevMax + increaseAmount;

            float current = hp.CurrentValue.Value;
            hp.SetMaxValue(newMax);
            hp.SetCurrentValue(current);
            Debug.Log($"최대 hp : {prevMax} → {newMax}, 현재 hp: {current}");
        }
        else
        {
            Debug.LogWarning("[MaxHpItem] HP 속성 없음");
        }

        DomainFactory.Instance.GetDomain(DomainKey.Inventory, out InventoryDomain inv);
        inv.AddItem(ItemType.MaxHp, 1);

        _isUsed = true;
        interactionUI?.SetActive(false);

        Destroy(gameObject);
    }
}
