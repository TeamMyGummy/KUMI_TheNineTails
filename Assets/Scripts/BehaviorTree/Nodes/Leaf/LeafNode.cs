using System;
using UnityEngine;

namespace BehaviorTree.Leaf
{
    public abstract class LeafNode : BTNode
    {
        [Input] public BTNode input;
        protected BTGraph btGraph;
        protected BTContext Context => btGraph.Context;
        [NonSerialized] protected bool isRunning = false;

        protected virtual NodeState Start() { return State = NodeState.Running; }
        protected abstract NodeState Update();
        protected virtual void OnAbort(){}

        public void SetBTGraph(BTGraph graph)
        {
            btGraph = graph;
        }

        public void Abort()
        {
            State = NodeState.Abort;
            OnAbort();
        }
        
        public override NodeState Evaluate()
        {
            btGraph.CheckPrevNode(this);
            if (!isRunning)
            {
                Debug.Log("현재 시작된 노드는 : " + name);
                isRunning = true;
                State = Start();
                if (State != NodeState.Running)
                {
                    isRunning = false;
                    return State;
                }

                return NodeState.Running;
            }

            State = Update();
            
            if (State != NodeState.Running)
            {
                isRunning = false;
                return State;
            }
            return NodeState.Running;
        }

        public NodeState ParallelEvaluate()
        {
            if (State != NodeState.Running)
            {
                Debug.Log("현재 시작된 노드는 : " + name);
                return State = Start();
            }
            return State = Update();
        }

    }
}