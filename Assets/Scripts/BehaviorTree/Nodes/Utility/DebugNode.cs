using BehaviorTree.Leaf;
using UnityEngine;

namespace BehaviorTree.Utility
{
    [NodeTint(NodeColorPalette.DEBUG_NODE)]
    public class DebugNode : LeafNode
    {
        [SerializeField] private string LogMessage;
        protected override NodeState Update()
        {
            Debug.Log(LogMessage);
            return NodeState.Success;
        }
    }
}