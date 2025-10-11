using UnityEngine;

namespace BehaviorTree.Leaf
{
    [NodeTint(NodeColorPalette.STATE_NODE)]
    public class IfAttributeNode : LeafNode, IStateNode
    {
        [SerializeField] private int biggerThan;
        [SerializeField] private string attribute;

        protected override NodeState Start()
        {
            if (Context.ASC.Attributes[attribute].Value > biggerThan)
            {
                return NodeState.Success;
            }
            return NodeState.Failure;
        }

        protected override NodeState Update()
        {
            return State;
        }
    }
}