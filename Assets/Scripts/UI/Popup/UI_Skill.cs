using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameAbilitySystem;
using TMPro;

public class UI_Skill : MonoBehaviour
{
    [SerializeField] private GameObject skillPopupPrefab;
    [SerializeField] private Transform canvas;
    [SerializeField] private GameObject skillSlotPrefab;

    [SerializeField] private GameObject skillDescriptionPanel;
    [SerializeField] private Image skillIcon;
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI skillDescriptionText;

    private GameObject skillPopupInstance;
    private Transform skillSlotParent;
    private readonly List<SkillSlot> skillSlots = new();
    private int selectedSkillIndex = -1;
    private AbilitySystem abilitySystem;

    public void SetAbilitySystem(AbilitySystem asc)
    {
        abilitySystem = asc;
    }

    public void TogglePopupExternally()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    private void InitPopupContents()
    {
        skillSlotParent = skillPopupInstance.transform.Find("canvas/Canvas/SkillSlotPanel");
        if (skillSlotParent == null) return;

        var soList = ResourcesManager.Instance.Load<AbilitySystemSO>("Domain/Player")?.AbilitySO;
        if (soList == null) return;

        int maxSlots = Mathf.Min(skillSlotParent.childCount, soList.Count);

        for (int i = 0; i < maxSlots; i++)
        {
            var so = soList[i];
            var key = so.skillKey;
            bool granted = abilitySystem.IsGranted(key);

            var parentSlot = skillSlotParent.GetChild(i);
            var slotGO = Instantiate(skillSlotPrefab, parentSlot);
            slotGO.transform.localPosition = Vector3.zero;
            slotGO.transform.localScale = Vector3.one;

            if (slotGO.TryGetComponent(out SkillSlot slot))
            {
                slot.SetSkill(key, so, granted);
                slot.OnClick = () => TrySelectSkill(slot);
                skillSlots.Add(slot);
            }
        }

        skillDescriptionPanel?.SetActive(false);
    }

    private void TrySelectSkill(SkillSlot slot)
    {
        if (!slot.IsGranted) return;

        foreach (var s in skillSlots)
            s.SetSelected(false);

        slot.SetSelected(true);
        selectedSkillIndex = skillSlots.IndexOf(slot);

        ShowSkillDescription(slot.AbilitySO);
    }

    private void ShowSkillDescription(GameplayAbilitySO so)
    {
        if (so == null || skillDescriptionPanel == null)
        {
            skillDescriptionPanel?.SetActive(false);
            return;
        }

        skillDescriptionPanel.SetActive(true);
        skillIcon.sprite = so.skillIcon;
        skillName.text = so.skillName.ToString();
        skillDescriptionText.text = so.description;
    }
}
