using UnityEngine;
using XNode;

namespace BehaviorTree.Pure 
{
    public class GetRandom : Node //순수 노드 - 값을 연산하여 반환만 함
    {
        [SerializeField] public float startRange;
        [SerializeField] public float endRange;
        [Output(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Multiple)] 
        public float value;
        
        public override object GetValue(XNode.NodePort port)
        {
            if (port.fieldName == "value") 
                return Random.Range(startRange, endRange);
            return null;
        }
    }
}