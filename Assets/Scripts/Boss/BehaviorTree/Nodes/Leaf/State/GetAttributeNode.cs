using UnityEngine;
using XNode;

namespace BehaviorTree.Leaf
{
    public class GetAttributeNode : Node
    {
        [SerializeField] private string attribute;
        [Output(connectionType = ConnectionType.Multiple)] public int value;

        public override object GetValue(NodePort port)
        {
            if (port.fieldName != "value") 
                return null;
            
            IBTGraph currentGraph = graph as IBTGraph;
            return (int) currentGraph.Context.ASC.Attributes[attribute].Value;
        }
    }
}