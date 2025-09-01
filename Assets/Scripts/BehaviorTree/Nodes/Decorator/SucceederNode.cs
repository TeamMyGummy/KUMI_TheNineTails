namespace BehaviorTree.Decorator
{
    public class SucceederNode : DecoratorNode
    {
        public override NodeState Evaluate()
        {
            var child = GetChild();
            if (child == null) return NodeState.Success;

            var result = child.Evaluate();
            return result == NodeState.Running ? NodeState.Running : NodeState.Success;
        }
    }
}