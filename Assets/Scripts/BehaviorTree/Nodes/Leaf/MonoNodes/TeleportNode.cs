using UnityEngine;

namespace BehaviorTree.Leaf
{
    public class TeleportNode : MonoNode
    {
        //[강승연] x, y의 Input을 받아서 조정하는 무언가가 또 생기면 전체 리팩토링
        [SerializeField][Input] public float x;
        [SerializeField] public EPositionType xType;
        [SerializeField] public float y;
        [SerializeField] public EPositionType yType;
        
        protected override void OnEnter()
        {
            if (GetPort("x").IsConnected)
            {
                x = GetInputValue<float>("x");
            }
            if (runtimeHandler is TeleportHandler handler) 
                handler.SetMovementPoint(new Vector3(x, y, 0f), xType, yType);
        }
    }
}