using UnityEngine;
using BehaviorTree;
using BehaviorTree.Leaf;

namespace BehaviorTree
{
    [RequireComponent(typeof(PositionHelper))]
    public class MoveHandler : ActionHandler
    {
        private const float StopThreshold = 0.1f;
        [SerializeField] private float speed = 2f;
        private Vector3 _currentDest;
        private Vector3 _vector;
        private EPositionType _xtype;
        private EPositionType _ytype;
        private PositionHelper _positionHelper;
    
        private void Awake()
        {
            _positionHelper = GetComponent<PositionHelper>();
        }
    
        public void SetMovementPoint(Vector3 vector, EPositionType xtype, EPositionType ytype, float speed)
        {
            _vector = vector;
            _xtype = xtype;
            _ytype = ytype;
            if (speed != 0f) this.speed = speed;
        }
    
        protected override NodeState OnStartAction()
        {
            _currentDest = new Vector3(
                _positionHelper.GetDestination(_xtype, transform, _vector).x, 
                _positionHelper.GetDestination(_ytype, transform, _vector).y);

            if (speed < 0f)
            {
                transform.position = _currentDest;
                return NodeState.Success;
            }
            
            return NodeState.Running;
        }
    
        protected override NodeState OnUpdateAction()
        {
            // 목표 지점까지의 벡터와 거리 계산
            Vector3 vectorToDest = _currentDest - transform.position;
            float distanceToDest = vectorToDest.magnitude;

            // 1. 목적지에 도착했는지 먼저 확인
            if (distanceToDest <= StopThreshold)
            {
                return NodeState.Success; // 목적지 도달 → 행동 완료
            }

            // 이번 프레임에 이동할 거리 계산
            float frameMoveDistance = speed * Time.deltaTime;

            // 2. 이번 프레임 이동량이 남은 거리보다 길 경우 (오버슈팅 방지)
            if (frameMoveDistance >= distanceToDest)
            {
                // 목적지를 지나치게 되므로, 그냥 목적지로 위치를 고정하고 성공 처리
                transform.position = _currentDest;
                return NodeState.Success;
            }
    
            // 3. 아직 이동 중인 일반적인 경우
            // 방향은 위에서 계산한 벡터를 정규화하여 사용
            transform.position += vectorToDest.normalized * frameMoveDistance;

            return NodeState.Running; // 아직 도달 안함 → 계속 실행 중
        }
    }
}