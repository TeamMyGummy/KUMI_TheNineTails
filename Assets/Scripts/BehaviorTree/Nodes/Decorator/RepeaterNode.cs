using System;
using UnityEngine;

namespace BehaviorTree.Decorator
{
    public class RepeaterNode : DecoratorNode
    {
        [Header("반복 횟수")]
        public int repeatCount = 3;
        
        [NonSerialized] private int currentCount = 0;

        public override NodeState Evaluate()
        {
            var child = GetChild();
            if (child == null) return NodeState.Failure;

            if (currentCount >= repeatCount)
            {
                currentCount = 0;
                return NodeState.Success;
            }

            var result = child.Evaluate();
            
            if (result != NodeState.Running)
            {
                currentCount++;
                Debug.Log("반복횟수 : " + currentCount);
                
                if (currentCount >= repeatCount)
                {
                    currentCount = 0;
                    return NodeState.Success;
                }
            }

            return NodeState.Running;
        }
    }
}