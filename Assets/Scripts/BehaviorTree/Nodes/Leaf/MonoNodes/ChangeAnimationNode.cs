using UnityEngine;

public enum EAnimationControl
{
    Set,
    Add
}

namespace BehaviorTree.Leaf
{
    public class ChangeAnimationNode : MonoNode
    {
        [SerializeField] public string animationName;
        [SerializeField] public EAnimationControl howToPlay;
        [SerializeField] public bool loop;
        [SerializeField] public float delay = 0f;

        protected override void OnEnter()
        {
            if (runtimeHandler is BossAnimationHandler handler) 
                handler.ChooseAnimation(animationName, howToPlay, loop, delay);
        }
    }
}