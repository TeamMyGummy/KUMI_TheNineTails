using UnityEngine;

namespace BehaviorTree.Leaf
{
    public class TeleportNode : MonoNode
    {
        [SerializeField] public float x;
        [SerializeField] public EPositionType xType;
        [SerializeField] public float y;
        [SerializeField] public EPositionType yType;
        
        protected override void OnEnter()
        {
            if (runtimeHandler is TeleportHandler handler) 
                handler.SetMovementPoint(new Vector3(x, y, 0f), xType, yType);
        }
    }
}