using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Util;

public class UI_StartMenu : MonoBehaviour
{
    [Header("Menu Buttons (0:New, 1:Load, 2:Settings, 3:Exit)")]
    [SerializeField] private Button[] menuButtons;
    
    [Header("Selection Arrows")]
    [SerializeField] private GameObject[] menuArrows;

    [Header("Popups")]
    [SerializeField] private GameObject newGamePopup;
    [SerializeField] private GameObject loadGamePopup;
    [SerializeField] private GameObject settingsPopup; // 환경설정 팝업 프리팹

    private int currentSelection = 0;
    private GameObject loadGamePopupInstance;
    private GameObject settingsPopupInstance;
    private bool isControlEnabled = true;

    private void Start()
    {
        InitUI();
    
        // 마우스 커서를 숨기거나 잠그지 않습니다. (기본 상태 유지)
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        UpdateSelectionVisuals();
    }

    private void InitUI()
    {
        bool hasSaveData = JsonLoader.Exists("gamedata_0");

        for (int i = 0; i < menuButtons.Length; i++)
        {
            // 핵심: 버튼의 Image와 Text가 마우스 클릭(Raycast)을 무시하도록 설정
            if (menuButtons[i].TryGetComponent<Image>(out var img)) 
                img.raycastTarget = false; 

            var text = menuButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (text != null) 
                text.raycastTarget = false;

            // Button 컴포넌트의 자체 기능(마우스 호버 등)을 완전히 무시하도록 설정
            menuButtons[i].interactable = false; 
            // 주의: interactable을 끄면 버튼이 회색으로 변할 수 있으니 
            // 텍스트 색상은 UpdateSelectionVisuals에서 관리하는 대로 나옵니다.

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
        // 위/아래 방향키 조작 (0~3 순환)
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

        // 엔터키 결정
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

            // 1. 화살표 활성화/비활성화
            if (menuArrows.Length > i && menuArrows[i] != null)
                menuArrows[i].SetActive(isSelected);

            // 2. 텍스트 강조 효과 (색상만 변경, 크기 고정)
            if (isSelected)
            {
                text.color = Color.white;
                
                // 이어하기(인덱스 1)가 선택된 경우에만 로드 정보 팝업 표시
                if (i == 1 && JsonLoader.Exists("gamedata_0")) ShowLoadGamePopup();
                else HideLoadGamePopup();
            }
            else
            {
                // 선택되지 않은 버튼 처리
                if (i == 1 && !JsonLoader.Exists("gamedata_0")) 
                    text.color = Color.gray;
                else 
                    text.color = new Color(0.6f, 0.6f, 0.6f); // 비선택 항목은 약간 어둡게
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

    // --- 기능별 로직 ---

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
            // 팝업이 닫힐 때 isControlEnabled를 true로 돌려주는 로직은 팝업 스크립트에서 처리하거나,
            // 여기서 간단히 제어 가능합니다.
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

    // 팝업 응답용 (새로하기 확인창)
    public void NewGame_yesBtn() { DomainFactory.Instance.DeleteGameData(); SceneLoader.LoadScene("B2_Monster1"); }
    public void NewGame_noBtn() { newGamePopup.SetActive(false); isControlEnabled = true; }

    // 설정창 닫기용 (외부에서 호출)
    public void CloseSettings() { isControlEnabled = true; }

    // --- 이어하기 정보 팝업 ---

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