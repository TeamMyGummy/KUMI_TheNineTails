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
                _positionHelper.GetDestination(_xtype, transform.position, _vector).x, 
                _positionHelper.GetDestination(_ytype, transform.position, _vector).y);
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
    }
}