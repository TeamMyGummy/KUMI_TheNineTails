using System;
using UnityEngine;

namespace BehaviorTree.Composite
{
    public class SelectorNode : CompositeNode
    {
        [NonSerialized] private int currentChildIndex = 0;

        public override NodeState Evaluate()
        {
            // Execute only the current child
            var child = GetChild(currentChildIndex);
            if (child == null)
            {
                // Move to next child if current is null
                currentChildIndex++;
                if (currentChildIndex >= children.Count)
                {
                    currentChildIndex = 0;
                    return NodeState.Failure;
                }
                return NodeState.Running; // Try next child in next tick
            }
            NodeState result = child.Evaluate();
            
            switch (result)
            {
                case NodeState.Success:
                    currentChildIndex = 0; // Reset for next evaluation
                    return NodeState.Success;
                
                case NodeState.Running:
                    // Stay on current child
                    return NodeState.Running;
                
                case NodeState.Failure:
                    // Move to next child
                    currentChildIndex++;
                    if (currentChildIndex >= children.Count)
                    {
                        currentChildIndex = 0; // Reset for next evaluation
                        return NodeState.Failure;
                    }
                    return NodeState.Running; // Try next child in next tick
                case NodeState.Abort:
                    currentChildIndex = 0;
                    return NodeState.Abort;
                default:
                    Debug.Log("Fail");
                    return NodeState.Failure;
            }
        }
    }
}