using UnityEngine;

[CreateNodeMenu("FSM/Wait Action")]
public class WaitNode : ActionNode
{
    [SerializeField] private float delay;
    private float timer;
    protected override void OnEnterAction()
    {
        timer = 0f;
    }

    protected override void OnExecuteAction()
    {
        timer += Time.deltaTime;
        if (timer > delay)
        {
            isCompleted = true;
            actionResult = true;
        }
    }
}
