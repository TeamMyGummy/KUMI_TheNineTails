
using UnityEngine;

namespace BehaviorTree
{
    public class FlipHandler : ActionHandler
    {
        private bool _isRight;

        public void SetDirectionState(bool isRight)
        {
            _isRight = isRight;
        }
        
        protected override NodeState OnStartAction()
        {
            Vector3 currentRotation = transform.eulerAngles;

            if (_isRight) currentRotation.y = 0f;
            else currentRotation.y = -180f;

            transform.eulerAngles = currentRotation;
            
            return NodeState.Success;
        }

        protected override NodeState OnUpdateAction()
        {
            DebugUtil.AssertLog();
            return NodeState.Success;
        }
    }
}