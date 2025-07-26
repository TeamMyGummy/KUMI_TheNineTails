using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcObject : MonoBehaviour
{
    [SerializeField] private GameObject interactionUI;
    
    
    // TODO: 대화창 띄우는 함수 가져와서 NpcInteraction에 추가
    
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
            Debug.LogWarning("[NpcObject] interactionUI == null");
        }
    }
    
    public void NpcInteraction()
    {
        if (_playerInRange == true && _isUsed == false)
        {
            Debug.Log("NpcInteraction 실행");
            
            YarnManager.Instance.RunDialogue("Start");
            
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
                controller.SetNpc(this);
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
