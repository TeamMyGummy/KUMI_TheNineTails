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
    private Transform _playerOnPlatform;
    
    private Vector2 _currentTarget;
    private Vector2 _worldPointA;
    private Vector2 _worldPointB;
    
    private Rigidbody2D _platformRb;
    private Rigidbody2D _playerRb;
    
    private float _waitTimer = 0f;
    
    private bool _isVertical;
    private bool _isWaiting;
   

    private void Awake()
    {
        _platformRb = GetComponent<Rigidbody2D>();
        
        _pointA = transform.GetChild(1);
        _pointB = transform.GetChild(2);
    }

    private void Start()
    {
        _isWaiting =  false;
            
        _worldPointA = _pointA.position;
        _worldPointB = _pointB.position;
        
        _currentTarget = _worldPointB;
        
        // 이동방향 판단 (상하/좌우)
        Vector2 delta = _worldPointB - _worldPointA;
        _isVertical = Mathf.Abs(delta.y) > Mathf.Abs(delta.x) ? true : false;
    }

    /// <summary>
    /// 플랫폼 이동
    /// </summary>
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

        _platformRb.MovePosition(newPosition);
        /*MovePlayerWithPlatform(deltaPosition);*/

        if (Vector2.Distance(newPosition, _currentTarget) < 0.01f)
        {
            _isWaiting = true;
        }
    }
    
    public void SetPlayer(Transform player)
    {
        _playerOnPlatform = player;
        
        if (_playerRb == null) 
            _playerRb = player.GetComponent<Rigidbody2D>();
    }
    
    public void RemovePlayer()
    {
        _playerOnPlatform = null;
    }

    /*private void MovePlayerWithPlatform(Vector2 delta)
    {
        if (Mathf.Abs(_playerRb.velocity.x) < 0.01f)
        {
            _playerOnPlatform.position += (Vector3)delta;
            Debug.Log("MovePlayerWithPlatform 실행");
        }
    }*/
    
    

    // 끝부분 도착했을 때 플레이어 미끄러지거나 튀어오르지 않게 보정
    // rb 캐싱이 비용이 많이 든다길래 mp에다 적었는데 굳이이럴필요없나... 서희언니 보고있나요? 알려주실래요?
    /*public void ApplyPlatformVelocity()
    {
        Vector2 platformVelocity = _platformRb.velocity;
        
        if (_isVertical)
        {
            _playerRb.velocity = new Vector2(_playerRb.velocity.x, platformVelocity.y);
        }
        else
        {
            _playerRb.velocity = new Vector2(platformVelocity.x, _playerRb.velocity.y);
        }
        
    }*/
    
    
}
