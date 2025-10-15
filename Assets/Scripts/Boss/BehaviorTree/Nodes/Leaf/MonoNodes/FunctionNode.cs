using UnityEngine;

namespace BehaviorTree.Leaf
{
    public class FunctionNode : MonoNode
    {
        [SerializeField] private string functionName;
        [SerializeField] protected bool waitForEnd;
        [SerializeField] protected float completeDelay = 0f;
        protected override void OnEnter()
        {
            if (runtimeHandler is FunctionHandler handler)
            {
                handler.ChooseFunction(functionName, waitForEnd, completeDelay);
            }
        }
    }
}