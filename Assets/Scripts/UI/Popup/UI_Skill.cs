using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using GameAbilitySystem;
using TMPro;

public class UI_Skill : MonoBehaviour
{
    [SerializeField] private Transform skillSlotParent; // Skill 1~9 부모
    [SerializeField] private GameObject skillDescriptionPanel;
    [SerializeField] private Image skillIcon;
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI skillDescriptionText;
    [SerializeField] private GameObject SkillPanel;
    [SerializeField] private GameObject JournalPanel;

    private List<SkillSlot> skillSlots = new();
    private int selectedSkillIndex = -1;
    private AbilitySystem abilitySystem;

    public void SetAbilitySystem(AbilitySystem asc)
    {
        //Debug.Log("[UI_Skill] InitSkillSlots() 실행됨");
        abilitySystem = asc;
        InitSkillSlots();
        SelectFirstGrantedSkill();
    }

    public void TogglePopupExternally()
    {
        gameObject.SetActive(!gameObject.activeSelf);
        if (gameObject.activeSelf)
        {
            OpenSkillView();
        }
    }

    private void Update()
    {
        if (!gameObject.activeSelf) return;

        if (Keyboard.current.aKey.wasPressedThisFrame)
            SelectPrevious();
        else if (Keyboard.current.dKey.wasPressedThisFrame)
            SelectNext();
    }

    private void InitSkillSlots()
    {
        var soList = ResourcesManager.Instance.Load<AbilitySystemSO>("Domain/Player")?.AbilitySO;
        skillSlots.Clear();

        for (int i = 0; i < skillSlotParent.childCount; i++)
        {
            var slotObj = skillSlotParent.GetChild(i);
            if (slotObj.TryGetComponent(out SkillSlot slot))
            {
                if (i < soList.Count)
                {
                    var so = soList[i];
                    var key = so.skillKey;

                    bool granted = abilitySystem.IsGranted(key);

                    slot.SetSkill(key, so, granted);
                    slot.OnClick = granted ? () => TrySelectSkill(slot) : null;

                    skillSlots.Add(slot);
                }
                else
                {
                    slot.SetSkill(AbilityKey.None, null, false);
                    slot.OnClick = null;
                    skillSlots.Add(slot);
                }
            }
        }
    }

    private void TrySelectSkill(SkillSlot slot)
    {
        if (!slot.IsGranted)
        {
            Debug.Log("[TrySelectSkill] Skill not granted");
            return;
        }

        //Debug.Log($"[TrySelectSkill] Selecting {slot.AbilitySO.skillName}");

        foreach (var s in skillSlots)
            s.SetSelected(false);

        slot.SetSelected(true);
        selectedSkillIndex = skillSlots.IndexOf(slot);
        ShowSkillDescription(slot.AbilitySO);
    }

    public void ForceReferenceReconnect()
    {
        skillDescriptionText = transform.Find("Skill/Canvas/DescriptionPanel/Text")?.GetComponent<TMPro.TextMeshProUGUI>();
        skillName = transform.Find("Skill/Canvas/DescriptionPanel/Name")?.GetComponent<TMPro.TextMeshProUGUI>();
        skillIcon = transform.Find("Skill/Canvas/DescriptionPanel/Icon")?.GetComponent<Image>();
        skillDescriptionPanel = transform.Find("Skill/Canvas/DescriptionPanel")?.gameObject;
    }

    private void ShowSkillDescription(GameplayAbilitySO so)
    {
        //Debug.Log("[ShowSkillDescription] 호출됨");

        if (skillDescriptionPanel != null)
        {
            //Debug.Log("[ShowSkillDescription] 패널 활성화 시도");
            skillDescriptionPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("[ShowSkillDescription] skillDescriptionPanel == null");
        }

        if (skillDescriptionText != null)
        {
            skillDescriptionText.gameObject.SetActive(true);
            skillDescriptionText.text = so?.description ?? "No Desc";
            skillDescriptionText.ForceMeshUpdate();
        }
        else
        {
            Debug.LogError("[ShowSkillDescription] skillDescriptionText == null");
        }

        if (skillName != null)
        {
            skillName.text = so.skillName.ToString();
        }
        else
        {
            Debug.LogError("[ShowSkillDescription] skillDescriptionText == null");
        }
    }

    private void SelectFirstGrantedSkill()
    {
        for (int i = 0; i < skillSlots.Count; i++)
        {
            if (skillSlots[i].IsGranted)
            {
                //Debug.Log($"[UI_Skill] 첫 선택 슬롯: {i}");
                TrySelectSkill(skillSlots[i]);
                return;
            }
        }
    }

    private void SelectPrevious()
    {
        int count = skillSlots.Count;
        if (count == 0) return;

        for (int offset = 1; offset < count; offset++)
        {
            int idx = (selectedSkillIndex - offset + count) % count;
            if (skillSlots[idx].IsGranted)
            {
                TrySelectSkill(skillSlots[idx]);
                break;
            }
        }
    }

    private void SelectNext()
    {
        int count = skillSlots.Count;
        if (count == 0) return;

        for (int offset = 1; offset < count; offset++)
        {
            int idx = (selectedSkillIndex + offset) % count;
            if (skillSlots[idx].IsGranted)
            {
                TrySelectSkill(skillSlots[idx]);
                break;
            }
        }
    }
    public void OnClickJournalBtn()
    {
        JournalPanel.SetActive(true);
        SkillPanel.SetActive(false);
    }

    public void OnClickSkillBtn()
    {
        JournalPanel.SetActive(false);
        SkillPanel.SetActive(true);
    }

    public void OpenSkillView()
    {
        gameObject.SetActive(true);
        OnClickSkillBtn();
        SelectFirstGrantedSkill();
    }
}
