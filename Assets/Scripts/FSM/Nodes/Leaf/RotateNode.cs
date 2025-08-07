using UnityEngine;

public enum RotationType
{
    Absolute, //절대각도
    OffSet, //상대각도
}
[CreateNodeMenu("FSM/Leaf/Rotate Node")]
public class RotateNode : MonoNode{
    
    //angle 기준은 육십분법을 따름(라디안 X!!)
    [SerializeField] private float angle;
    [SerializeField] private RotationType type;
    [SerializeField] private float speed;
    protected override void OnEnterAction()
    {
        if (runtimeHandler is RotateHandler handler)
        {
            handler.SetRotation(angle, type, speed);
        }

        base.OnEnterAction();
    }
}
