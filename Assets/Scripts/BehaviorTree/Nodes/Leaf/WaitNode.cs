using System;
using UnityEngine;

namespace BehaviorTree.Leaf
{
    public class WaitNode : LeafNode
    {
        [Header("Wait Settings")]
        [SerializeField] public float waitTime = 1.0f;
        
        [NonSerialized] private float _startTime;
        [NonSerialized] private float _elapsedTime;

        protected override NodeState Start()
        {
            _startTime = Time.time;
            _elapsedTime = 0f;
            return State = NodeState.Running;
        }

        protected override NodeState Update()
        {
            _elapsedTime = Time.time - _startTime;
            
            if (_elapsedTime >= waitTime)
            {
                return State = NodeState.Success;
            }
            
            return State = NodeState.Running;
        }

        protected override void OnAbort()
        {
            // Wait 중 중단되면 타이머 리셋
            _elapsedTime = 0f;
        }
    }
}