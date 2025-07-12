using UnityEngine;
using UnityEngine.UI;

public class UI_SettingsPopup : MonoBehaviour
{
    [SerializeField] private Button closeBtn;

    private void Start()
    {
        closeBtn.onClick.AddListener(ClosePopup);
    }

    public void ClosePopup()
    {
        Destroy(gameObject);
    }
}
