using UnityEngine;
using XNode;

public abstract class BaseNode : Node, IForceCancel
{
    protected bool result = true;
    protected bool isCompleted = false;
    
    [TextArea(2, 10)]
    [SerializeField] protected string NodeName = "Action";
    

    public bool GetResult()
    {
        return result;
    }
    public void SetResult(bool actionResult)
    {
        result = actionResult;
    }
    public virtual void OnEnter() { }
    public virtual void OnExit() { }
    public abstract BaseNode Execute();
    public override object GetValue(NodePort port) => null;
    
    // 외부에서 노드를 강제 종료할 때 사용
    public virtual void ForceComplete(bool success)
    {
        isCompleted = true;
        result = success;
    }
    
    // 현재 실행 중인지 확인
    public bool IsRunning() => !isCompleted;
}