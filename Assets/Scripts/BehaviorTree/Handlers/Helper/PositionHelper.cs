using BehaviorTree.Leaf;
using UnityEngine;

namespace BehaviorTree
{
    //중복 코드가 다수 발생해 추가 클래스 만듦 = Helper
    public class PositionHelper : MonoBehaviour
    {
        private Transform _player;

        void Awake()
        {
            _player = GameObject.FindWithTag("Player").transform;
        }
        
        public Vector3 GetDestination(EPositionType type, Vector3 root, Vector3 offset)
        {
            switch (type)
            {
                case EPositionType.Offset:
                    return root + offset;
                case EPositionType.TargetOffset:
                    return _player.position + offset;
                case EPositionType.WorldLocation:
                    return offset;
                case EPositionType.CameraOffset:
                    return Camera.main.transform.position + offset;
                case EPositionType.Input:
                    return Vector3.zero;
            }
    
            return Vector3.zero;
        }
    }
}