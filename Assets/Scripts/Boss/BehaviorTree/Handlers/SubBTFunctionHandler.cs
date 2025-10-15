using UnityEngine;

namespace BehaviorTree
{
    public class SubBTFunctionHandler : ActionHandler
    {
        [SerializeField] private SubBTController subBTController;
        
        private string _functionName;
        private bool _waitForCompletion;
        private bool _functionStarted;

        public void SetFunctionInfo(string functionName, bool waitForCompletion)
        {
            _functionName = functionName;
            _waitForCompletion = waitForCompletion;
        }

        public override NodeState OnStartAction()
        {
            if (subBTController == null)
            {
                return NodeState.Failure;
            }

            if (!subBTController.IsInitialized)
            {
                return NodeState.Failure;
            }

            if (string.IsNullOrEmpty(_functionName))
            {
                return NodeState.Failure;
            }

            if (!subBTController.HasFunction(_functionName))
            {
                Debug.LogError($"[SubBTFunctionHandler] Function '{_functionName}' not found in SubBTController");
                return NodeState.Failure;
            }

            bool started = subBTController.StartFunction(_functionName);
            if (!started)
            {
                return NodeState.Failure;
            }

            _functionStarted = true;

            if (!_waitForCompletion)
            {
                // 함수 시작만 하고 바로 성공 반환
                return NodeState.Success;
            }

            return NodeState.Running;
        }

        protected override NodeState OnUpdateAction()
        {
            if (!_waitForCompletion)
            {
                return NodeState.Success;
            }

            // 함수가 아직 실행 중인지 확인
            if (subBTController.IsFunctionActive(_functionName))
            {
                return NodeState.Running;
            }

            // 함수가 완료됨
            Debug.Log($"[SubBTFunctionHandler] Function '{_functionName}' completed");
            return NodeState.Success;
        }
    }
}