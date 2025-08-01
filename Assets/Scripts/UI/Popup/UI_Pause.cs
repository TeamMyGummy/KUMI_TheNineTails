using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Util;

public class UI_Pause : MonoBehaviour
{
    [SerializeField] private GameObject pausePopup;
    [SerializeField] private GameObject settingsPopup;
    [SerializeField] private Transform canvas; 

    private GameObject pausePopupInstance;
    private GameObject settingsPopupInstance;

    private bool isPaused = false;

    private PlayerController player;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
    } 

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            ShowPausePopup();
            isPaused = true;
            player?.OnDisableAllInput();
        }
    }

    private void ShowPausePopup()
    {
        if (pausePopupInstance != null) return;

        pausePopupInstance = Instantiate(pausePopup, canvas);

        var resumeBtn = pausePopupInstance.transform.Find("Panel/resumeBtn")?.GetComponent<Button>();
        var settingsBtn = pausePopupInstance.transform.Find("Panel/settingsBtn")?.GetComponent<Button>();
        var mainBtn = pausePopupInstance.transform.Find("Panel/mainBtn")?.GetComponent<Button>();

        if (resumeBtn != null) resumeBtn.onClick.AddListener(ResumeGame);
        if (settingsBtn != null) settingsBtn.onClick.AddListener(OnClickSettings);
        if (mainBtn != null) mainBtn.onClick.AddListener(OnClickSaveAndMain);
    }

    public void ResumeGame()
    {
        if (pausePopupInstance != null)
        {
            Destroy(pausePopupInstance);
            pausePopupInstance = null;
        }
        isPaused = false;
        player?.OnEnableAllInput();
    }

    public void OnClickSettings()
    {
        if (settingsPopupInstance == null)
        {
            settingsPopupInstance = Instantiate(settingsPopup, transform);
        }
    }

    public void OnClickSaveAndMain()
    {
        DomainFactory.Instance.SaveGameData();
        SceneLoader.LoadScene("Start");
    }
}