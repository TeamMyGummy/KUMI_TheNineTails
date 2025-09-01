using System;
using UnityEngine;

namespace BehaviorTree.Leaf
{
    //동일한 ActionHandler를 쓰는 노드를 병렬 실행 시 문제될 수 있음
    public abstract class MonoNode : LeafNode
    {
        public ActionHandler runtimeHandler;
        
        [Header("Action Settings")]
        public string handlerKey;


        protected abstract void OnEnter();

        protected override void OnAbort()
        {
            runtimeHandler.StopAction();
        }

        protected override NodeState Start()
        {
            OnEnter();
            return runtimeHandler.StartAction();
        }

        protected override NodeState Update()
        {
            NodeState currentState = runtimeHandler.UpdateAction();
            return currentState;
        }
    }
}