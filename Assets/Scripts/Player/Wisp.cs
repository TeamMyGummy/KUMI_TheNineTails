using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Wisp : MonoBehaviour
{
    private IMovement _targetMovement;
    private GameObject _player;
    private Transform _target;
    public float followSpeed = 2f;
    public Vector3 playerOffset = new Vector3(0, 1.5f, -1f);
    public Vector3 currentOffset;
    private bool _bApplyDirection;
    private bool _isTargetPlayer = true;
    
    // --- 수정된 변수 ---
    [Header("곡선 속도 설정")]
    public AnimationCurve speedCurve; // 시간에 따른 속도 변화 곡선
    public float moveDuration = 1.0f; // 목표 지점까지 이동하는 총 시간

    private Vector3 _startPosition; // 이동 시작 위치
    private Transform _curveTargetTransform;
    private Vector3 _curveTargetOffset;
    private float _moveTimer; // 이동 경과 시간
    // -------------------
    
    void Awake()
    {
        _player = GameObject.FindWithTag("Player");
        _targetMovement = _player.GetComponent<IMovement>();
        ChangeTargetToPlayer();
    }
    
    void Update()
    {
        if (_isTargetPlayer)
        {
            // --- 기존 플레이어 추적 로직 ---
            if (_target == null)
            {
                _target = GameObject.FindWithTag("Player").transform;
                if (_target == null) return;
            }

            Vector3 desiredPos = _target.position;
            Vector3 offset = currentOffset;
            if (_bApplyDirection)
                offset.x *= _targetMovement.Direction.x;
            desiredPos += offset;
            transform.position = Vector3.Lerp(transform.position, desiredPos, followSpeed * Time.deltaTime);
        }
        else
        {
            // --- '속도'가 조절된 직선 이동 로직 ---
            if (_curveTargetTransform == null)
            {
                ChangeTargetToPlayer();
                return;
            }

            _moveTimer += Time.deltaTime;
            
            // 1. 선형적인 시간 진행률 (0 -> 1)
            float linearT = Mathf.Clamp01(_moveTimer / moveDuration);

            // 2. AnimationCurve를 통과시켜 '속도'가 적용된 진행률 (0 -> 1) 계산
            float easedT = speedCurve.Evaluate(linearT);

            // 3. 시작점과 끝점 계산
            Vector3 startPoint = _startPosition;
            Vector3 endPoint = _curveTargetTransform.position + _curveTargetOffset;

            // 4. '속도'가 적용된 easedT를 사용해 *직선* 보간
            //    (경로가 휘는 것이 아님)
            transform.position = Vector3.Lerp(startPoint, endPoint, easedT);

            // 5. 이동 완료 시(linearT 기준) 플레이어 추적으로 복귀
            if (linearT >= 1.0f)
            {
                ChangeTargetToPlayer();
            }
        }
    }

    public bool IsTargetEqualPlayer()
    {
        return _isTargetPlayer;
    }

    public void ChangeTargetToPlayer()
    {
        _isTargetPlayer = true;
        _target = _player.transform;
        currentOffset = playerOffset;
        _bApplyDirection = true;
    }

    public void ChangeTarget(Transform target, Vector3 offset)
    {
        _isTargetPlayer = false;
        
        // 이동에 필요한 값들 설정
        _startPosition = transform.position;
        _curveTargetTransform = target;
        _curveTargetOffset = offset;
        _moveTimer = 0f;
    }
}