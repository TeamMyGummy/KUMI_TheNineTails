using UnityEngine;
using BehaviorTree.Leaf;

namespace BehaviorTree.Leaf
{
    public class TargetingNode : MonoNode
    {
        [Output(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override)] public Vector3 dest;
        protected override void OnEnter()
        {
            if (runtimeHandler is TargetingHandler handler)
            {
                dest = handler.GetTargetPosition();
                Debug.Log(dest);
            }
        }
        
        public override object GetValue(XNode.NodePort port)
        {
            if (port.fieldName == "dest") 
                return dest;
            return null;
        }
    }
}