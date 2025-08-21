using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Util;
using UnityEngine.SceneManagement;

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

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            HandleEsc();
        }
    }

    private void HandleEsc()
    {
        if (settingsPopupInstance != null)
        {
            CloseSettingsIfAny();
            return;
        }

        if (isPaused)
        {
            ResumeGame();
            return;
        }

        if (TryCloseAnyOtherPopup())
            return;

        ShowPausePopup();
        isPaused = true;
        player?.OnDisableAllInput();
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            if (TryCloseAnyOtherPopup())
                return;

            ShowPausePopup();
            isPaused = true;
            player?.OnDisableAllInput();
        }
    }

    private void ShowPausePopup()
    {
        if (pausePopupInstance != null) return;

        pausePopupInstance = Instantiate(pausePopup, canvas);

        var resumeBtn   = pausePopupInstance.transform.Find("Panel/resumeBtn")  ?.GetComponent<Button>();
        var settingsBtn = pausePopupInstance.transform.Find("Panel/settingsBtn")?.GetComponent<Button>();
        var mainBtn     = pausePopupInstance.transform.Find("Panel/mainBtn")    ?.GetComponent<Button>();

        if (resumeBtn   != null) resumeBtn.onClick.AddListener(ResumeGame);
        if (settingsBtn != null) settingsBtn.onClick.AddListener(OnClickSettings);
        if (mainBtn     != null) mainBtn.onClick.AddListener(OnClickSaveAndMain);
    }

    public void ResumeGame()
    {
        if (pausePopupInstance != null)
        {
            Destroy(pausePopupInstance);
            pausePopupInstance = null;
        }

        CloseSettingsIfAny();

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

    private void CloseSettingsIfAny()
    {
        if (settingsPopupInstance != null)
        {
            Destroy(settingsPopupInstance);
            settingsPopupInstance = null;
        }
    }

    private bool TryCloseAnyOtherPopup()
    {
        bool closed = false;

        var infos = FindObjectsOfType<SkillInfo>(true);
        foreach (var i in infos)
        {
            Destroy(i.gameObject);
            closed = true;
        }
        if (closed) return true;

        var skill = FindObjectOfType<UI_Skill>(true);
        if (skill != null && skill.gameObject.activeSelf)
        {
            skill.gameObject.SetActive(false);
            return true;
        }

        var guides = FindObjectsOfType<UI_KeyGuidePopup>(true);
        foreach (var g in guides)
        {
            Destroy(g.gameObject);
            closed = true;
        }
        if (closed) return true;

        return false;
    }

    public void OnClickSaveAndMain()
    {
        DomainFactory.Instance.Data.LanternState.RecentScene = SceneManager.GetActiveScene().name;
        DomainFactory.Instance.SaveGameData();
        SceneLoader.LoadScene("Start");
    }
}
