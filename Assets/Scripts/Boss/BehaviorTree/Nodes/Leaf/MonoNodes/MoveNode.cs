using BehaviorTree;
using UnityEngine;

namespace BehaviorTree.Leaf
{
    public enum EPositionType
    {
        Offset,
        TargetOffset,
        WorldLocation,
        CameraOffset,
        OffsetWithRootDirection,
        Input //Spawn 때문에 추가
    }
    public class MoveNode : MonoNode
    {
        [SerializeField] public float x;
        [SerializeField] public EPositionType xType;
        [SerializeField] public float y;
        [SerializeField] public EPositionType yType;
        [SerializeField] public float speed;

        protected override void OnEnter()
        {
            if (runtimeHandler is MoveHandler handler) 
                handler.SetMovementPoint(new Vector3(x, y, 0f), xType, yType, speed);
        }
    }
}