using UnityEngine;
using XNode;
using System.Collections.Generic;
using UnityEngine.Serialization;
using Yarn.Unity;

[CreateNodeMenu("FSM/Composite/Parallel Node")]
public class ParallelNode : CompositeNode
{
    // true = 모든 자식 완료 시 종료, false = 하나라도 완료되면 종료
    [SerializeField] private bool finishRequireAll = true; 
    // true = 모든 자식이 true일 때 true 반환, false = 하나의 자식만 true여도 true 반환
    [SerializeField] private bool resultRequireAll = true;

    private List<BaseNode> runningChildren = new List<BaseNode>();
    protected bool actionResult = true;

    protected override void OnEnterAction()
    {
        runningChildren.Clear();
        actionResult = resultRequireAll;
        
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

    protected override void OnExecuteAction() 
    {
        if (isCompleted)
        {
            var nextBranch = GetNextNode(); 
            nextBranch?.SetResult(actionResult);
            Debug.Log("다음 노드로 넘어갑니다" + nextBranch);
            result = true;
            isCompleted = true;
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
                    result = true;
                    isCompleted = true;
                    break;
                }
            }
        }
    }
}
