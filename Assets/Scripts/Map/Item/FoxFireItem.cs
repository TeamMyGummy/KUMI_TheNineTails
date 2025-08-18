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
    }

    private void Start()
    {
        if (interactionUI != null)
            interactionUI.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {        
        if (_isUsed) return;

        if (other.CompareTag("Player"))
        {
            _playerInRange = true;
            interactionUI?.SetActive(true);

            var controller = other.GetComponent<PlayerController>();
            controller?.SetFoxFireItem(this);
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
    public void ApplyFoxFireIncrease()
    {
        if (_isUsed || !_playerInRange) return;

        if (_playerModel.Attribute.Attributes.TryGetValue("FoxFireCount", out var foxfire))
        {
            float prevMax = foxfire.MaxValue;
            float newMax = prevMax + increaseAmount;

            float current = foxfire.CurrentValue.Value;
            foxfire.SetMaxValue(newMax);
            foxfire.SetCurrentValue(current);

            Debug.Log($"여우불 갯수 증가: Max {prevMax} → {newMax}, 현재: {current}");
        }

        DomainFactory.Instance.GetDomain(DomainKey.Inventory, out InventoryDomain inv);
        inv.AddItem(ItemType.FoxFire, 1);

        _isUsed = true;
        interactionUI?.SetActive(false);
        Destroy(gameObject);
    }
}
