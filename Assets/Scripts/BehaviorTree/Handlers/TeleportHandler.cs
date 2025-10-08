using BehaviorTree.Leaf;
using UnityEngine;

namespace BehaviorTree
{
    [RequireComponent(typeof(PositionHelper))]
    public class TeleportHandler : ActionHandler
    {
        private Vector3 _vector;
        private EPositionType _xtype;
        private EPositionType _ytype;
        private PositionHelper _positionHelper;
    
        private void Awake()
        {
            _positionHelper = GetComponent<PositionHelper>();
        }
        
        public void SetMovementPoint(Vector3 vector, EPositionType xtype, EPositionType ytype)
        {
            _vector = vector;
            _xtype = xtype;
            _ytype = ytype;
        }
        
        protected override NodeState OnStartAction()
        {
            Vector3 destination = new Vector3(
                _positionHelper.GetDestination(_xtype, transform, _vector).x, 
                _positionHelper.GetDestination(_ytype, transform, _vector).y);
            gameObject.transform.position = destination;
            return NodeState.Success;
        }

        protected override NodeState OnUpdateAction()
        {
            DebugUtil.AssertLog();
            return NodeState.Success;
        }
    }
}