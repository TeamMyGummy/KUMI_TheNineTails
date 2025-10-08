using UnityEngine;

namespace BehaviorTree.Leaf
{
    public class SetActiveNode : MonoNode
    {
        [SerializeField] private bool setActive;
        protected override void OnEnter()
        {
            if (runtimeHandler is SetActiveHandler handler) 
                handler.SetActiveState(setActive);
        }
    }
}