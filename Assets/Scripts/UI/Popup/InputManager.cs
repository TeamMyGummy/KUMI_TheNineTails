using UnityEngine;
using GameAbilitySystem;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject uiSkillPrefab; 
    [SerializeField] private Transform canvas;
    [SerializeField] private UI_Pause pauseUI; 

    private UI_Skill uiSkillInstance;
    private AbilitySystem abilitySystem;
    private bool hasTabOpenedUI = false;

    void Start()
    {
        abilitySystem = new AbilitySystem();
        abilitySystem.Init("Domain/Player");
        abilitySystem.SetSceneState(player);
        abilitySystem.GrantAllAbilities();

        if (pauseUI == null) pauseUI = FindObjectOfType<UI_Pause>();
    }

    void Update()
    {
        // 1. ESC 입력 (최우선순위: 팝업 닫기 또는 일시정지)
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            HandleEscapeInput();
            return;
        }

        // 2. 일시정지 상태일 때의 입력 처리 (메뉴 이동)
        if (pauseUI.IsPaused)
        {
            HandlePauseMenuInput();
            return; // 일시정지 중에는 다른 입력(스킬창 등) 차단
        }

        // 3. Tab 입력 (스킬창 토글)
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            if (CanOpenSkillUI())
            {
                hasTabOpenedUI = true;
                ToggleSkillUI();
            }
        }

        // 4. 스킬 UI 조작 (Q, W)
        if (hasTabOpenedUI && uiSkillInstance != null && uiSkillInstance.gameObject.activeSelf)
        {
            if (Keyboard.current.qKey.wasPressedThisFrame) uiSkillInstance.CycleRight();
            else if (Keyboard.current.wKey.wasPressedThisFrame) uiSkillInstance.CycleLeft();
        }
    }

    private void HandleEscapeInput()
    {
        // 1순위: 설정창이 켜져있으면 무시 (설정창 자체 ESC 로직이 닫을 것임)
        if (FindObjectOfType<UI_SettingsPopup>() != null) return;

        // 2순위: 스킬창이 켜져있으면 스킬창 닫기
        if (uiSkillInstance != null && uiSkillInstance.gameObject.activeSelf)
        {
            ToggleSkillUI();
            hasTabOpenedUI = false;
        }
        // 3순위: 그 외 상황에선 일시정지 토글
        else
        {
            pauseUI.TogglePause();
        }
    }

    private void HandlePauseMenuInput()
    {
        if (Keyboard.current.upArrowKey.wasPressedThisFrame) pauseUI.Navigate(-1);
        else if (Keyboard.current.downArrowKey.wasPressedThisFrame) pauseUI.Navigate(1);
        else if (Keyboard.current.enterKey.wasPressedThisFrame) pauseUI.SelectCurrent();
    }

    void ToggleSkillUI()
    {
        if (uiSkillInstance == null)
        {
            var go = Instantiate(uiSkillPrefab, canvas);
            uiSkillInstance = go.GetComponent<UI_Skill>();
            uiSkillInstance.SetAbilitySystem(abilitySystem);
            uiSkillInstance.ForceReferenceReconnect();
            player.GetComponent<PlayerInput>().enabled = false;
            return;
        }

        bool willClose = uiSkillInstance.gameObject.activeSelf;
        uiSkillInstance.TogglePopupExternally();
        player.GetComponent<PlayerInput>().enabled = willClose;
    }

    private bool CanOpenSkillUI()
    {
        if (FindObjectOfType<SkillInfo>() != null) return false;
        if (pauseUI.IsPaused) return false;
        if (FindObjectOfType<UI_SettingsPopup>() != null) return false;
        return true;
    }
}