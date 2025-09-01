using UnityEngine;
using BehaviorTree;
using BehaviorTree.Leaf;

namespace BehaviorTree
{
    public class MoveHandler : ActionHandler
    {
        private const float StopThreshold = 0.1f;
        [SerializeField] private float speed = 2f;
        private Vector3 _currentDest;
        private Vector3 _vector;
        private EMoveType _xtype;
        private EMoveType _ytype;
        private Transform _player;
    
        private void Awake()
        {
            _player = GameObject.FindWithTag("Player").transform;
        }
    
        public void SetMovementPoint(Vector3 vector, EMoveType xtype, EMoveType ytype, float speed)
        {
            _vector = vector;
            _xtype = xtype;
            _ytype = ytype;
            if (speed != 0f) this.speed = speed;
        }
    
        protected override NodeState OnStartAction()
        {
            _currentDest = new Vector3(GetTargetDestination(_xtype).x, GetTargetDestination(_ytype).y);
            return NodeState.Running;
        }
    
        protected override NodeState OnUpdateAction()
        {
            if (Vector3.Distance(transform.position, _currentDest) <= StopThreshold) 
                return NodeState.Success; // 목적지 도달 → 행동 완료
    
            // 방향 계산
            var direction = (_currentDest - transform.position).normalized;
    
            // 프레임 이동
            transform.position += direction * (speed * Time.deltaTime);
    
            return NodeState.Running; // 아직 도달 안함 → 계속 실행 중
        }
    
        private Vector3 GetTargetDestination(EMoveType type)
        {
            switch (type)
            {
                case EMoveType.Offset:
                    return transform.position + _vector;
                case EMoveType.TargetOffset:
                    return _player.position + _vector;
                case EMoveType.WorldLocation:
                    return _vector;
                case EMoveType.CameraOffset:
                    return Camera.main.transform.position + _vector;
            }
    
            return Vector3.zero;
        }
    }
}