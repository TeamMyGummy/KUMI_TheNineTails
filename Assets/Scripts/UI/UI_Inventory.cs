using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.Inventory;

public class UI_Inventory : MonoBehaviour
{
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private ItemCatalog itemCatalog;

    private InventoryDomain _inv;

    private void Awake()
    {
        DomainFactory.Instance.GetDomain(DomainKey.Inventory, out _inv);
    }

    private void OnEnable()
    {
        if (_inv != null)
        {
            _inv.OnChanged += Refresh;
            Refresh();
        }
    }

    private void OnDisable()
    {
        if (_inv != null)
            _inv.OnChanged -= Refresh;
    }

    private void Refresh()
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        foreach (var item in _inv.Items)
        {
            var go = Instantiate(slotPrefab, contentParent);

            // 🔹 아이콘
            var def = itemCatalog.Get(item.Type);
            if (def != null)
            {
                var iconImage = go.transform.Find("Icon").GetComponent<Image>();
                iconImage.sprite = def.icon;
            }

            // 🔹 이름
            go.transform.Find("Name").GetComponent<TextMeshProUGUI>().text =
                def != null ? def.displayName : item.Type.ToString();

            // 🔹 개수
            go.transform.Find("Count").GetComponent<TextMeshProUGUI>().text = item.Amount.ToString();
        }
    }
}
