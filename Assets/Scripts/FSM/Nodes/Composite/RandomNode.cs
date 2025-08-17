using UnityEngine;
using XNode;
using System.Collections.Generic;

[CreateNodeMenu("FSM/Composite/Random Node")]
public class RandomNode : BaseNode
{
    [Input] public BaseNode input;
    [Output] public BaseNode next;
    [Output(dynamicPortList = true)] public List<BaseNode> children = new();
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
    }

    public override BaseNode Execute()
    {
        return selectedChild;
    }
}