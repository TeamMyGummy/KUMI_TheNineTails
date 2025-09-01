using System.Collections.Generic;

namespace BehaviorTree.Composite
{
    //상태가 존재할 시 자식이 Abort일 때 체크해서 리셋시켜야 함
    public abstract class CompositeNode : BTNode
    {
        [Input] public BTNode input;
        [Output(dynamicPortList = true)] public List<BTNode> children = new List<BTNode>();

        protected BTNode GetChild(int index)
        {
            var port = GetOutputPort("children " + index);
            return port != null && port.IsConnected
                ? port.Connection.node as BTNode
                : null;
        }
    }
}