namespace BehaviorTree
{
    public class SetActiveHandler : ActionHandler
    {
        private bool _isActive;

        public void SetActiveState(bool isActive)
        {
            _isActive = isActive;
        }
        
        protected override NodeState OnStartAction()
        {
            gameObject.SetActive(_isActive);
            return NodeState.Success;
        }

        protected override NodeState OnUpdateAction()
        {
            DebugUtil.AssertLog();
            return NodeState.Success;
        }
    }
}