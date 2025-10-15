using UnityEngine;
using GameAbilitySystem;

public class TailBox : MonoBehaviour
{
    [Header("UI & Prefab")]
    [SerializeField] private GameObject interactionUI; // 자식(InteractionUI) 연결 권장
    [SerializeField] private SkillInfo skillInfoPrefab;
    [SerializeField] private Transform baseCanvas;

    [Header("Ability")]
    [SerializeField] private AbilityKey key = AbilityKey.PlayerAttack;
    [SerializeField] private AbilityName abilityName = AbilityName.PlayerAttack;

    private AbilitySystem _playerModel;
    private bool _playerInRange;
    private bool _isUsed;

    // 고정 인덱스 자식
    private GameObject _beforeUseImage;
    private GameObject _afterUseImage;

    private void Awake()
    {
        DomainFactory.Instance.GetDomain(DomainKey.Player, out _playerModel);
        if (_playerModel == null)
        {
            Debug.LogError("[TailBox] PlayerModel을 찾지 못했습니다. Domain 설정을 확인하세요.");
        }

        if (transform.childCount >= 3)
        {
            _beforeUseImage = transform.GetChild(0)?.gameObject;
            _afterUseImage  = transform.GetChild(1)?.gameObject;

            // 권장: 인스펙터에서 interactionUI를 비워두면 자식(2)을 자동 할당
            if (interactionUI == null)
                interactionUI = transform.GetChild(2)?.gameObject;
        }
        else
        {
            Debug.LogError($"[TailBox] 자식이 3개 미만입니다. (childCount={transform.childCount})");
        }
    }

    private void Start()
    {
        if (_beforeUseImage) _beforeUseImage.SetActive(true);
        if (_afterUseImage)  _afterUseImage.SetActive(false);
        if (interactionUI)   interactionUI.SetActive(false);
    }

    public void TailBoxInteraction()
    {
        Debug.Log($"[TailBox] Try interact: inRange={_playerInRange}, used={_isUsed}");

        if (!_playerInRange || _isUsed) return;

        bool granted = false;
        if (_playerModel != null)
        {
            granted = _playerModel.GrantAbility(key, abilityName);
            Debug.Log(granted ? "[TailBox] 스킬 부여 성공" : "[TailBox] 스킬 부여 실패(키/이름 미존재?)");
        }
        else
        {
            Debug.LogError("[TailBox] PlayerModel이 null이라 GrantAbility 불가");
        }

        // 부여 성공일 때만 사용 완료 처리
        if (granted)
        {
            _isUsed = true;

            if (interactionUI) interactionUI.SetActive(false);

            if (_beforeUseImage) _beforeUseImage.SetActive(false);
            if (_afterUseImage)  _afterUseImage.SetActive(true);

            ShowSkillInfoPopup(key, abilityName);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isUsed) return;

        if (other.CompareTag("Player"))
        {
            _playerInRange = true;
            if (interactionUI) interactionUI.SetActive(true);
        }

        var controller = other.GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.SetTailBox(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (_isUsed) return;

        if (other.CompareTag("Player"))
        {
            _playerInRange = false;
            if (interactionUI) interactionUI.SetActive(false);
        }
    }
    
    // 스킬 정보 팝업 띄우기
    private void ShowSkillInfoPopup(AbilityKey key, AbilityName name)
    {
        var instance = Instantiate(skillInfoPrefab, baseCanvas);

        // 스킬 정보 가져오기
        var so = _playerModel.GetSkillSO(key);
        if (so != null)
        {
            instance.SetSkillInfo(so);
        }
        else
        {
            Debug.LogWarning($"[TailBox] 스킬 정보가 없습니다: {key}, {name}");
        }

        instance.gameObject.SetActive(true);
    }
}
