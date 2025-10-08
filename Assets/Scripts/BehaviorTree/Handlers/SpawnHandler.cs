using BehaviorTree.Leaf;
using UnityEngine;

namespace BehaviorTree
{
    public class SpawnHandler : ActionHandler
    {
        //봐서 노드로 옮겨도 될 듯
        [SerializeField] private GameObject spawnObject;
        private float delay;
        private Vector3 spawnPosition;
        private EPositionType _xtype;
        private EPositionType _ytype;
        private PositionHelper _positionHelper;
    
        private void Awake()
        {
            _positionHelper = GetComponent<PositionHelper>();
        }

        public void SetPosition(Vector2 inputPos, Vector2 desiredPos, EPositionType xtype, EPositionType ytype, float delay)
        {
            spawnPosition = inputPos;
            if (xtype != EPositionType.Input) spawnPosition.x = _positionHelper.GetDestination(_xtype, transform, desiredPos).x;
            if (ytype != EPositionType.Input) spawnPosition.y = _positionHelper.GetDestination(_ytype, transform, desiredPos).y;
            this.delay = delay; 
        }

        protected override NodeState OnStartAction()
        {
            var spawned = ResourcesManager.Instance.Instantiate(spawnObject);
            spawned.transform.position = spawnPosition;
            if (delay > 0f) ResourcesManager.Instance.Destroy(spawned, delay);
            return NodeState.Success; // 스폰은 즉시 완료
        }

        protected override NodeState OnUpdateAction()
        {
            return NodeState.Success;
        }
    }
}