using System.Collections;
using System.Collections.Generic;
using GameAbilitySystem;
using UnityEngine;

public class TailBox : MonoBehaviour
{
    [SerializeField] private GameObject interactionUI;
    [SerializeField] private AbilityKey key = AbilityKey.PlayerAttack;
    [SerializeField] private AbilityName name = AbilityName.PlayerAttack;
    private SkillInfo skillInfoInstance;
    [SerializeField] private SkillInfo skillInfoPrefab;
    [SerializeField] private Transform baseCanvas;

    private AbilitySystem _playerModel;
    private bool _playerInRange = false;
    private bool _isUsed = false;
    
    // 자동 할당 대상 (자식 인덱스 고정: before(0), after(1), interaction(2))
    private GameObject _beforeUseImage;
    private GameObject _afterUseImage;
    private GameObject _interactionUI;

    public void Awake()
    {
        DomainFactory.Instance.GetDomain(DomainKey.Player, out _playerModel);
        
        // 자식 인덱스 고정
        if (transform.childCount >= 3)
        {
            _beforeUseImage = transform.GetChild(0).gameObject; // beforeUseImage
            _afterUseImage  = transform.GetChild(1).gameObject; // afterUseImage
            _interactionUI  = transform.GetChild(2).gameObject; // InteractionUI
        }
        else
        {
            Debug.LogError($"[TailBox:{name}] 자식이 3개 미만입니다. (childCount={transform.childCount})");
        }
    }

    private void Start()
    {
        // 초기 상태 확정
        if (_beforeUseImage != null) _beforeUseImage.SetActive(true);
        if (_afterUseImage  != null) _afterUseImage.SetActive(false);
        if (_interactionUI  != null) _interactionUI.SetActive(false);
    }

    public void TailBoxInteraction()
    {
        if (_playerInRange == true && _isUsed == false)
        {
            bool success = _playerModel.GrantAbility(key, name);

            if (success)
            {
                Debug.Log("스킬 부여 성공!");
                ShowSkillInfoPopup(key, name);
            }
            else
            {
                Debug.LogWarning("스킬 부여 실패: 해당 키/이름에 맞는 스킬이 존재하지 않음.");
            }

            _isUsed = true;
            interactionUI.SetActive(false);
            
            // 이미지 전환
            _beforeUseImage.SetActive(false);
            _afterUseImage.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isUsed == false)
        {
            if (other.CompareTag("Player"))
            {
                _playerInRange = true;
                interactionUI.SetActive(true);
            }

            var controller = other.GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.SetTailBox(this);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (_isUsed == false)
        {
            if (other.CompareTag("Player"))
            {
                _playerInRange = false;
                interactionUI.SetActive(false);
            }
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
