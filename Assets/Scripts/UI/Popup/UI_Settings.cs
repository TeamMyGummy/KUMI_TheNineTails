using UnityEngine;
using UnityEngine.UI;

public class UI_SettingsPopup : MonoBehaviour
{
    [SerializeField] private Button closeBtn;
    private UI_StartMenu startMenu;

    private void Start()
    {
        startMenu = Object.FindFirstObjectByType<UI_StartMenu>();

        if (closeBtn != null)
        {
            // 이 버튼은 인스펙터에서 Raycast Target이 켜져 있어야 합니다.
            closeBtn.onClick.AddListener(ClosePopup);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) ClosePopup();
    }

    public void ClosePopup()
    {
        if (startMenu != null) startMenu.CloseSettings();
        Destroy(gameObject);
    }
}