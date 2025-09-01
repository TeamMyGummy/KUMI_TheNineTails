using UnityEngine;

namespace BehaviorTree.Leaf
{
    public class CheckIdleNode : LeafNode
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