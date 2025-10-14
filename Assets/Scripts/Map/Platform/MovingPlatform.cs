using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 1.5f;         // 최대 속도
    [SerializeField] private float minSpeed = 0.05f;     // 양 끝에서의 최소 속도
    [SerializeField] private float slowRadius = 1.0f;    // 이 반경 안에서 가속/감속 시작
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

    private Transform _childCollider;

    private void Awake()
    {
        _platformRb = GetComponent<Rigidbody2D>();

        _childCollider = transform.GetChild(0);
        _pointA = transform.GetChild(1);
        _pointB = transform.GetChild(2);
    }

    private void Start()
    {
        _isWaiting = false;

        _worldPointA = _pointA.position;
        _worldPointB = _pointB.position;

        _currentTarget = _worldPointB;

        Vector2 delta = _worldPointB - _worldPointA;
        _isVertical = Mathf.Abs(delta.y) > Mathf.Abs(delta.x) ? true : false;

        if (_childCollider != null)
        {
            _childCollider.position = transform.position;
        }
    }

    /// <summary>
    /// 플랫폼 이동 (출발 가속 + 도착 감속: 양 끝에 같은 slowRadius 적용)
    /// </summary>
    private void FixedUpdate()
    {
        if (_isWaiting)
        {
            _waitTimer += Time.fixedDeltaTime;
            if (_waitTimer >= waitTime)
            {
                _isWaiting = false;
                _waitTimer = 0f;
                _currentTarget = (_currentTarget == _worldPointA) ? _worldPointB : _worldPointA;
            }

            return;
        }

        Vector2 currentPosition = transform.position;
        float distanceToTarget = Vector2.Distance(currentPosition, _currentTarget);

        // 전체 경로 길이 (A <-> B)
        float totalDistance = Vector2.Distance(_worldPointA, _worldPointB);
        if (totalDistance <= 0f)
        {
            // 포인트가 겹쳐 있으면 아무것도 안함
            return;
        }

        // 출발점 계산: 현재 이동 방향에 따라 출발점(startPoint)을 정함
        Vector2 startPoint = (_currentTarget == _worldPointB) ? _worldPointA : _worldPointB;

        float distanceFromStart = Vector2.Distance(currentPosition, startPoint);

        // 양 끝에서의 비율(0=끝에 가까움, 1=slowRadius 바깥)
        float tStart = (slowRadius <= 0f) ? 1f : Mathf.Clamp01(distanceFromStart / slowRadius);
        float tEnd = (slowRadius <= 0f) ? 1f : Mathf.Clamp01(distanceToTarget / slowRadius);

        // 이징 적용: SmoothStep으로 부드럽게 (0->1)
        float easedStart = Mathf.SmoothStep(0f, 1f, tStart); // 출발에서 멀어질수록 0->1
        float easedEnd = Mathf.SmoothStep(0f, 1f, tEnd);     // 도착에서 멀어질수록 0->1

        // 최종 속도 계수: 양 끝 중 더 작은 값을 사용 -> 양 끝에서 모두 느려짐, 중앙에서 최대
        float speedFactor = Mathf.Min(easedStart, easedEnd);

        // 현재 속도 계산
        float currentSpeed = Mathf.Lerp(minSpeed, speed, speedFactor);

        // 이동
        Vector2 newPosition = Vector2.MoveTowards(currentPosition, _currentTarget, currentSpeed * Time.fixedDeltaTime);
        Vector2 deltaPosition = newPosition - currentPosition;

        _platformRb.MovePosition(newPosition);
        // 필요하면 플레이어 동승 처리: MovePlayerWithPlatform(deltaPosition);

        // 도착 판정: 거의 도달하면 대기 상태로 전환
        if (distanceToTarget <= 0.01f)
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

   
}
