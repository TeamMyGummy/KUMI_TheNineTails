using System;
using System.Collections.Generic;
using XNode;

namespace BehaviorTree
{
    public enum NodeState
    {
        Running, //현재 실행중
        Success, //종료 상태
        Failure, //종료 상태
        Abort, //종료 상태
        Default, //한번도 실행되지 않았을 경우
    }
    
    [System.Serializable]
    public abstract class BTNode : Node
    {
        [NonSerialized] public NodeState State = NodeState.Default;
        
        public abstract NodeState Evaluate();
        
        public override object GetValue(NodePort port)
        {
            return State;
        }
    }
}