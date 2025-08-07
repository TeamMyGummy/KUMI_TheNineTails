using UnityEngine;
using XNode;

[CreateNodeMenu("FSM/Leaf/Mono Action")]
public class MonoNode : ActionNode
{
    [SerializeField]
    public string handlerKey; // 런타임에 사용할 키

    [System.NonSerialized]
    public ActionHandler runtimeHandler;

    protected override void OnEnterAction()
    {
        if (runtimeHandler == null)
        {
            Debug.LogError($"[MonoActionNode] 실행 핸들러({handlerKey})가 주입되지 않았습니다.");
            isCompleted = true;
            result = false;
            return;
        }

        runtimeHandler.OnEnterAction();
    }

    protected override void OnExecuteAction()
    {
        if (runtimeHandler == null)
        {
            isCompleted = true;
            result = false;
            return;
        }

        bool done = runtimeHandler.OnExecuteAction();
        if (done)
        {
            runtimeHandler.OnExitAction();
            isCompleted = true;
            result = true;
        }
    }
}