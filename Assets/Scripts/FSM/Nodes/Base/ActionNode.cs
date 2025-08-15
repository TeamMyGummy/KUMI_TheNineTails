using UnityEngine;

public abstract class ActionNode : BaseNode
{
    [Input] public BaseNode input;
    [Output] public BaseNode nextBranch;
    
    [TextArea(2, 10)]
    [SerializeField] private string actionName = "Action";
    [SerializeField] private bool sequential = false;
    
    protected bool isCompleted = false;
    
    //OnEnter 로직
    protected abstract void OnEnterAction();
    
    public override void OnEnter()
    {
        if (sequential && !result)
        {
            isCompleted = true;
            GetNextBranch().SetResult(false);
            return;
        }
        
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
            var nextBranch = GetNextBranch();
            nextBranch?.SetResult(result);
            Debug.Log("다음 노드로 넘어갑니다" + nextBranch);
            return nextBranch;
        }
        
        return this;
    }

    public override void OnExit()
    {
        result = true;
        isCompleted = false;
    }
    
    // 외부에서 노드를 강제 종료할 때 사용
    public virtual void ForceComplete(bool success)
    {
        isCompleted = true;
        result = success;
    }
    
    // 현재 실행 중인지 확인
    public bool IsRunning() => !isCompleted;
    
    private BaseNode GetNextBranch()
    {
        var port = GetOutputPort("nextBranch");
        return port?.IsConnected == true ? port.Connection.node as BaseNode : null;
    }
}