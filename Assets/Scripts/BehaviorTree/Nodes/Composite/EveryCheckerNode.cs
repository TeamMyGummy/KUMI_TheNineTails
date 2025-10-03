using UnityEngine;

namespace BehaviorTree.Composite
{
    //한 틱에 모든 자식을 순회하는데, 이때 Failure이 나오면 계속 자식 순회 아니면 멈춤
    [NodeTint(NodeColorPalette.COMPOSITE_NODE)]
    public class EveryCheckerNode : CompositeNode
    {
        public override NodeState Evaluate()
        {
            for (int currentChildIndex = 0; currentChildIndex < children.Count; currentChildIndex++)
            {
                // Execute only the current child
                var child = GetChild(currentChildIndex);
                if (child == null)
                {
                    continue;
                }
                NodeState result = child.Evaluate();
                DebugUtil.Log("노드의 반환값은 : " + result + " 현재 index는 : " + currentChildIndex);
            
                switch (result)
                {
                    case NodeState.Success: 
                        return NodeState.Success;
                    case NodeState.Running:
                        return NodeState.Running;
                    case NodeState.Abort:
                        return NodeState.Abort;
                
                    case NodeState.Failure:
                        continue;
                    default:
                        Debug.Log("Fail");
                        return NodeState.Failure;
                }
            }

            return NodeState.Failure;
        }
    }
}