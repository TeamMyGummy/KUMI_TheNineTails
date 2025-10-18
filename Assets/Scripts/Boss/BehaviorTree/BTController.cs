using System;
using System.Collections.Generic;
using GameAbilitySystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace BehaviorTree
{
    [Serializable]
    public class HandlerBinding
    {
        public string key;
        public ActionHandler handler;
    }
    
    [RequireComponent(typeof(BTContext))]
    public class BTController : MonoBehaviour
    {
        [SerializeField] private BTGraph btGraph;
        
        [Header("핸들러 매핑")]
        [SerializeField] protected HandlerBinding[] handlerBindings;

        private Dictionary<string, ActionHandler> handlerMap;
    
        [Header("Events")]
        [Space(10)]
        [Tooltip("AI 종료 후 실행됩니다. ")]
        public UnityEvent onAfterEnd;

        [Header("Developer Mode")] [Space(10)] [Tooltip("체크 시 조건 없이 바로 AI가 실행됩니다.")]
        public bool executeAI;

        public bool isRunning => btGraph.IsRunning;

        void Start()
        {
            btGraph.Context = GetComponent<BTContext>();
            
            MapHandler();
            InjectHandlerToNode();
            
            if (executeAI)
            {
                StartAI();
            }
        }

        void Update()
        {
            btGraph?.Update();
        }
        
        public void StartAI()
        {
            btGraph?.StartGraph(onAfterEnd);
        }

        public void StopAI()
        {
            btGraph.StopGraph();
        }
        
        void MapHandler()
        {
            handlerMap = new Dictionary<string, ActionHandler>();
            foreach (var binding in handlerBindings)
            {
                if (!string.IsNullOrEmpty(binding.key) && binding.handler != null)
                {
                    handlerMap[binding.key] = binding.handler;
                }
            }
        }
        void InjectHandlerToNode()
        {
            foreach (var node in btGraph.nodes)
            {
                if (node is BehaviorTree.Leaf.MonoNode monoNode)
                {
                    if (!string.IsNullOrEmpty(monoNode.handlerKey) &&
                        handlerMap.TryGetValue(monoNode.handlerKey, out var handler))
                    {
                        monoNode.runtimeHandler = handler;
                    }
                    else
                    {
                        Debug.LogWarning($"[FSMController] 핸들러 키 '{monoNode.handlerKey}'를 찾을 수 없습니다.");
                    }
                }
            }
        }
    }
}