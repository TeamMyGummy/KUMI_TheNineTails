[CreateNodeMenu("FSM/Branch Node")]
public class BranchNode : BaseNode
{
    [Input] public ActionNode input;
    [Output] public BaseNode trueOutput;
    [Output] public BaseNode falseOutput;
    
    public override BaseNode Execute()
    {
        if (result.HasValue)
        {
            var nextNode = result.Value ? GetTrueOutput() : GetFalseOutput();
            result = null; // 초기화
            return nextNode;
        }
        
        return this;
    }
    
    private BaseNode GetTrueOutput()
    {
        var port = GetOutputPort("trueOutput");
        return port?.IsConnected == true ? port.Connection.node as BaseNode : null;
    }
    
    private BaseNode GetFalseOutput()
    {
        var port = GetOutputPort("falseOutput");
        return port?.IsConnected == true ? port.Connection.node as BaseNode : null;
    }
}