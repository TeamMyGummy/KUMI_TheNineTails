using UnityEngine;
using BehaviorTree.Leaf;

namespace BehaviorTree.Leaf
{
    public class SpawnNode : MonoNode
    {
        [Input] public Vector3 spawnPosition;
        [Output(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override)] public Vector3 location;
        [SerializeField] public float delay;
        [SerializeField] private GameObject spawnObject;
        
        [SerializeField] public float x;
        [SerializeField] public EPositionType xType;
        [SerializeField] public float y;
        [SerializeField] public EPositionType yType;

        protected override void OnEnter()
        {
            location = GetInputValue<Vector3>("spawnPosition", spawnPosition);
            
            if (runtimeHandler is SpawnHandler handler) 
                handler.SetPosition(location, new Vector2(x, y), xType, yType, delay, spawnObject);
        }
        
        public override object GetValue(XNode.NodePort port)
        {
            if (port.fieldName == "location") 
                return location;
            return null;
        }
    }
}