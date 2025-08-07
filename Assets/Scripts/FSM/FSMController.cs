using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using GameAbilitySystem;

[Serializable]
public class HandlerBinding
{
    public string key;
    public ActionHandler handler;
}

public class FSMController : MonoBehaviour
{
    [SerializeField] private FSMGraph fsmGraph;
    [SerializeField] private AbilitySystemSO so;
    private readonly AbilitySystem _asc = new();

    [Header("핸들러 매핑")]
    [SerializeField] protected HandlerBinding[] handlerBindings;

    private Dictionary<string, ActionHandler> handlerMap;

    void Start()
    {
        InitializeHandlerMap();

        // 각 노드에 핸들러 주입
        foreach (var node in fsmGraph.nodes)
        {
            if (node is MonoNode monoNode)
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

        _asc?.Init(so);
        _asc?.GrantAllAbilities();
        fsmGraph.InitGraph(_asc);
        fsmGraph?.Start();
    }

    void Update()
    {
        fsmGraph?.Update();
    }

    public ActionNode GetCurrentActionNode()
    {
        return fsmGraph?.currentNode as ActionNode;
    }

    public void CancelCurrentAction(bool success)
    {
        var currentAction = GetCurrentActionNode();
        currentAction?.ForceComplete(success);
    }

    void InitializeHandlerMap()
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
}