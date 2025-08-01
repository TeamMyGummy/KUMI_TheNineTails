using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float waitTime = 0.5f;

    private Vector3 _target;
    private Transform _playerOnPlatform;
    
    private void Start()
    {
        _target = pointB.position;
        StartCoroutine(MovePlatform());
    }
    
    private IEnumerator MovePlatform()
    {
        Debug.Log("MovePlatform실행");
        while (true)
        {
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = _target;
            float journey = 0f;
            float totalDistance = Vector3.Distance(startPosition, targetPosition);
            
            while (journey < totalDistance)
            {
                float distanceThisFrame = speed * Time.deltaTime;
                journey += distanceThisFrame;
                
                Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, distanceThisFrame);
                Vector3 deltaPosition = newPosition - transform.position;
                transform.position = newPosition;
                
                
                MovePlayerWithPlatform(deltaPosition);
                
                yield return null;
            }
            
            _target = (_target == pointA.position) ? pointB.position : pointA.position;
            
            yield return new WaitForSeconds(waitTime);
        }
    }
    
    private void MovePlayerWithPlatform(Vector3 deltaPosition)
    {
        _playerOnPlatform.position += deltaPosition;
        
        /*if (_playerOnPlatform != null)
        {
            _playerOnPlatform.position += deltaPosition;
        }*/
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
