using UnityEngine;

public enum RotationType
{
    Absolute,
    OffSet
}

namespace BehaviorTree.Leaf
{
    public class RotateNode : MonoNode
    {
        [SerializeField] public float angle;
        [SerializeField] public RotationType type;
        [SerializeField] public float speed;

        protected override void OnEnter()
        {
            if (runtimeHandler is RotateHandler handler) 
                handler.SetRotation(angle, type, speed);
        }
    }
}