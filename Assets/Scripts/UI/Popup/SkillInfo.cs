using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameAbilitySystem;
using UnityEngine.InputSystem;

public class SkillInfo : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI skillNameText;
    [SerializeField] private TextMeshProUGUI skillDescriptionText;
    [SerializeField] private Image skillIconImage;
    [SerializeField] private Image skillKeyImage;

    public void SetSkillInfo(GameplayAbilitySO so)
    {
        skillNameText.text = so.skillName.ToString();
        skillDescriptionText.text = so.description;
        skillIconImage.sprite = so.skillIcon;
        skillKeyImage.sprite = so.skillKeyboard;
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            OnClick_Close();
        }
    }

    public void OnClick_Close()
    {
        Destroy(gameObject);
    }
}
