using UnityEngine;

namespace BehaviorTree
{
    public class TargetingHandler : ActionHandler
    {
        private Transform target;

        public void Awake()
        {
            target = GameObject.FindWithTag("Player").transform;
        }

        public Vector3 GetTargetPosition()
        {
            //효과발생
            return target.position;
        }

        public override NodeState OnStartAction()
        {
            return NodeState.Success; // 타겟팅은 즉시 완료
        }

        protected override NodeState OnUpdateAction()
        {
            return NodeState.Success;
        }
    }
}