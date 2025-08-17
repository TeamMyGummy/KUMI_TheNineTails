using UnityEngine;

[CreateNodeMenu("FSM/Composite/Branch Node")]
public class BranchNode : BaseNode
{
    [Input] public ActionNode input;
    [Output] public BaseNode trueOutput;
    [Output] public BaseNode falseOutput;
    
    public override BaseNode Execute()
    {
        result = GetInputBranch().GetResult();
        var nextNode = result ? GetTrueOutput() : GetFalseOutput();
        return nextNode;
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
    
    private BaseNode GetInputBranch()
    {
        var port = GetInputPort("input");
        return port?.IsConnected == true ? port.Connection.node as BaseNode : null;
    }
}