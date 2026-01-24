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
    [SerializeField] private GameObject loadGamePopup;
    [SerializeField] private GameObject settingsPopup;

    private int currentSelection = 0;
    private GameObject loadGamePopupInstance;
    private GameObject settingsPopupInstance;
    private bool isControlEnabled = true;

    private void Start()
    {
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
            
            EventTrigger trigger = menuButtons[i].gameObject.GetComponent<EventTrigger>();
            if (trigger == null) trigger = menuButtons[i].gameObject.AddComponent<EventTrigger>();
            
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((data) => {
                if (isControlEnabled)
                {
                    currentSelection = index;
                    UpdateSelectionVisuals();
                }
            });
            trigger.triggers.Add(entry);

            menuButtons[i].onClick.RemoveAllListeners();
            menuButtons[i].onClick.AddListener(() => {
                if (isControlEnabled)
                {
                    currentSelection = index;
                    ExecuteSelection();
                }
            });

            if (i == 1 && !hasSaveData) text.color = Color.gray;
        }
    }

    private void Update()
    {
        if (!isControlEnabled) return;
        
        HandleInput();
        UpdateLoadGamePopupPosition();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentSelection = (currentSelection - 1 + menuButtons.Length) % menuButtons.Length;
            UpdateSelectionVisuals();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentSelection = (currentSelection + 1) % menuButtons.Length;
            UpdateSelectionVisuals();
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            ExecuteSelection();
        }
    }

    private void UpdateSelectionVisuals()
    {
        for (int i = 0; i < menuButtons.Length; i++)
        {
            bool isSelected = (i == currentSelection);
            var text = menuButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            
            if (menuArrows.Length > i && menuArrows[i] != null)
                menuArrows[i].SetActive(isSelected);

            if (isSelected)
            {
                text.color = Color.white;
                
                if (i == 1 && JsonLoader.Exists("gamedata_0")) ShowLoadGamePopup();
                else HideLoadGamePopup();
            }
            else
            {
                if (i == 1 && !JsonLoader.Exists("gamedata_0")) 
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
            case 0: // 새로하기
                OnClick_newGameBtn(); 
                break;
            case 1: // 이어하기
                if (JsonLoader.Exists("gamedata_0")) OnClick_loadGameBtn(); 
                break;
            case 2: // 환경설정
                OnClick_settingsBtn(); 
                break;
            case 3: // 게임종료
                OnClick_exitBtn(); 
                break;
        }
    }


    public void OnClick_newGameBtn()
    {
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

    public void OnClick_loadGameBtn() => DomainFactory.Instance.ClearStateAndReload();

    public void OnClick_settingsBtn()
    {
        if (settingsPopupInstance == null)
        {
            settingsPopupInstance = Instantiate(settingsPopup, transform);
            isControlEnabled = false; 
        }
    }

    public void OnClick_exitBtn()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void NewGame_yesBtn() { DomainFactory.Instance.DeleteGameData(); SceneLoader.LoadScene("B2_Monster1"); }
    public void NewGame_noBtn() { newGamePopup.SetActive(false); isControlEnabled = true; }
    public void CloseSettings() { isControlEnabled = true; }

    private void ShowLoadGamePopup()
    {
        if (loadGamePopupInstance == null)
        {
            loadGamePopupInstance = Instantiate(loadGamePopup, transform);
            var lanternState = DomainFactory.Instance.Data.LanternState;
            LoadGamePopup popupUI = loadGamePopupInstance.GetComponent<LoadGamePopup>();
            if (popupUI != null) popupUI.SetInfo(lanternState.RecentFloor, lanternState.RecentSection);
        }
    }

    private void HideLoadGamePopup()
    {
        if (loadGamePopupInstance != null) { Destroy(loadGamePopupInstance); loadGamePopupInstance = null; }
    }

    private void UpdateLoadGamePopupPosition()
    {
        if (currentSelection == 1 && loadGamePopupInstance != null)
        {
            RectTransform arrowRect = menuArrows[1].GetComponent<RectTransform>();
            loadGamePopupInstance.GetComponent<RectTransform>().anchoredPosition = arrowRect.anchoredPosition + new Vector2(250, 0); 
        }
    }
}