using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Util;
using UnityEngine.EventSystems;

public class UI_StartMenu : MonoBehaviour
{
    [Header("Menu Buttons (0:New, 1:Load, 2:Settings, 3:Exit)")]
    [SerializeField] private Button[] menuButtons;

    [Header("Selection Arrows")]
    [SerializeField] private GameObject[] menuArrows;

    [Header("Popups")]
    [SerializeField] private GameObject newGamePopup;
    [SerializeField] private GameObject settingsPopup;

    private int currentSelection = 0;
    private GameObject settingsPopupInstance;
    private bool isControlEnabled = true;

    private bool hasValidDomain = true;
    
    private SoundManager soundManager;

    private void Start()
    {
        if (DomainFactory.Instance == null)
        {
            Debug.LogError("[UI_StartMenu] DomainFactory.Instance is NULL");
            hasValidDomain = false;
        }

        soundManager = SoundManager.Instance;
        
        InitUI();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        
        UpdateSelectionVisuals();
    }

    private void InitUI()
    {
        bool hasSaveData = JsonLoader.Exists("gamedata_0");

        for (int i = 0; i < menuButtons.Length; i++)
        {
            int index = i;

            if (menuButtons[i].TryGetComponent<Image>(out var img))
                img.raycastTarget = true;

            var text = menuButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
                text.raycastTarget = true;

            menuButtons[i].interactable = true;
            menuButtons[i].transition = Selectable.Transition.None;

            EventTrigger trigger = menuButtons[i].GetComponent<EventTrigger>();
            if (trigger == null)
                trigger = menuButtons[i].gameObject.AddComponent<EventTrigger>();

            // 중복 등록 방지
            trigger.triggers.Clear();

            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            entry.callback.AddListener((data) =>
            {
                if (!isControlEnabled) return;
                currentSelection = index;
                UpdateSelectionVisuals();
            });
            trigger.triggers.Add(entry);

            menuButtons[i].onClick.RemoveAllListeners();
            menuButtons[i].onClick.AddListener(() =>
            {
                if (!isControlEnabled) return;
                currentSelection = index;
                ExecuteSelection();
            });

            // Load 버튼 비활성 색상 처리
            if (i == 1 && !hasSaveData && text != null)
                text.color = Color.gray;
        }
    }

    private void Update()
    {
        if (!isControlEnabled) return;
        HandleInput();
    }

    private void HandleInput()
    {
        //사운드 나오면 수정
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SoundManager.Instance.PlaySFX(SFXName.마우스_클릭);
            currentSelection = (currentSelection - 1 + menuButtons.Length) % menuButtons.Length;
            UpdateSelectionVisuals();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SoundManager.Instance.PlaySFX(SFXName.마우스_클릭);
            currentSelection = (currentSelection + 1) % menuButtons.Length;
            UpdateSelectionVisuals();
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SoundManager.Instance.PlaySFX(SFXName.마우스_클릭);
            ExecuteSelection();
        }
    }

    private void UpdateSelectionVisuals()
    {
        bool hasSaveData = JsonLoader.Exists("gamedata_0");

        for (int i = 0; i < menuButtons.Length; i++)
        {
            bool isSelected = (i == currentSelection);
            var text = menuButtons[i].GetComponentInChildren<TextMeshProUGUI>();

            if (menuArrows.Length > i && menuArrows[i] != null)
                menuArrows[i].SetActive(isSelected);

            if (text == null) continue;

            if (isSelected)
            {
                text.color = Color.white;
            }
            else
            {
                if (i == 1 && !hasSaveData)
                    text.color = Color.gray;
                else
                    text.color = new Color(0.6f, 0.6f, 0.6f);
            }
        }
    }

    private void ExecuteSelection()
    {
        switch (currentSelection)
        {
            case 0: // New Game
                OnClick_newGameBtn();
                break;

            case 1: // Load Game
                if (JsonLoader.Exists("gamedata_0"))
                    OnClick_loadGameBtn();
                break;

            case 2: // Settings
                OnClick_settingsBtn();
                break;

            case 3: // Exit
                OnClick_exitBtn();
                break;
        }
    }

    public void OnClick_newGameBtn()
    {
        soundManager.PlaySFX(SFXName.마우스_클릭);
        if (JsonLoader.Exists("gamedata_0"))
        {
            newGamePopup.SetActive(true);
            isControlEnabled = false;
        }
        else
        {
            SceneLoader.LoadScene("B1_Tutorial");
        }
    }

    public void OnClick_loadGameBtn()
    {
        soundManager.PlaySFX(SFXName.마우스_클릭);
        if (!hasValidDomain) return;
        DomainFactory.Instance.ClearStateAndReload();
    }

    public void OnClick_settingsBtn()
    {
        soundManager.PlaySFX(SFXName.마우스_클릭);
        if (settingsPopupInstance == null)
        {
            settingsPopupInstance = Instantiate(settingsPopup, transform);
            isControlEnabled = false;
        }
    }

    public void OnClick_exitBtn()
    {
        soundManager.PlaySFX(SFXName.마우스_클릭);
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    // ===== Popup Callbacks =====

    public void NewGame_yesBtn()
    {
        soundManager.PlaySFX(SFXName.마우스_클릭);
        if (!hasValidDomain) return;

        DomainFactory.Instance.DeleteGameData();
        SceneLoader.LoadScene("B2_Monster1");
    }

    public void NewGame_noBtn()
    {
        soundManager.PlaySFX(SFXName.마우스_클릭);
        newGamePopup.SetActive(false);
        isControlEnabled = true;
    }

    public void CloseSettings()
    {
        soundManager.PlaySFX(SFXName.마우스_클릭);
        isControlEnabled = true;
    }
}