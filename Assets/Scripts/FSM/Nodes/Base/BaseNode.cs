using XNode;

public abstract class BaseNode : Node
{
    protected bool? result = null;
    
    public void SetResult(bool actionResult)
    {
        result = actionResult;
    }
    public virtual void OnEnter() { }
    public virtual void OnExit() { }
    public abstract BaseNode Execute();
    
    public override object GetValue(NodePort port) => null;
}