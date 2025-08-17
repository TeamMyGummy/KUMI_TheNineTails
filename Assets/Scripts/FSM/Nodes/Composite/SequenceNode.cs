using UnityEngine;
using System.Collections.Generic;

[CreateNodeMenu("FSM/Composite/Sequence Node")]
public class SequenceNode : CompositeNode
{
    private int currentIndex;

    private BaseNode childNode;

    protected override void OnEnterAction()
    {
        currentIndex = 0;
        result = true;

        if (children.Count == 0)
        {
            result = true;
            isCompleted = true;
            Debug.LogWarning("SequenceNode: 자식 노드가 없습니다.");
        }
        else
        {
            childNode = GetChild(currentIndex);
            if (childNode != null)
            {
                childNode.SetResult(true); // 초기 상태
                childNode.OnEnter();
            }
        }
    }
    
    protected override void OnExecuteAction() 
    {
        var next = childNode.Execute();
        
        if (next != childNode)
        {
            childNode.OnExit();

            bool childResult = true;
            childResult = childNode.GetResult(); 

            if (!childResult)
            {
                // 실패 → 종료
                result = false;
                isCompleted = true;
                return;
            }

            // 다음 자식으로
            currentIndex++;
            if (currentIndex >= children.Count)
            {
                result = true;
                isCompleted = true;
                return;
            }

            // 다음 자식 시작
            var nextChild = GetChild(currentIndex);
            if (nextChild != null)
            {
                childNode = nextChild;
                nextChild.SetResult(true);
                nextChild.OnEnter();
            }
        }
    }
}
