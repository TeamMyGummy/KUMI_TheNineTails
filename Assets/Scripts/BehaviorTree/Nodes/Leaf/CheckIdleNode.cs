using UnityEngine;

namespace BehaviorTree.Leaf
{
    public class CheckIdleNode : LeafNode
    [NodeTint(NodeColorPalette.STATE_NODE)]
    {
        protected override NodeState Start()
        {
            return State = Context.bIdleState ? NodeState.Success : NodeState.Failure;
        }
        protected override NodeState Update()
        {
            Debug.Log("Fail");
            return State;
        }
    }
}