using UnityEngine;
using XNode;
using System.Collections.Generic;
using UnityEngine.Serialization;
using Yarn.Unity;

//상황 봐서 sequential 처리하는 걸 Composite Node로 빼도...;
[CreateNodeMenu("FSM/Composite/Parallel Node")]
public class ParallelNode : BaseNode
{
    [Input] public BaseNode input;
    [Output] public BaseNode next;

    // true = 모든 자식 완료 시 종료, false = 하나라도 완료되면 종료
    [SerializeField] private bool finishRequireAll = true; 
    // true = 모든 자식이 true일 때 true 반환, false = 하나의 자식만 true여도 true 반환
    [SerializeField] private bool resultRequireAll = true;
    [SerializeField] private bool sequential = false;

    [Output(dynamicPortList = true)]
    public List<BaseNode> children = new List<BaseNode>();

    private List<BaseNode> runningChildren = new List<BaseNode>();
    private bool isCompleted = false;
    protected bool actionResult = true;

    public override void OnEnter()
    {
        runningChildren.Clear();
        isCompleted = false;
        actionResult = resultRequireAll;

        if (sequential && result == false)
        {
            isCompleted = true;
            actionResult = false;
            return;
        }
        
        // 연결된 자식 모두 실행 시작
        for (int i = 0; i < children.Count; i++)
        {
            var port = GetOutputPort("children " + i);
            if (port != null && port.IsConnected)
            {
                var child = port.Connection.node as BaseNode;
                if (child != null)
                {
                    runningChildren.Add(child);
                    child.SetResult(true); // 초기값 전달
                    child.OnEnter();
                }
            }
        }

        if (runningChildren.Count == 0)
        {
            Debug.LogWarning("ParallelNode: 실행할 자식 노드가 없습니다.");
            isCompleted = true;
        }
    }

    public override BaseNode Execute()
    {
        if (isCompleted)
        {
            var nextBranch = GetNextNode();
            nextBranch?.SetResult(actionResult);
            Debug.Log("다음 노드로 넘어갑니다" + nextBranch);
            return GetNextNode();
        }

        for (int i = runningChildren.Count - 1; i >= 0; i--)
        {
            var child = runningChildren[i];
            var next = child.Execute();

            // 자식이 자기 자신이 아닌 다른 노드를 반환 → 종료된 것으로 판단
            if (next != child)
            {
                child.OnExit();
                if (child is ActionNode)
                {
                    var c = child as ActionNode;
                    if (resultRequireAll) actionResult = actionResult && c.GetResult();
                    else actionResult = actionResult || c.GetResult();
                }
                runningChildren.RemoveAt(i);

                // 종료 조건 검사
                if (!finishRequireAll || runningChildren.Count == 0)
                {
                    isCompleted = true;
                    break;
                }
            }
        }

        return this;
    }

    private BaseNode GetNextNode()
    {
        var port = GetOutputPort("next");
        return port != null && port.IsConnected ? port.Connection.node as BaseNode : null;
    }
}
