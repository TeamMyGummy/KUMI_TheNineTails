namespace BehaviorTree.Decorator
{
    [NodeTint(NodeColorPalette.DECORATOR_NODE)]
    public class InverterNode : DecoratorNode
    {
        public override NodeState Evaluate()
        {
            var child = GetChild();
            if (child == null) return NodeState.Failure;

            var result = child.Evaluate();
            
            switch (result)
            {
                case NodeState.Success:
                    return NodeState.Failure;
                case NodeState.Failure:
                    return NodeState.Success;
                case NodeState.Running:
                    return NodeState.Running;
                default:
                    return NodeState.Failure;
            }
        }
    }
}