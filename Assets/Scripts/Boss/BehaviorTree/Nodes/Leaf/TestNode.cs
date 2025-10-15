using UnityEngine;

namespace BehaviorTree.Leaf
{
    public class TestNode : LeafNode
    {
        public string print;

        protected override void OnAbort()
        {
            throw new System.NotImplementedException();
        }

        protected override NodeState Update()
        {
            Debug.Log(print);
            return NodeState.Success;
        }
    }
}