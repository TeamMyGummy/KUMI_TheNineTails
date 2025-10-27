using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Util;

public enum LanternAppearance
{
    Off,
    Small,
    Big,
}

//LanternObject가 존재하는 씬에는 반드시 Lantern이 씬에 존재해야 함
public class LanternObject : MonoBehaviour
{
    public int LanternKey { get; private set; }

    [SerializeField] private GameObject offLanternImage;      
    [SerializeField] private GameObject smallLanternImage;   
    [SerializeField] private GameObject bigLanternImage;    

    [SerializeField] private GameObject interactionUI;
    
    /// <summary>
    /// 시작화면 UI에 저장된 층수/구간 표기하기 위해 추가 (바꿀 수도 있음... 노가다가 이게 맞나...)
    /// </summary>
    [SerializeField] private string numberOfFloor; //층수
    [SerializeField] private string sectionName; // 구간명
    public string NumberOfFloor => numberOfFloor;
    public string SectionName => sectionName;

    public bool isInteracting = false;
    private bool _playerInRange = false;
    private LanternAppearance _currentAppearance = LanternAppearance.Off;

    public Action<int> Interacted = delegate { };

    void Awake()
    {
        ChangeLanternState(LanternAppearance.Off);

        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }
        else
        {
            Debug.LogWarning("[LanternObject] interactionUI == null");
        }

        LanternKey = SceneLoader.GetCurrentSceneName().StringToInt() + transform.GetSiblingIndex();
        
        Debug.Log($"{LanternKey} : {transform.GetSiblingIndex()}");
        Lantern.Instance.Register(this);
    }

    /*public void Bind(Action<int> interacted)
    {
        Interacted = interacted;
    }*/

    public void ChangeLanternState(LanternAppearance appearance)
    {
        _currentAppearance = appearance;

        if (offLanternImage != null) offLanternImage.SetActive(false);
        if (smallLanternImage != null) smallLanternImage.SetActive(false);
        if (bigLanternImage != null) bigLanternImage.SetActive(false);

        switch (appearance)
        {
            case LanternAppearance.Off:
                if (offLanternImage != null)
                    offLanternImage.SetActive(true);
                break;

            case LanternAppearance.Small:
                if (smallLanternImage != null)
                {
                    Debug.Log("랜턴 Small로 change");
                    smallLanternImage.SetActive(true);
                }
                break;

            case LanternAppearance.Big:
                if (bigLanternImage != null)
                {
                    Debug.Log("랜턴 Big으로 change");
                    bigLanternImage.SetActive(true);
                }
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = true;

            if (interactionUI != null)
            {
                interactionUI.SetActive(true);
            }
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.SetLanternObject(this);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = false;

            if (interactionUI != null)
                interactionUI.SetActive(false);

            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.SetLanternObject(null); // 더 이상 상호작용 대상 아님
            }
        }
    }

    public void InteractWithLantern()
    {
        if (_playerInRange)
        {
            Interacted?.Invoke(LanternKey);
            isInteracting = true;
            ChangeLanternState(LanternAppearance.Big);
        }
    }
    
    public void SetLanternKey(int key)
    {
        LanternKey = key;
    }

}


