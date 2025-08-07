using UnityEngine;
using XNode;
using System.Collections.Generic;

[CreateNodeMenu("FSM/Composite/Random Node")]
public class RandomNode : BaseNode
{
    [Input] public BaseNode input; // 이전 노드에서 진입
    [Output(dynamicPortList = true)] public List<BaseNode> children = new List<BaseNode>();

    private BaseNode selectedChild;

    public override void OnEnter()
    {
        // 연결된 자식 노드들 목록 가져오기
        var connectedChildren = new List<BaseNode>();
        for (int i = 0; i < children.Count; i++)
        {
            var port = GetOutputPort("children " + i);
            if (port != null && port.IsConnected)
            {
                connectedChildren.Add(port.Connection.node as BaseNode);
            }
        }

        if (connectedChildren.Count == 0)
        {
            Debug.LogWarning("RandomCompositeNode: 연결된 자식 노드가 없습니다.");
            selectedChild = null;
            return;
        }

        // 랜덤으로 하나 선택
        selectedChild = connectedChildren[Random.Range(0, connectedChildren.Count)];

        // 선택된 자식 노드에게 결과 초기화 및 OnEnter 호출
        selectedChild?.SetResult(true);
        selectedChild?.OnEnter();

        Debug.Log($"RandomCompositeNode: {selectedChild?.name} 노드 선택됨");
    }

    public override BaseNode Execute()
    {
        if (selectedChild == null)
        {
            // 자식이 없으면 자기 자신을 끝내고 null 반환 (또는 다른 기본 처리)
            return null;
        }

        var nextNode = selectedChild.Execute();
        if (nextNode != selectedChild)
        {
            selectedChild.OnExit();
            return nextNode;
        }

        return this; // 아직 실행 중이면 자기 자신 유지
    }
}