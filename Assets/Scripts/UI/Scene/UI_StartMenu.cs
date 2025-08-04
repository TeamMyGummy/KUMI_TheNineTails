using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Util;

public class UI_StartMenu : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button loadGameBtn;
    [SerializeField] private Button newGameBtn;
    [SerializeField] private Button settingsBtn;
    [SerializeField] private Button keyGuideBtn;
    [SerializeField] private Button exitBtn;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI loadGameText;

    [Header("Popups")]
    [SerializeField] private GameObject newGamePopup;
    [SerializeField] private GameObject loadGamePopup;
    [SerializeField] private GameObject settingsPopup;
    [SerializeField] private GameObject keyGuidePopup;

    private GameObject loadGamePopupInstance;
    private bool isHoveringLoadGame = false;
    private GameObject settingsPopupInstance;
    private GameObject keyGuidePopupInstance;

    private void Start()
    {
        if (JsonLoader.Exists("gamedata_0"))
        {
            loadGameBtn.interactable = true;
            loadGameText.color = Color.white;
        }
        else
        {
            loadGameBtn.interactable = false;
            loadGameText.color = Color.gray;
        }
    }
    private void Update()
    {
        if (isHoveringLoadGame && loadGamePopupInstance != null)
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                transform as RectTransform,
                Input.mousePosition,
                null,
                out pos
            );

            loadGamePopupInstance.GetComponent<RectTransform>().anchoredPosition = pos + new Vector2(100, -50);
        }
    }

    public void OnClick_loadGameBtn()
    {
        DomainFactory.Instance.ClearStateAndReload();
    }

    public void OnClick_newGameBtn()
    {
        if (JsonLoader.Exists("gamedata_0"))
        {
            newGamePopup.SetActive(true);
        }
        else
        {
            SceneLoader.LoadScene("B2");
        }
    }

    public void OnClick_settingsBtn()
    {
        if (settingsPopupInstance == null)
        {
            settingsPopupInstance = Instantiate(settingsPopup, transform);
        }
    }

    public void OnClick_keyGuideBtn()
    {
        if (keyGuidePopupInstance == null)
        {
            keyGuidePopupInstance = Instantiate(keyGuidePopup, transform);
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

    public void NewGame_yesBtn()
    {
        if (JsonLoader.Exists("gamedata_0"))
        {
            string path = Util.JsonLoader.GetDynamicDataPath("gamedata_0");
            System.IO.File.Delete(path);
        }
        DomainFactory.Instance.ClearStateAndReload();
    }

    public void NewGame_noBtn()
    {
        newGamePopup.SetActive(false);
    }

    public void LoadGame_hovering()
    {
        if (!JsonLoader.Exists("gamedata_0")) return;

        if (loadGamePopupInstance == null)
        {
            loadGamePopupInstance = Instantiate(loadGamePopup, transform);
        }

        isHoveringLoadGame = true;
    }

    public void LoadGame_hovering_exit()
    {
        if (loadGamePopupInstance != null)
        {
            Destroy(loadGamePopupInstance);
            loadGamePopupInstance = null;
        }

        isHoveringLoadGame = false;
    }
}