using UnityEngine;

namespace BehaviorTree.Leaf
{
    public class TurnIdleCheckerInto : LeafNode
    [NodeTint(NodeColorPalette.STATE_NODE)]
    {
        public bool bTurnIdleState;
        protected override NodeState Start()
        {
            Context.bIdleState = bTurnIdleState;
            return State = NodeState.Success;
        }
        protected override NodeState Update()
        {
            Debug.Log("Fail");
            return State;
        }
    }
}