
using UnityEngine;

public enum MoveType
{
    WorldLocation,
    CameraOffset,
    Offset,
    TargetOffset,
}

[CreateNodeMenu("FSM/Leaf/Move Node")]
public class MoveNode : MonoNode
{
    [SerializeField] public float x;
    [SerializeField] public MoveType xType;
    [SerializeField] public float y;
    [SerializeField] public MoveType yType;
    [SerializeField] public float speed;
    
    protected override void OnEnterAction()
    {
        if (runtimeHandler is MoveHandler handler)
        {
            handler.SetMovementPoint(new Vector3(x, y, 0f), xType, yType, speed);
        }

        base.OnEnterAction();
    }
}
