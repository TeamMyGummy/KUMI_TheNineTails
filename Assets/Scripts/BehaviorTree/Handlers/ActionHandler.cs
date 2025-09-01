using UnityEngine;

namespace BehaviorTree
{
    public abstract class ActionHandler : MonoBehaviour
    {
        protected NodeState currentState = NodeState.Success;
        protected bool isExecuting = false;

        /// <summary>
        /// Called when the action starts
        /// </summary>
        public virtual NodeState StartAction()
        {
            isExecuting = true;
            currentState = NodeState.Running;
            return OnStartAction();
        }

        /// <summary>
        /// Called every frame while the action is running
        /// </summary>
        public virtual NodeState UpdateAction()
        {
            if (!isExecuting) return NodeState.Failure;
            
            currentState = OnUpdateAction();
            
            if (currentState != NodeState.Running)
            {
                EndAction();
            }
            
            return currentState;
        }

        /// <summary>
        /// Called when the action ends (success or failure)
        /// </summary>
        public virtual void EndAction()
        {
            isExecuting = false;
            OnEndAction();
        }

        /// <summary>
        /// Force stop the action
        /// </summary>
        public virtual void StopAction()
        {
            Debug.Log("ㄹㅇ?");
            if (isExecuting)
            {
                isExecuting = false;
                currentState = NodeState.Abort;
                OnStopAction();
            }
        }

        // Abstract methods to be implemented by concrete actions
        protected abstract NodeState OnStartAction();
        protected abstract NodeState OnUpdateAction();
        protected virtual void OnEndAction() { }
        protected virtual void OnStopAction() { }

        public bool IsExecuting => isExecuting;
        public NodeState CurrentState => currentState;
    }
}