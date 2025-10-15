namespace BehaviorTree
{
    [NodeTint(NodeColorPalette.ROOT_NODE)]
    public class RootNode : BTNode
    {
        [Output] public BTNode child;
        public override NodeState Evaluate()
        {
            return GetChild().Evaluate();
        }
        
        protected BTNode GetChild()
        {
            var port = GetOutputPort("child");
            return port != null && port.IsConnected
                ? port.Connection.node as BTNode
                : null;
        }
    }
}