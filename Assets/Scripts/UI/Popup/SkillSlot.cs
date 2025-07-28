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

    private void Awake()
    {
        var button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(OnClickSkill);
    }

    public void SetSkill(AbilityKey key, GameplayAbilitySO so, bool granted)
    {
        Key = key;
        AbilitySO = so;
        IsGranted = granted;

        iconImage.enabled = true;

        if (so != null && so.skillIcon != null && granted)
        {
            //Debug.Log($"[SkillSlot] 적용 아이콘: {so.skillIcon.name}");
            iconImage.sprite = so.skillIcon;
            iconImage.color = Color.white;
        }
        else
        {
            iconImage.color = new Color(0.3f, 0.3f, 0.3f); // 어둡게 처리
        }

        selectionCircle?.SetActive(false);

        var button = GetComponent<Button>();
        if (button != null)
            button.interactable = granted;
    }

    public void SetSelected(bool selected)
    {
        if (selectionCircle != null)
        {
            //Debug.Log($"[SkillSlot] SetSelected({selected}) for {AbilitySO?.skillName}");
            selectionCircle.SetActive(selected);
        }
    }

    public void OnClickSkill()
    {
        if (IsGranted)
            OnClick?.Invoke();
    }
}
