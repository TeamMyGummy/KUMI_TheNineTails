using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Util;
using UnityEngine.SceneManagement;

public class UI_Pause : MonoBehaviour
{
    [SerializeField] private GameObject pausePopupPrefab; // 프리팹
    [SerializeField] private GameObject settingsPopup;
    [SerializeField] private Transform canvas; 

    private GameObject pausePopupInstance;
    private UI_PausePopup popupReferences; // 프리팹 내부의 버튼/화살표 참조
    
    private int currentSelection = 0;
    private bool isPaused = false;
    private bool isControlEnabled = true;
    private PlayerController player;

    public bool IsPaused => isPaused;

    private void Awake() => player = FindObjectOfType<PlayerController>();

    public void TogglePause()
    {
        if (isPaused) ResumeGame();
        else ShowPausePopup();
    }

    private void ShowPausePopup()
    {
        if (pausePopupInstance != null) return;

        // 1. 프리팹 생성
        pausePopupInstance = Instantiate(pausePopupPrefab, canvas);
        
        // 2. 프리팹에 붙어있는 도우미 스크립트 가져오기
        popupReferences = pausePopupInstance.GetComponent<UI_PausePopup>();

        isPaused = true;
        isControlEnabled = true;
        currentSelection = 0;
        Time.timeScale = 0f;
        player?.OnDisableAllInput();

        // 3. 버튼 마우스 클릭 차단 설정
        foreach (var btn in popupReferences.menuButtons)
        {
            if (btn.TryGetComponent<Image>(out var img)) img.raycastTarget = false;
            var text = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null) text.raycastTarget = false;
        }

        UpdateSelectionVisuals();
    }

    public void Navigate(int direction)
    {
        if (!isControlEnabled || popupReferences == null) return;
        
        int btnCount = popupReferences.menuButtons.Length;
        currentSelection = (currentSelection + direction + btnCount) % btnCount;
        UpdateSelectionVisuals();
    }

    public void SelectCurrent()
    {
        if (!isControlEnabled) return;
        ExecuteSelection();
    }

    private void UpdateSelectionVisuals()
    {
        if (popupReferences == null) return;

        for (int i = 0; i < popupReferences.menuButtons.Length; i++)
        {
            bool isSelected = (i == currentSelection);
            
            // 화살표 켜고 끄기
            if (popupReferences.menuArrows[i] != null)
                popupReferences.menuArrows[i].SetActive(isSelected);
            
            // 텍스트 색상 변경
            var text = popupReferences.menuButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
                text.color = isSelected ? Color.white : new Color(0.6f, 0.6f, 0.6f);
        }
    }

    private void ExecuteSelection()
    {
        switch (currentSelection)
        {
            case 0: ResumeGame(); break;
            case 1: OnClickSettings(); break;
            case 2: OnClickSaveAndMain(); break;
        }
    }

    public void ResumeGame()
    {
        if (pausePopupInstance != null) Destroy(pausePopupInstance);
        Time.timeScale = 1f;
        isPaused = false;
        player?.OnEnableAllInput();
    }

    public void OnClickSettings()
    {
        Instantiate(settingsPopup, canvas);
        isControlEnabled = false;
    }

    public void CloseSettings() => isControlEnabled = true;

    public void OnClickSaveAndMain()
    {
        Time.timeScale = 1f;
        DomainFactory.Instance.Data.LanternState.RecentScene = SceneManager.GetActiveScene().name;
        DomainFactory.Instance.SaveGameData();
        SceneLoader.LoadScene("Start");
    }
}