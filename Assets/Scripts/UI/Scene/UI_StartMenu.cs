using UnityEngine;
using UnityEngine.UI;
using TMPro;
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

            loadGamePopupInstance.GetComponent<RectTransform>().anchoredPosition = pos + new Vector2(200, -100);
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
            SceneLoader.LoadScene("B1_Tutorial");
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
        DomainFactory.Instance.DeleteGameData();
        SceneLoader.LoadScene("B2_Monster1");
    }

    public void NewGame_noBtn()
    {
        newGamePopup.SetActive(false);
    }

    public void LoadGame_hovering()
    {
        // 1. 저장 데이터가 없으면 즉시 종료
        if (!JsonLoader.Exists("gamedata_0")) return;

        // 2. 팝업이 아직 생성되지 않았으면 생성
        if (loadGamePopupInstance == null)
        {
            loadGamePopupInstance = Instantiate(loadGamePopup, transform);

            // DomainFactory 인스턴스 체크 (안전성 확보)
            if (DomainFactory.Instance == null)
            {
                Debug.LogError("DomainFactory.Instance가 씬에 없습니다. 저장 정보를 가져올 수 없습니다.");
                return;
            }

            // 3. 저장된 LanternState 데이터에서 층수/구역명 문자열을 직접 가져옴
            var lanternState = DomainFactory.Instance.Data.LanternState;
        
            string floor = lanternState.RecentFloor;
            string area = lanternState.RecentSection; 
            
            Debug.Log($"[UI Update] 저장된 정보 로드: 층={floor}, 구역={area}");
        
            // 4. 팝업 컴포넌트를 가져와서 텍스트 갱신
            LoadGamePopup popupUI = loadGamePopupInstance.GetComponent<LoadGamePopup>();
        
            if (popupUI != null)
            {
                popupUI.SetInfo(floor, area);
            }
            else
            {
                // 이 에러가 뜬다면 LoadGamePopup 프리팹에 스크립트가 붙어있는지 확인 필요
                Debug.LogError("LoadGamePopup 스크립트를 찾을 수 없습니다. (프리팹 설정 확인)");
            }
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