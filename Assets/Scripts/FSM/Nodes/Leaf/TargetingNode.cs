using UnityEngine;

[CreateNodeMenu("FSM/Leaf/Targeting Node")]
public class TargetingNode : MonoNode
{
    [Output] public Vector3 dest;

    protected override void OnEnterAction()
    {
        if (runtimeHandler is TargetingHandler handler)
        {
            dest = handler.GetTargetPosition();
        }

        base.OnEnterAction();
    }
    
    public override object GetValue(XNode.NodePort port)
    {
        if (port.fieldName == "dest") 
            return dest;
        return null;
    }
}
