using UnityEngine;

namespace BehaviorTree.Leaf
{
    public class EndNode : LeafNode
    {
        protected override NodeState Start()
        {
            btGraph.StopGraph();
            return NodeState.Abort; 
        }
        
        protected override NodeState Update()
        {
            Debug.Log("Fail");
            return NodeState.Default;
        }
    }
}