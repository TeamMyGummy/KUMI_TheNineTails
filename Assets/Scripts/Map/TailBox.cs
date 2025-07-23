using System.Collections;
using System.Collections.Generic;
using GameAbilitySystem;
using UnityEngine;

public class TailBox : MonoBehaviour
{
    [SerializeField] private GameObject interactionUI;
    [SerializeField] private AbilityKey key = AbilityKey.PlayerAttack;
    [SerializeField] private AbilityName name = AbilityName.PlayerAttack;
    
    private AbilitySystem _playerModel;
    private bool _playerInRange = false;
    private bool _isUsed = false;
    
    public void Awake()
    {
        DomainFactory.Instance.GetDomain(DomainKey.Player, out _playerModel);
    }
    
    private void Start()
    {
        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }
        else
        {
            Debug.LogWarning("[LanternObject] interactionUI == null");
        }
    }

    public void TailBoxInteraction()
    {
        if (_playerInRange == true && _isUsed == false)
        {
            bool success = _playerModel.GrantAbility(key, name);

            if (success)
            {
                Debug.Log("스킬 부여 성공!");
            }
            else
            {
                Debug.LogWarning("스킬 부여 실패: 해당 키/이름에 맞는 스킬이 존재하지 않음.");
            }
            
            _isUsed = true;
            interactionUI.SetActive(false);
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
}
