using System;
using UnityEngine;

namespace BehaviorTree.Composite
{
    public class SequenceNode : CompositeNode
    {
        [NonSerialized] private int currentChildIndex = 0;

        public override NodeState Evaluate()
        {
            // Execute only the current child
            var child = GetChild(currentChildIndex);
            if (child == null)
            {
                Debug.LogWarning("[SequenceNode] children 포트에 연결된 노드가 없습니다. ");
                // Move to next child if current is null
                currentChildIndex++;
                if (currentChildIndex >= children.Count)
                {
                    currentChildIndex = 0;
                    return NodeState.Success; // All children completed
                }
                return NodeState.Running; // Continue to next child in next tick
            }

            var result = child.Evaluate();
            
            switch (result)
            {
                case NodeState.Failure:
                    currentChildIndex = 0; // Reset for next evaluation
                    return NodeState.Failure;
                
                case NodeState.Running:
                    // Stay on current child
                    return NodeState.Running;
                
                case NodeState.Success:
                    // Move to next child
                    currentChildIndex++;
                    if (currentChildIndex >= children.Count)
                    {
                        currentChildIndex = 0; // Reset for next evaluation
                        return NodeState.Success; // All children succeeded
                    }
                    return NodeState.Running; // Continue to next child in next tick
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