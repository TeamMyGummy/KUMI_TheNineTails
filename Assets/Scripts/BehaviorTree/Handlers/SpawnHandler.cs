using UnityEngine;

namespace BehaviorTree
{
    public class SpawnHandler : ActionHandler
    {
        //봐서 노드로 옮겨도 될 듯
        [SerializeField] private GameObject spawnObject;
        private float delay;
        private Vector3 spawnPosition;

        public void SetPosition(Vector2 pos, float delay)
        {
            spawnPosition = pos;
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