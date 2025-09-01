using XNode;

namespace BehaviorTree.Decorator
{
    public abstract class DecoratorNode : BTNode
    {
        [Input] public BTNode input;
        [Output] public BTNode child;
        
        protected BTNode GetChild()
        {
            var port = GetOutputPort("child");
            return port != null && port.IsConnected
                ? port.Connection.node as BTNode
                : null;
        }
    }
}