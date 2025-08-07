
using UnityEngine;

public enum MoveType
{
    WorldLocation,
    Offset,
    TargetOffset,
}

[CreateNodeMenu("FSM/Move Node")]
public class MoveNode : MonoNode
{
    [SerializeField] public Vector3 vector;
    [SerializeField] public MoveType type;
    [SerializeField] public float speed;
    
    protected override void OnEnterAction()
    {
        if (runtimeHandler is MovementHandler)
        {
            MovementHandler handler = runtimeHandler as MovementHandler;
            handler.SetMovementPoint(vector, type, speed);
        }

        base.OnEnterAction();
    }
}
