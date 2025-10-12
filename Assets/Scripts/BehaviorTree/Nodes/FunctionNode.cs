using UnityEngine;

namespace BehaviorTree
{
    /// <summary>
    /// SubBTGraph의 FunctionNode (RootNode와 동일하지만 구분을 위해)
    /// </summary>
    [NodeTint(NodeColorPalette.ROOT_NODE)]
    public class FunctionNode : BTNode
    {
        [Output] public BTNode child;
        [Output] public BTNode onAbort;
        
        [Tooltip("이 함수 노드의 고유 이름")]
        public string functionName;
        
        public override NodeState Evaluate()
        {
            var childNode = GetChild();
            if (childNode != null)
            {
                return childNode.Evaluate();
            }
            
            return NodeState.Failure;
        }
        
        public BTNode GetChild()
        {
            var port = GetOutputPort("child");
            return port != null && port.IsConnected
                ? port.Connection.node as BTNode
                : null;
        }
        
        public BTNode GetAbortNode()
        {
            var port = GetOutputPort("onAbort");
            return port != null && port.IsConnected
                ? port.Connection.node as BTNode
                : null;
        }
        
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(functionName))
            {
                functionName = $"Function_{GetInstanceID()}";
            }
        }
    }
}