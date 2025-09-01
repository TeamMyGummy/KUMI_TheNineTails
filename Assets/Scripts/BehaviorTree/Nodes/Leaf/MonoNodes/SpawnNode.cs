using UnityEngine;
using BehaviorTree.Leaf;

namespace BehaviorTree.Leaf
{
    public class SpawnNode : MonoNode
    {
        [Input] public Vector3 spawnPosition;
        [SerializeField] public float delay;

        protected override void OnEnter()
        {
            Vector3 pos = GetInputValue<Vector3>("spawnPosition", spawnPosition);
            
            Debug.Log(pos);
            
            if (runtimeHandler is SpawnHandler handler) 
                handler.SetPosition(pos, delay);
        }
    }
}