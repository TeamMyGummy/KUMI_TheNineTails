using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UI_InventoryToggle : MonoBehaviour
{
    [SerializeField] private Button inventoryIconButton; 
    [SerializeField] private GameObject inventoryPanelPrefab;
    [SerializeField] private Transform uiParent;

    private GameObject _panelInstance;

    private void Awake()
    {
    
    }

    public void OnClickInventoryIcon() => Toggle();

    public void Toggle()
    {
        if (_panelInstance == null)
        {
            _panelInstance = Instantiate(inventoryPanelPrefab, uiParent == null ? transform.root : uiParent);
            _panelInstance.SetActive(true);

            return;
        }

        _panelInstance.SetActive(!_panelInstance.activeSelf);
    }


    private void Close()
    {
        if (_panelInstance != null) _panelInstance.SetActive(false);
    }
}
