using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformCollider : MonoBehaviour
{
    private MovingPlatform _movingPlatform;
        
        private void Awake()
        {
            _movingPlatform = GetComponentInParent<MovingPlatform>();
            if (_movingPlatform == null)
            {
                Debug.LogError("[MovingPlatformSprite] MovingPlatform component == null");
            }
        }
    
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                _movingPlatform.SetPlayer(other.transform);
            }
        }
    
        private void OnCollisionExit2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                _movingPlatform.RemovePlayer();
            }
        }
}
