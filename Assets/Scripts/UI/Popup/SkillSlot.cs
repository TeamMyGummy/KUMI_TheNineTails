using UnityEngine;
using UnityEngine.UI;
using GameAbilitySystem;
using System;

public class SkillSlot : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private GameObject selectionCircle;
    public AbilityKey Key { get; private set; }
    public GameplayAbilitySO AbilitySO { get; private set; }
    public bool IsGranted { get; private set; }

    public Action OnClick;

    public void SetSkill(AbilityKey key, GameplayAbilitySO so, bool granted)
    {
        Key = key;
        AbilitySO = so;
        IsGranted = granted;

        if (iconImage != null)
        {
            if (so != null && so.skillIcon != null)
            {
                iconImage.enabled = true;
                iconImage.sprite = so.skillIcon;
                iconImage.color = Color.white;
            }
            else
            {
                iconImage.enabled = false;
                iconImage.color = new Color(0.3f, 0.3f, 0.3f); // 회색처리
            }
        }

        if (selectionCircle != null)
        {
            selectionCircle.SetActive(false);
        }

        var button = GetComponent<Button>();
        if (button != null)
        {
            button.interactable = granted;
        }
    }

    public void SetSelected(bool selected)
    {
        if (selectionCircle != null)
            selectionCircle.SetActive(selected);
    }

    public void OnClickSkill()
    {
        if (IsGranted)
            OnClick?.Invoke();
    }
}
