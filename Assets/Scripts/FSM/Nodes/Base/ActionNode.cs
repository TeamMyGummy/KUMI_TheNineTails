using UnityEngine;

public abstract class ActionNode : BaseNode
{
    [Input] public BaseNode input;
    
    //OnEnter 로직
    protected abstract void OnEnterAction();
    
    public override void OnEnter()
    {
        // TODO: 행동 시작 로직 구현
        OnEnterAction();
    }
    
    /// <summary>
    /// OnUpdate() <br/>
    /// 행동 완료 시 isCompleted = true로 설정 <br/>
    /// actionResult에 성공/실패 결과 설정
    /// </summary>
    protected abstract void OnExecuteAction();

    public override BaseNode Execute()
    {
        // TODO: 행동 실행 로직 구현
        OnExecuteAction();
            
        if (isCompleted)
        {
            var inputNode = GetInputBranch();
            return inputNode;
        }
        
        return this;
    }

    public override void OnExit()
    {
        result = true;
        isCompleted = false;
    }
    
    
    private BaseNode GetInputBranch()
    {
        var port = GetInputPort("input");
        return port?.IsConnected == true ? port.Connection.node as BaseNode : null;
    }
}