using System;
using UnityEngine;

namespace BehaviorTree.Composite
{
    public class InputRunNode : CompositeNode
    {
        [Input] public int childIndex = -1;
        
        public override NodeState Evaluate()
        {
            if (childIndex == -1)
            {
                childIndex = GetInputValue<int>("childIndex") - 1;
            }
            var child = GetChild(childIndex);
            
            var result = child.Evaluate();
            
            switch (result)
            {
                case NodeState.Failure:
                    childIndex = -1;
                    return NodeState.Failure; 
                case NodeState.Running:
                    return NodeState.Running;
                case NodeState.Success:
                    childIndex = -1;
                    return NodeState.Success;
                default:
                    Debug.Log("Fail");
                    return NodeState.Failure;
            }
        }
    }
}