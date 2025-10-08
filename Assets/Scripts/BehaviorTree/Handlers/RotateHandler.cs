using UnityEngine;

namespace BehaviorTree
{
    public class RotateHandler : ActionHandler
    {
        [SerializeField] private float speed = 10f;
        private float _angle;
        private RotationType _type;
    
        private Quaternion _targetRotation;

        public void SetRotation(float angle, RotationType type, float speed)
        {
            _angle = angle;
            _type = type;
            if (speed != 0f) this.speed = speed;
        }

        protected override NodeState OnStartAction()
        {
            float finalTargetAngle;

            if (_type == RotationType.Absolute)
            {
                finalTargetAngle = _angle;
            }
            else // _type == RotationType.OffSet
            {
                finalTargetAngle = transform.eulerAngles.z + _angle;
            }

            Vector3 currentEuler = transform.eulerAngles;
            _targetRotation = Quaternion.Euler(currentEuler.x, currentEuler.y, finalTargetAngle);
        
            if (speed < 0f)
            {
                transform.rotation = _targetRotation;
                return NodeState.Success;
            }
            return NodeState.Running;
        }

        protected override NodeState OnUpdateAction()
        {
            // 1. 이번 프레임에 회전할 최대 각도를 계산합니다.
            float step = speed * Time.deltaTime * 10;

            // 2. 목표 지점까지 회전시킵니다.
            transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, step);

            // 3. 목표에 거의 도달했는지 Quaternion.Angle로 확인합니다.
            if (Quaternion.Angle(transform.rotation, _targetRotation) < 0.01f)
            {
                transform.rotation = _targetRotation; // 오차 보정
                return NodeState.Success;
            }

            return NodeState.Running;
        }
    }
}