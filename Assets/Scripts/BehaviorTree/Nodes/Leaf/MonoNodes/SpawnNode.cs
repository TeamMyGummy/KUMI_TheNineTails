using UnityEngine;
using BehaviorTree.Leaf;

namespace BehaviorTree.Leaf
{
    public class SpawnNode : MonoNode
    {
        [Input] public Vector3 spawnPosition;
        [SerializeField] public float delay;
        
        [SerializeField] public float x;
        [SerializeField] public EPositionType xType;
        [SerializeField] public float y;
        [SerializeField] public EPositionType yType;

        protected override void OnEnter()
        {
            Vector3 pos = GetInputValue<Vector3>("spawnPosition", spawnPosition);
            
            if (runtimeHandler is SpawnHandler handler) 
                handler.SetPosition(pos, new Vector2(x, y), xType, yType, delay);
        }
    }
}