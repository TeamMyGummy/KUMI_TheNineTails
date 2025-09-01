using UnityEngine;

namespace BehaviorTree.Leaf
{
    public class IfAttributeNode : LeafNode
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