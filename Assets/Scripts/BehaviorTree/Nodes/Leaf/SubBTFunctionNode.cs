using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree.Leaf
{
    /// <summary>
    /// SubBTGraph의 함수를 실행하는 노드
    /// </summary>
    public class SubBTFunctionNode : MonoNode
    {
        [Tooltip("실행할 함수 이름")]
        [SerializeField] private string functionName;
        
        [Tooltip("함수가 완료될 때까지 대기")]
        [SerializeField] private bool waitForCompletion = true;

        protected override void OnEnter()
        {
            if (runtimeHandler is SubBTFunctionHandler handler)
            {
                handler.SetFunctionInfo(functionName, waitForCompletion);
            }
        }
    }
}