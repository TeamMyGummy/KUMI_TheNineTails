using UnityEngine;

namespace BehaviorTree
{
    public class FollowingHandler : ActionHandler
    {
        [Header("Node Settings")] [Tooltip("스폰할 오브젝트의 프리팹입니다.")]
        public GameObject prefabToSpawn;

        [Tooltip("따라다닐 시간(초)입니다. 이 시간이 지나면 스폰된 오브젝트는 파괴됩니다.")]
        public float duration = 5.0f;

        private GameObject spawnedObject;
        private Transform target;
        private float timer;

        void Awake()
        {
            target = GameObject.FindWithTag("Player").transform;
        }

        protected override NodeState OnStartAction()
        {
            Vector3 position = target.position;

            spawnedObject = Instantiate(prefabToSpawn, position, Quaternion.identity);

            timer = duration;
            return NodeState.Running;
        }

        protected override NodeState OnUpdateAction()
        {
            if (spawnedObject == null)
            {
                return NodeState.Failure;
            }

            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                Destroy(spawnedObject);
                return NodeState.Success;
            }

            spawnedObject.transform.position = target.position;

            return NodeState.Running;
        }

        protected override void OnEndAction()
        {
            if (spawnedObject != null)
            {
                Destroy(spawnedObject);
            }

            spawnedObject = null;
        }

        protected override void OnStopAction()
        {
            if (spawnedObject != null)
            {
                Destroy(spawnedObject);
            }

            spawnedObject = null;
        }
}
}