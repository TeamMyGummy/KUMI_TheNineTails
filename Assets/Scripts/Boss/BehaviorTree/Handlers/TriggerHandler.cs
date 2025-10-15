using System;
using Unity.VisualScripting;
using UnityEngine;

namespace BehaviorTree
{
    public class TriggerHandler : ActionHandler
    {
        [SerializeField] private string targetTag;
        private bool bTriggered = false;

        public override NodeState OnStartAction()
        {
            var state = bTriggered ? NodeState.Success : NodeState.Failure;
            bTriggered = false;
            return state;
        }

        protected override NodeState OnUpdateAction()
        {
            Debug.Break();
            return NodeState.Failure;
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag(targetTag))
            {
                bTriggered = true;
            }
        }
    }
}