[CreateNodeMenu("FSM/Start Node")]
public class StartNode : BaseNode
{
    [Output] public BaseNode nextNode;
    
    public BaseNode GetNextNode()
    {
        var port = GetOutputPort("nextNode");
        return port?.IsConnected == true ? port.Connection.node as BaseNode : null;
    }
    
    public override BaseNode Execute() => GetNextNode();
}