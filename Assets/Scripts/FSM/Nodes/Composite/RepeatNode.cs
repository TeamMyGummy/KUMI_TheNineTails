using System;
using UnityEngine;
using XNode;

[CreateNodeMenu("FSM/Composite/Repeat Node")]
public class RepeatNode : CompositeNode
{
    [SerializeField] private int repeatCount = 1;

    private int currentCount;
    [NonSerialized] private BaseNode childNode;

    protected override void OnEnterAction()
    {
        currentCount = 0;
        result = true;
        childNode = GetChild(0);
        if (childNode != null)
        {
            childNode.SetResult(true); 
            childNode.OnEnter();
        }
        else
        {
            Debug.LogWarning("RepeatNode: 자식 노드가 연결되지 않았습니다.");
        }
    }
    
    protected override void OnExecuteAction()
    {
        if (childNode is null)
        {
            result = false;
            isCompleted = true;
            return;
        }
        
        var next = childNode.Execute();
        
        if (next != childNode)
        {
            childNode.OnExit();

            // 결과 확인
            bool childResult = true;
            if (childNode is BaseNode actionChild)
                childResult = actionChild.GetResult();

            if (!childResult)
            {
                // 실패 → 반복 종료
                result = false;
                isCompleted = true;
                return;
            }

            // 성공 → 카운트 증가
            currentCount++;
            if (currentCount < repeatCount)
            {
                // 다시 실행
                childNode.SetResult(true);
                childNode.OnEnter();
            }
            else
            {
                // 반복 완료
                result = true;
                isCompleted = true;
            }
        }
    }
}