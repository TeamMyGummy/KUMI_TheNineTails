using GameAbilitySystem;
using UnityEngine;
using XNode;

namespace BehaviorTree.Leaf
{
    [Node.NodeTint(NodeColorPalette.STATE_NODE)]
    public class ModifyAttributeNode : LeafNode
    {
        [SerializeField] private string attribute;
        [SerializeField] public int value;
        [SerializeField] public EModOperation operationType;
        protected override NodeState Start()
        {
            Context.ASC.Attributes[attribute].Modify(value, operationType);
            return NodeState.Success;
        }
        protected override NodeState Update()
        {
            throw new System.NotImplementedException();
        }
    }
}