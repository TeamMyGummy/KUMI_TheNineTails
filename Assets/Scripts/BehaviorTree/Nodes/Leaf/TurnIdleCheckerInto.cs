using UnityEngine;

namespace BehaviorTree.Leaf
{
    [NodeTint(NodeColorPalette.STATE_NODE)]
    public class TurnIdleCheckerInto : LeafNode, IStateNode
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