using System.Collections.Generic;
using UnityEngine;
using XNode;

public abstract class CompositeNode : BaseNode
{
    [Input] public BaseNode input;
    [Output] public BaseNode next;
    [Output(dynamicPortList = true)] public List<BaseNode> children = new();

    //OnEnter 로직
    protected abstract void OnEnterAction();
    
    public override void OnEnter()
    {
        // TODO: 행동 시작 로직 구현
        isCompleted = false;
        OnEnterAction();
    }
    
    protected abstract void OnExecuteAction();

    public override BaseNode Execute()
    {
        if (isCompleted)
        {
            var nextNode = GetNextNode();
            return nextNode;
        }
        
        // TODO: 행동 실행 로직 구현
        OnExecuteAction();
        
        return this;
    }
    
    protected bool GetChildResult(int index)
    {
        return GetChild(index).GetResult();
    }
    
    protected BaseNode GetChild(int index)
    {
        var port = GetOutputPort("children " + index);
        return port != null && port.IsConnected
            ? port.Connection.node as BaseNode
            : null;
    }
    
    protected BaseNode GetNextNode()
    {
        var port = GetOutputPort("next");
        return port != null && port.IsConnected ? port.Connection.node as BaseNode : null;
    }
}