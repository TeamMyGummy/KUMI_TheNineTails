using System;
using Unity.VisualScripting;
using UnityEngine;

namespace BehaviorTree
{
    public class LayerTriggerHandler : ActionHandler
    {
        [SerializeField] private string layerName;
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
            if (other.gameObject.layer == LayerMask.NameToLayer(layerName))
            {
                bTriggered = true;
                Debug.Log("Trigger됨" + name);
            }
        }
    }
}