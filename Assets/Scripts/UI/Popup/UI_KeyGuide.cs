using UnityEngine;
using UnityEngine.UI;

public class UI_KeyGuidePopup : MonoBehaviour
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
