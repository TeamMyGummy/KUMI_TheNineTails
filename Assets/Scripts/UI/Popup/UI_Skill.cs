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
    [SerializeField] private GameObject InventoryPanel;

    private List<SkillSlot> skillSlots = new();
    private int selectedSkillIndex = -1;
    private AbilitySystem abilitySystem;
    private enum Tab { Skill = 0, Journal = 1, Inventory = 2 }
    private int _currentTab = (int)Tab.Skill;

    // ──────────────────────────────────────────────────────────
    // 외부에서 ASC 주입
    public void SetAbilitySystem(AbilitySystem asc)
    {
        abilitySystem = asc;
        InitSkillSlots();
        SelectFirstGrantedSkill();
    }

    // 외부 토글 (버튼/단축키 등)
    public void TogglePopupExternally()
    {
        bool next = !gameObject.activeSelf;
        gameObject.SetActive(next);
        if (next) OpenSkillView();
        else      CloseSkillView();
    }

    // ──────────────────────────────────────────────────────────
    private void Update()
    {
        if (!gameObject.activeSelf) return;

        // 스킬창이 열려 있을 때 ESC/TAB → 스킬창만 닫기
        if (Keyboard.current.escapeKey.wasPressedThisFrame ||
            Keyboard.current.tabKey.wasPressedThisFrame)
        {
            CloseSkillView();
            return;
        }

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
        if (!slot.IsGranted) return;

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
        if (skillDescriptionPanel != null)
            skillDescriptionPanel.SetActive(true);

        if (skillDescriptionText != null)
        {
            skillDescriptionText.gameObject.SetActive(true);
            skillDescriptionText.text = so?.description ?? "No Desc";
            skillDescriptionText.ForceMeshUpdate();
        }

        if (skillName != null)
            skillName.text = so.skillName.ToString();
    }

    private void SelectFirstGrantedSkill()
    {
        for (int i = 0; i < skillSlots.Count; i++)
        {
            if (skillSlots[i].IsGranted)
            {
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

    // ───────────────────── 탭 버튼들 ─────────────────────
    public void OnClickJournalBtn()
    {
        JournalPanel.SetActive(true);
        SkillPanel.SetActive(false);
        InventoryPanel.SetActive(false);
    }

    public void OnClickSkillBtn()
    {
        JournalPanel.SetActive(false);
        SkillPanel.SetActive(true);
        InventoryPanel.SetActive(false);
    }

    public void OnClickInventoryBtn()
    {
        JournalPanel.SetActive(false);
        SkillPanel.SetActive(false);
        InventoryPanel.SetActive(true);
    }

    private void SetTab(Tab tab)
    {
        _currentTab = (int)tab;

        switch (tab)
        {
            case Tab.Skill:
                OnClickSkillBtn();
                SelectFirstGrantedSkill();
                break;
            case Tab.Journal:
                OnClickJournalBtn();
                break;
            case Tab.Inventory:
                OnClickInventoryBtn();
                break;
        }
    }

    public void CycleRight() // Q 키: 좌측 패널로 순환
    {
        int next = (_currentTab + 3 - 1) % 3;
        SetTab((Tab)next);
    }

    public void CycleLeft()  // W 키: 우측 패널로 순환
    {
        int next = (_currentTab + 1) % 3;
        SetTab((Tab)next);
    }

    // ───────────────────── 열기/닫기 공통 ─────────────────────
    public void OpenSkillView()
    {
        gameObject.SetActive(true);
        SetTab(Tab.Skill);
    }

    private void CloseSkillView()
    {
        gameObject.SetActive(false);
    }
}
