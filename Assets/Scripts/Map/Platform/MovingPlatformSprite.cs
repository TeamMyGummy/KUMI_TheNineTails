using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformSprite : MonoBehaviour
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
            /* Vector2 contactPoint = other.contacts[0].point;
            Vector2 platformCenter = transform.position;
            
            if (contactPoint.y > platformCenter.y)
            { 
				_movingPlatform.SetPlayer(other.transform);
			} */
            
            /*Debug.Log("실행은 되고 있니?");*/
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
