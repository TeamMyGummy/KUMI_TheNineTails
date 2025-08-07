using UnityEngine;

[CreateNodeMenu("FSM/Composite/Repeat Node")]
public class RepeatNode : BaseNode
{
    [Input] public BaseNode input; // 이전 노드 연결
    [Input] public BaseNode child; // 반복할 자식 노드 연결
    [Output] public BaseNode next; // 반복이 끝나면 이동할 노드

    [SerializeField] private int repeatCount = 0; // 0 = 무한 반복
    [SerializeField] private float delay = 0f; // 반복 간 대기 시간(초)

    private int currentCount;
    private float delayTimer;
    private bool waiting;
    private bool isCompleted;
    private bool actionResult = true;

    public override void OnEnter()
    {
        isCompleted = false;
        currentCount = 0;
        waiting = false;
        delayTimer = 0f;
        actionResult = true;

        var childNode = GetChildNode();
        if (childNode != null)
        {
            childNode.SetResult(true);
            childNode.OnEnter();
        }
        else
        {
            Debug.LogWarning("RepeatNode: 연결된 자식 노드가 없습니다.");
            isCompleted = true;
        }
    }

    public override BaseNode Execute()
    {
        if (isCompleted)
        {
            var nextBranch = GetNextNode();
            nextBranch?.SetResult(actionResult);
            return nextBranch;
        }

        if (waiting)
        {
            delayTimer -= Time.deltaTime;
            if (delayTimer <= 0f)
            {
                waiting = false;
                var childNode = GetChildNode();
                if (childNode != null)
                {
                    childNode.SetResult(true);
                    childNode.OnEnter();
                }
            }
            return this;
        }

        var childNodeExec = GetChildNode();
        if (childNodeExec != null)
        {
            var nextNode = childNodeExec.Execute();

            if (nextNode != childNodeExec) // 자식 종료 시
            {
                childNodeExec.OnExit();

                // 자식 결과 반영
                actionResult &= childNodeExec.GetResult();

                currentCount++;

                // 반복 종료 조건
                if (repeatCount > 0 && currentCount >= repeatCount)
                {
                    isCompleted = true;
                    return this;
                }

                // delay 대기
                if (delay > 0f)
                {
                    waiting = true;
                    delayTimer = delay;
                }
                else
                {
                    // 즉시 다음 반복
                    childNodeExec.SetResult(true);
                    childNodeExec.OnEnter();
                }
            }
        }

        return this;
    }

    private BaseNode GetChildNode()
    {
        var port = GetInputPort("child");
        return port != null && port.IsConnected ? port.Connection.node as BaseNode : null;
    }

    private BaseNode GetNextNode()
    {
        var port = GetOutputPort("next");
        return port != null && port.IsConnected ? port.Connection.node as BaseNode : null;
    }
}
