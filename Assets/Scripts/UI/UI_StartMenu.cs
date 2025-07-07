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
    [SerializeField] private Button exitBtn;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI loadGameText;

    [Header("Popups")]
    [SerializeField] private GameObject newGamePopup;
    [SerializeField] private GameObject loadGamePopup;
    [SerializeField] private GameObject settingsPopup;

    private GameObject loadGamePopupInstance;
    private GameObject settingsPopupInstance;

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

    public void OnClick_loadGameBtn()
    {
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

    public void OnClick_exitBtn()
    {
        Application.Quit();
    }


    public void NewGame_yesBtn()
    {
        if (JsonLoader.Exists("gamedata_0"))
        {
            string path = Util.JsonLoader.GetDynamicDataPath("gamedata_0");
            System.IO.File.Delete(path);
        }
        SceneLoader.LoadScene("B2");
    }

    public void NewGame_noBtn()
    {
        newGamePopup.SetActive(false);
    }

    public void LoadGame_hovering()
    {
        if (!JsonLoader.Exists("gamedata_0")) return;
        if (loadGamePopupInstance != null) return;

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform as RectTransform,
            Input.mousePosition,
            null,
            out pos
        );

        loadGamePopupInstance = Instantiate(loadGamePopup, transform);
        loadGamePopupInstance.GetComponent<RectTransform>().anchoredPosition = pos + new Vector2(100, -50);
    }

    public void LoadGame_hovering_exit()
    {
        if (loadGamePopupInstance != null)
        {
            Destroy(loadGamePopupInstance);
            loadGamePopupInstance = null;
        }
    }
}