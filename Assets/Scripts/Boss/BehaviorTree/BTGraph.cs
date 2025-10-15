using System;
using System.Collections.Generic;
using BehaviorTree.Leaf;
using UnityEngine;
using UnityEngine.Serialization;
using XNode;

namespace BehaviorTree
{
    [CreateAssetMenu(fileName = "BT Graph", menuName = "AI/BT Graph")]
    public class BTGraph : NodeGraph, IBTGraph
    {
        public BTContext Context { get; set; }
        public bool IsRunning { get; private set; } = false;
        [NonSerialized] private RootNode _rootNode;
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

            IsRunning = true;
        }

        public void StopGraph()
        {
            _rootNode = null;
        }

        public void StartFunction(string functionName)
        {
            
        }

        public void StopFunction(string functionName)
        {
            
        }

        public void Update()
        {
            if (_rootNode is null)
            {
                IsRunning = false;
                return;
            }

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