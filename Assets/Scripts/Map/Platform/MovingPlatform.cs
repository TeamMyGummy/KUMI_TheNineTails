using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private float waitTime = 0.5f;

    private Transform _pointA;
    private Transform _pointB;
    
    private Vector2 _currentTarget;
    private Vector2 _worldPointA;
    private Vector2 _worldPointB;
    private Transform _playerOnPlatform;
    private Rigidbody2D _rb;
    
    private bool _isWaiting = false;
    private float _waitTimer = 0f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        
        _pointA = transform.GetChild(1);
        _pointB = transform.GetChild(2);
    }

    private void Start()
    {
        _worldPointA = _pointA.position;
        _worldPointB = _pointB.position;
        
        _currentTarget = _worldPointB;
        /*StartCoroutine(MovePlatform());*/
    }

    private void FixedUpdate()
    {
        if (_isWaiting)
        {
            _waitTimer += Time.deltaTime;
            if (_waitTimer >= waitTime)
            {
                _isWaiting = false;
                _waitTimer = 0f;
                _currentTarget = (_currentTarget == _worldPointA) ? _worldPointB : _worldPointA;
            }

            return;
        }
        
        Vector2 currentPosition = transform.position;
        Vector2 newPosition = Vector2.MoveTowards(currentPosition, _currentTarget, speed * Time.deltaTime);
        Vector2 deltaPosition = newPosition - currentPosition;

        _rb.MovePosition(newPosition);

        if (Vector2.Distance(newPosition, _currentTarget) < 0.01f)
        {
            _isWaiting = true;
        }
    }

    /*private IEnumerator MovePlatform()
    {
        while (true)
        {
            Vector2 startPosition = transform.position;
            Vector2 targetPosition = _currentTarget;
            
            while (Vector2.Distance(transform.position, targetPosition) > 0.01f)
            {
                Vector2 newPosition = Vector2.MoveTowards((Vector2)transform.position, targetPosition, speed * Time.deltaTime);
                Vector2 deltaPosition = newPosition - (Vector2)transform.position;
                
                _rb.MovePosition(newPosition);
                MovePlayerWithPlatform(deltaPosition);
                
                yield return null;
            }
            
            _currentTarget = (_currentTarget == _worldPointA) ? _worldPointB : _worldPointA;
            
            yield return new WaitForSeconds(waitTime);
        }
    }*/
    
    public void SetPlayer(Transform player)
    {
        _playerOnPlatform = player;
        Debug.Log("SetPlayer");
    }
    
    public void RemovePlayer()
    {
        _playerOnPlatform = null;
        Debug.Log("RemovePlayer");
    }
    
}
