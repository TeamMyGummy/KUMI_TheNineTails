using UnityEngine;

[CreateNodeMenu("FSM/Composite/Count Node")] 
public class CountNode : BaseNode
{
    [Input] public ActionNode input;
    [Output] public BranchNode next;
    [SerializeField] public int limit;
    private int count = 0;

    public override void OnEnter()
    {
        if (limit == count)
        {
            count = 0;
            GetNextNode().SetResult(false);
            return;
        }

        count++;
        result = true;
        GetNextNode().SetResult(true);
    }
    public override BaseNode Execute()
    {
        return GetNextNode();
    }
    
    private BaseNode GetNextNode()
    {
        var port = GetOutputPort("next");
        return port != null && port.IsConnected ? port.Connection.node as BaseNode : null;
    }
}
