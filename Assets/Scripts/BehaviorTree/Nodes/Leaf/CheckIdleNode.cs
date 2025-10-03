using UnityEngine;

namespace BehaviorTree.Leaf
{
    [NodeTint(NodeColorPalette.STATE_NODE)]
    public class CheckIdleNode : LeafNode, IStateNode
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