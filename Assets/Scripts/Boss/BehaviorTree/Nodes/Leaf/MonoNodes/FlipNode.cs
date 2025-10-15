using UnityEngine;

namespace BehaviorTree.Leaf
{
    public class FlipNode : MonoNode
    {
        [SerializeField] private bool isRight;
        protected override void OnEnter()
        {
            if (runtimeHandler is FlipHandler handler) 
                handler.SetDirectionState(isRight);
        }
    }
}