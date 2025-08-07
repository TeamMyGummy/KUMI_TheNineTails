using UnityEngine;

public abstract class ActionHandler : MonoBehaviour
{
    public virtual void OnEnterAction() { }
    
    /// <returns>true면 종료</returns>
    public abstract bool OnExecuteAction();
    public virtual void OnExitAction() { }
}