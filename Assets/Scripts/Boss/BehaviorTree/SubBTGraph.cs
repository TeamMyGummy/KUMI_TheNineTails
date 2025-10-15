using System;
using System.Collections.Generic;
using BehaviorTree.Leaf;
using UnityEngine;
using XNode;

namespace BehaviorTree
{
    /// <summary>
    /// FunctionNode들만 모아둔 서브 BT 그래프
    /// </summary>
    [CreateAssetMenu(fileName = "SubBT Graph", menuName = "AI/SubBT Graph")]
    public class SubBTGraph : NodeGraph, IBTGraph
    {
        public BTContext Context { get; set; }
        
        [NonSerialized] private Dictionary<string, FunctionNode> _functionNodes = new Dictionary<string, FunctionNode>();
        [NonSerialized] private string _activeFunction = null;
        [NonSerialized] private LeafNode _prevNode = null;
        [NonSerialized] private bool _isAbortingFunction = false;
        [NonSerialized] private FunctionNode _abortingFunctionNode = null;
        
        public void Initialize(BTContext context)
        {
            Context = context;
            _functionNodes.Clear();
            _activeFunction = null;
            _prevNode = null;
            _isAbortingFunction = false;
            _abortingFunctionNode = null;
            
            foreach (var node in nodes)
            {
                if (node is FunctionNode funcNode)
                {
                    _functionNodes[funcNode.functionName] = funcNode;
                }

                if (node is LeafNode leaf)
                {
                    leaf.SetBTGraph(this);
                }
            }
        }

        public void Shutdown()
        {
            // 활성 함수 중단
            if (!string.IsNullOrEmpty(_activeFunction))
            {
                StopFunction(_activeFunction);
            }
            
            _activeFunction = null;
            _functionNodes.Clear();
            _prevNode = null;
            _isAbortingFunction = false;
            _abortingFunctionNode = null;
        }

        public bool StartFunction(string functionName)
        {
            // 이미 다른 함수가 실행 중이면 중단
            if (!string.IsNullOrEmpty(_activeFunction))
            {
                if (_activeFunction == functionName)
                {
                    return false;
                }
                
                StopFunction(_activeFunction);
            }
            
            _activeFunction = functionName;
            _prevNode = null;
            return true;
        }

        public void StopFunction(string functionName)
        {
            if (string.IsNullOrEmpty(functionName))
            {
                return;
            }
            
            if (_activeFunction != functionName)
            {
                return;
            }
            
            if (_functionNodes.TryGetValue(functionName, out var funcNode))
            {
                // onAbort 노드가 있으면 실행
                var abortNode = funcNode.GetAbortNode();
                if (abortNode != null)
                {
                    _isAbortingFunction = true;
                    _abortingFunctionNode = funcNode;
                    _prevNode = null;
                    // UpdateActiveFunctions에서 onAbort 노드를 실행하도록 플래그만 설정
                    return;
                }
                else
                {
                    // onAbort 노드가 없으면 경고만 출력하고 즉시 종료
                }
            }
            
            _activeFunction = null;
            _prevNode = null;
        }

        public void UpdateActiveFunctions()
        {
            // Abort 시퀀스 실행 중
            if (_isAbortingFunction && _abortingFunctionNode != null)
            {
                var abortNode = _abortingFunctionNode.GetAbortNode();
                if (abortNode != null)
                {
                    var result = abortNode.Evaluate();
                    
                    // onAbort 노드가 완료되면 함수 종료
                    if (result == NodeState.Success || result == NodeState.Failure)
                    {
                        _activeFunction = null;
                        _prevNode = null;
                        _isAbortingFunction = false;
                        _abortingFunctionNode = null;
                    }
                }
                else
                {
                    _activeFunction = null;
                    _prevNode = null;
                    _isAbortingFunction = false;
                    _abortingFunctionNode = null;
                }
                return;
            }
            
            // 일반 함수 실행
            if (string.IsNullOrEmpty(_activeFunction))
            {
                return;
            }
            
            if (_functionNodes.TryGetValue(_activeFunction, out var funcNode))
            {
                var result = funcNode.Evaluate();
                
                // 함수가 완료되거나 실패하면 자동으로 중단
                if (result == NodeState.Success || result == NodeState.Failure)
                {
                    _activeFunction = null;
                    _prevNode = null;
                }
            }
        }
        
        public bool IsFunctionActive(string functionName)
        {
            return _activeFunction == functionName;
        }

        public bool HasFunction(string functionName)
        {
            return _functionNodes.ContainsKey(functionName);
        }
        
        public void CheckPrevNode(LeafNode currNode)
        {
            if (currNode is not IStateNode) return;
            
            if (_prevNode != null && _prevNode != currNode && _prevNode.State == NodeState.Running)
            {
                _prevNode.Abort();
            }
            
            _prevNode = currNode;
        }

        public void StopGraph()
        {
            
        }
    }
}
