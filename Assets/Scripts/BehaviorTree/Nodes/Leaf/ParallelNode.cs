using System.Collections.Generic;
using System.Linq;
using BehaviorTree.Leaf;
using Unity.VisualScripting;
using UnityEngine;
using Yarn;

namespace BehaviorTree.Leaf
{
    //아주 불안정함. Abort 쪽에서 오류날 수 있음
    public class ParallelNode : LeafNode
    [NodeTint(NodeColorPalette.COMPOSITE_NODE)]
    {
        [Output(dynamicPortList = true)] public List<LeafNode> children = new();
        private List<bool> results = new List<bool>();

        protected override void OnAbort()
        {
            for (int i = 0; i < children.Count; i++)
            {
                LeafNode child = GetChild(i);
                if (child == null) continue;
                if (child.State == NodeState.Running) child.Abort();
            }
            
            results.Clear();
        }

        protected override NodeState Update()
        {
            if (results.Count == 0)
            {
                for (int i = 0; i < children.Count; i++)
                {
                    results.Add(false);
                }
            }
            int runningCount = 0;

            for (int i = 0; i < children.Count; i++)
            {
                if (results[i])
                {
                    continue;
                }

                var child = GetChild(i);
                if (child == null) continue;

                var result = child.ParallelEvaluate();

                switch (result)
                {
                    case NodeState.Success:
                    case NodeState.Failure:
                        results[i] = true;
                        break;
                    case NodeState.Running:
                        runningCount++;
                        break;
                }
            }

            if (runningCount != 0) return NodeState.Running;

            results.Clear();
            
            return NodeState.Success;
        }

        protected LeafNode GetChild(int index)
        {
            var port = GetOutputPort("children " + index);
            return port != null && port.IsConnected
                ? port.Connection.node as LeafNode
                : null;
        }
    }
}