using System;
using System.Collections;
using System.Collections.Generic;
using GameAbilitySystem;
using Unity.VisualScripting;
using UnityEngine;

public class HPRefillStation : MonoBehaviour
{
    
    [SerializeField] private GameObject interactionUI;
    [SerializeField] private GameObject beforeUseImage;
    [SerializeField] private GameObject afterUseImage;
    
    private bool _playerInRange = false;
    private bool _isUsed = false;
    
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
        
        if (beforeUseImage != null)
        {
            beforeUseImage.SetActive(true);
        }
        else
        {
            Debug.LogWarning("[LanternObject] beforeUseImage == null");
        }
        
        if (afterUseImage != null)
        {
            afterUseImage.SetActive(false);
        }
        else
        {
            Debug.LogWarning("[LanternObject] afterUseImage == null");
        }
    }

    public void RefillHp()
    {
        if (_playerInRange == true && _isUsed == false)
        {
            beforeUseImage.SetActive(false);
            afterUseImage.SetActive(true);
            
            AbilitySystem asc;
            DomainFactory.Instance.GetDomain(DomainKey.Player, out asc);
            GameplayAttribute att = asc.Attribute;
            
            var effect = new HpRefillEffect("HP");
            effect.Apply(att);
        
            Debug.Log("[HP] 최대 회복 완료");
            
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
                controller.SetHpRefillStation(this);
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
