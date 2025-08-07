
    using UnityEngine;

    [CreateNodeMenu("FSM/Leaf/Spawn Node")]
    public class SpawnNode : MonoNode
    {
        [SerializeField] public float delay;
        [Input] public Vector3 spawnPosition;
        
        protected override void OnEnterAction()
        {
            Vector3 pos = GetInputValue<Vector3>("spawnPosition", spawnPosition);

            if (runtimeHandler is SpawnHandler handler)
            {
                handler.SetPosition(pos, delay);
            }

            base.OnEnterAction();
        }
    }
