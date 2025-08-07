[CreateNodeMenu("FSM/Composite/Branch Node")]
public class BranchNode : BaseNode
{
    [Input] public ActionNode input;
    [Output] public BaseNode trueOutput;
    [Output] public BaseNode falseOutput;
    
    public override BaseNode Execute()
    {
            var nextNode = result ? GetTrueOutput() : GetFalseOutput();
            return nextNode;
        
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