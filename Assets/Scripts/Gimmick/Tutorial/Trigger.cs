using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    
    [SerializeField] public UnityEvent onPlayerEnter;
    
    PlayerController _playerController;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerController = other.GetComponent<PlayerController>();
            if (_playerController == null)
            {
                return; 
            }
            
            Debug.Log("플레이어가 트리거에 닿았습니다! 이벤트를 발생시킵니다.");
            
            if (onPlayerEnter != null)
            {
                onPlayerEnter.Invoke();
            }
            
            
        }
    }
}
