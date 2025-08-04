using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float waitTime = 0.5f;

    private Vector2 _target;
    private Vector2 _worldPointA;
    private Vector2 _worldPointB;
    private Transform _playerOnPlatform;
    
    private void Start()
    {
        _worldPointA = pointA.parent.TransformPoint(pointA.localPosition);
        _worldPointB = pointA.parent.TransformPoint(pointB.localPosition);
        
        _target = _worldPointB;
        StartCoroutine(MovePlatform());
    }
    
    private IEnumerator MovePlatform()
    {
        while (true)
        {
            Vector2 startPosition = transform.position;
            Vector2 targetPosition = _target;
            float journey = 0f;
            float totalDistance = Vector2.Distance(startPosition, targetPosition);
            
            while (journey < totalDistance)
            {
                float distanceThisFrame = speed * Time.deltaTime;
                journey += distanceThisFrame;
                
                Vector2 newPosition = Vector2.MoveTowards((Vector2)transform.position, targetPosition, distanceThisFrame);
                Vector2 deltaPosition = newPosition - (Vector2)transform.position;
                
                transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
                
                MovePlayerWithPlatform(deltaPosition);
                
                yield return null;
            }
            
            _target = (_target == _worldPointA) ? _worldPointB : _worldPointA;
            
            yield return new WaitForSeconds(waitTime);
        }
    }
    
    private void MovePlayerWithPlatform(Vector3 deltaPosition)
    {
        if (_playerOnPlatform != null)
        {
            _playerOnPlatform.position += (Vector3)deltaPosition;
        }
    }
    
    
    
    public void SetPlayer(Transform player)
    {
        _playerOnPlatform = player;
    }
    
    public void RemovePlayer()
    {
        _playerOnPlatform = null;
    }
    
}
