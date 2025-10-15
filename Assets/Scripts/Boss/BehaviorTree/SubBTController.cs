using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    /// <summary>
    /// SubBTGraph를 관리하는 컨트롤러
    /// </summary>
    [RequireComponent(typeof(BTContext))]
    public class SubBTController : MonoBehaviour
    {
        [SerializeField] private SubBTGraph subBTGraph;
        
        [Header("핸들러 매핑")]
        [SerializeField] protected HandlerBinding[] handlerBindings;

        private Dictionary<string, ActionHandler> handlerMap;
        private bool _isInitialized = false;

        public bool IsInitialized => _isInitialized;

        void Start()
        {
            Initialize();
        }

        void Update()
        {
            if (_isInitialized)
            {
                subBTGraph?.UpdateActiveFunctions();
            }
        }

        void OnDestroy()
        {
            Shutdown();
        }

        public void Initialize()
        {
            if (_isInitialized)
            {
                Debug.LogWarning("[SubBTController] Already initialized");
                return;
            }

            if (subBTGraph == null)
            {
                Debug.LogError("[SubBTController] SubBTGraph is not assigned");
                return;
            }

            subBTGraph.Context = GetComponent<BTContext>();
            
            MapHandler();
            InjectHandlerToNode();
            
            subBTGraph.Initialize(subBTGraph.Context);
            _isInitialized = true;
            
            Debug.Log("[SubBTController] Initialized");
        }

        public void Shutdown()
        {
            if (!_isInitialized) return;
            
            subBTGraph?.Shutdown();
            _isInitialized = false;
        }

        public bool StartFunction(string functionName)
        {
            if (!_isInitialized)
            {
                return false;
            }
            
            return subBTGraph.StartFunction(functionName);
        }

        public void StopFunction(string functionName)
        {
            if (!_isInitialized)
            {
                return;
            }
            
            subBTGraph.StopFunction(functionName);
        }

        public bool IsFunctionActive(string functionName)
        {
            if (!_isInitialized) return false;
            return subBTGraph.IsFunctionActive(functionName);
        }

        public bool HasFunction(string functionName)
        {
            if (!_isInitialized) return false;
            return subBTGraph.HasFunction(functionName);
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
            foreach (var node in subBTGraph.nodes)
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
                        Debug.LogWarning($"[SubBTController] Handler key '{monoNode.handlerKey}' not found");
                    }
                }
            }
        }
    }
}
