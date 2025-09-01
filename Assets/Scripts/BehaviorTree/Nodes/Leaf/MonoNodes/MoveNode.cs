using BehaviorTree;
using UnityEngine;

namespace BehaviorTree.Leaf
{
    public enum EMoveType
    {
        Offset,
        TargetOffset,
        WorldLocation,
        CameraOffset
    }
    public class MoveNode : MonoNode
    {
        [SerializeField] public float x;
        [SerializeField] public EMoveType xType;
        [SerializeField] public float y;
        [SerializeField] public EMoveType yType;
        [SerializeField] public float speed;

        protected override void OnEnter()
        {
            if (runtimeHandler is MoveHandler handler) 
                handler.SetMovementPoint(new Vector3(x, y, 0f), xType, yType, speed);
        }
    }
}