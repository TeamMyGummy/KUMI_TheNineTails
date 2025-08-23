
//StartNode랑 코드 같음... 1번 더 이런 일 생기면 리팩토링
[CreateNodeMenu("FSM/End Node")]
public class EndNode : BaseNode
{
    [Output] public BaseNode nextNode;
    
    public BaseNode GetNextNode()
    {
        var port = GetOutputPort("nextNode");
        return port?.IsConnected == true ? port.Connection.node as BaseNode : null;
    }
    
    public override BaseNode Execute() => GetNextNode();
}