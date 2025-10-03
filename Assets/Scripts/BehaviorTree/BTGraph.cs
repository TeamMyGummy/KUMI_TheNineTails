using System;
using BehaviorTree.Leaf;
using UnityEngine;
using XNode;

namespace BehaviorTree
{
    [CreateAssetMenu(fileName = "BT Graph", menuName = "AI/BT Graph")]
    public class BTGraph : NodeGraph
    {
        public BTContext Context { get; set; }
        [NonSerialized] private BTNode _rootNode;
        [NonSerialized] private LeafNode _prevNode;
        
        public void StartGraph()
        {
            foreach (var node in nodes)
            {
                if (node is RootNode root)
                {
                    _rootNode = root;
                }

                if (node is LeafNode leaf)
                {
                    leaf.SetBTGraph(this);
                }
            }
        }

        public void StopGraph()
        {
            _rootNode = null;
        }

        public void Update()
        {
            if (_rootNode is null) return;

            _rootNode.Evaluate();
        }

        public void CheckPrevNode(LeafNode currNode)
        {
            if (currNode is not IStateNode) return;
            if (_prevNode is not null && _prevNode != currNode && _prevNode.State == NodeState.Running)
            {
                _prevNode.Abort();
            }
        }
    }
}