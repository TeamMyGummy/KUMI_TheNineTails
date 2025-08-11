using UnityEngine;
using GameAbilitySystem;

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

        var attr = _playerModel.Attribute;
        if (attr.Attributes.TryGetValue("FoxFireCount", out var foxfire))
        {
            float prevMax = foxfire.MaxValue;
            float newMax = prevMax + increaseAmount;

            float current = foxfire.CurrentValue.Value;  // 유지할 값 저장
            foxfire.SetMaxValue(newMax);
            foxfire.SetCurrentValue(current);        

            Debug.Log($"여우불 갯수 증가: Max {prevMax} → {newMax}, 현재: {current}");

            // UI 업데이트
            FindObjectOfType<UI.VM_PlayerState>()?.SendMessage("OnValidate", SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            Debug.LogWarning("[FoxFireItem] FoxFireCount 속성 없음");
        }

        _isUsed = true;
        interactionUI?.SetActive(false);
        Destroy(gameObject);
    }
}
