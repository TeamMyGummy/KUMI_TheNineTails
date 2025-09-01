using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BehaviorTree.Composite
{
    public class RandomNode : CompositeNode
    {
        [NonSerialized] private int currentIndex = -1;
        public override NodeState Evaluate()
        {
            if (currentIndex == -1)
            {
                currentIndex = Random.Range(0, children.Count);
            }

            NodeState result = GetChild(currentIndex).Evaluate();
            
            switch (result)
            {
                case NodeState.Success:
                case NodeState.Failure:
                    currentIndex = -1;
                    return result;
                case NodeState.Running:
                    return result;
                case NodeState.Abort:
                    currentIndex = -1;
                    return NodeState.Abort;
                default:
                    Debug.Log("Fail");
                    return NodeState.Failure;
            }
        }
    }
}