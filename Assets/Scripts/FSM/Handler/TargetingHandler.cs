using UnityEngine;

public class TargetingHandler : ActionHandler
{
    private Transform target;
    public void Awake()
    {
        target = GameObject.FindWithTag("Player").transform;
    }

    public Vector3 GetTargetPosition()
    {
        //효과발생
        return target.position;
    }
    public override bool OnExecuteAction()
    {
        return true;
    }
}
