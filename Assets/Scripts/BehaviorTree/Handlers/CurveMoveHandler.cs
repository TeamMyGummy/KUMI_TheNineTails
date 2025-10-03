using UnityEngine;
using BehaviorTree;
using BehaviorTree.Leaf;

namespace BehaviorTree
{
    public class CurveMoveHandler : ActionHandler
    {
        // 이동 상태 변수
        private Vector3 _startPos;
        private Vector3 _currentDest;
        private float _timer;
        
        // 이동 파라미터
        private float _duration;
        private AnimationCurve _curve;

        // 목표 지점 계산용 변수
        private Vector3 _vector;
        private EPositionType _xtype;
        private EPositionType _ytype;
        private Transform _player;

        private void Awake()
        {
            _player = GameObject.FindWithTag("Player").transform;
        }

        public void SetMovementPoint(Vector3 vector, EPositionType xtype, EPositionType ytype, float duration, AnimationCurve curve)
        {
            _vector = vector;
            _xtype = xtype;
            _ytype = ytype;
            _duration = duration > 0f ? duration : 0.01f; // 0으로 나누는 것을 방지
            _curve = curve;
        }

        protected override NodeState OnStartAction()
        {
            _currentDest = new Vector3(GetTargetDestination(_xtype).x, GetTargetDestination(_ytype).y);
            _startPos = transform.position;
            _timer = 0f;
            return NodeState.Running;
        }

        protected override NodeState OnUpdateAction()
        {
            _timer += Time.deltaTime;
            float timeRatio = Mathf.Clamp01(_timer / _duration);

            // 커브가 있으면 커브를 따르고, 없으면 선형(timeRatio)으로 진행률(progress)을 계산
            float progress = (_curve != null && _curve.length > 0) 
                ? _curve.Evaluate(timeRatio) 
                : timeRatio;

            transform.position = Vector3.Lerp(_startPos, _currentDest, progress);

            return (timeRatio >= 1f) ? NodeState.Success : NodeState.Running;
        }

        private Vector3 GetTargetDestination(EPositionType type)
        {
            switch (type)
            {
                case EPositionType.Offset:
                    return transform.position + _vector;
                case EPositionType.TargetOffset:
                    return _player.position + _vector;
                case EPositionType.WorldLocation:
                    return _vector;
                case EPositionType.CameraOffset:
                    return Camera.main.transform.position + _vector;
            }
            return Vector3.zero;
        }
    }
}