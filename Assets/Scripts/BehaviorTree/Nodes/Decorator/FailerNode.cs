using UnityEngine;

namespace BehaviorTree.Decorator
{
    public class FailerNode : DecoratorNode
    {
        public override NodeState Evaluate()
        {
            var child = GetChild();
            if (child == null) return NodeState.Failure;

            var result = child.Evaluate();
            return result == NodeState.Running ? NodeState.Running : NodeState.Failure;
        }
    }
}