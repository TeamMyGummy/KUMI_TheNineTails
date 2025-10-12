using UnityEngine;
using XNode;

namespace BehaviorTree.Leaf
{
    [NodeTint(NodeColorPalette.STATE_NODE)]
    public class SetAttributeNode : LeafNode
    {
        [SerializeField] private string attribute;
        [SerializeField] public int value;
        protected override NodeState Start()
        {
            Context.ASC.Attributes[attribute].SetCurrentValue(value);
            return NodeState.Success;
        }
        protected override NodeState Update()
        {
            throw new System.NotImplementedException();
        }
    }
}