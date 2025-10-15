using BehaviorTree.Leaf;
using UnityEngine;

namespace BehaviorTree
{
    //중복 코드가 다수 발생해 추가 클래스 만듦 = Helper
    public class PositionHelper : MonoBehaviour
    {
        [SerializeField] private Transform _target;

        void Awake()
        {
            if(_target == null)
                _target = GameObject.FindWithTag("Player").transform;
        }
        
        public Vector3 GetDestination(EPositionType type, Transform current, Vector3 offset)
        {
            switch (type)
            {
                case EPositionType.Offset:
                    return current.position + offset;
                case EPositionType.TargetOffset:
                    return _target.position + offset;
                case EPositionType.WorldLocation:
                    return offset;
                case EPositionType.CameraOffset:
                    return Camera.main.transform.position + offset;
                case EPositionType.Input:
                    return Vector3.zero;
                case EPositionType.OffsetWithRootDirection:
                    return current.position + current.rotation * offset;
            }
    
            return Vector3.zero;
        }
    }
}