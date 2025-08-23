using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using GameAbilitySystem;
using R3;
using UnityEngine.Events;

[Serializable]
public class HandlerBinding
{
    public string key;
    public ActionHandler handler;
}

public class FSMController : MonoBehaviour, IAbilitySystem
{
    [SerializeField] private FSMGraph fsmGraph;
    [SerializeField] private AbilitySystemSO so;
    private readonly AbilitySystem _asc = new();

    public AbilitySystem asc => _asc;

    [Header("핸들러 매핑")]
    [SerializeField] protected HandlerBinding[] handlerBindings;

    private Dictionary<string, ActionHandler> handlerMap;
    
    [Header("Events")]
    [Space(10)]
    [Tooltip("AI 종료 후 실행됩니다. ")]
    public UnityEvent onAfterEnd;
    
    private bool _isRunning = false;

    public void StartAI()
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
        _asc?.SetSceneState(gameObject);
        asc.Attribute.Attributes["HP"].CurrentValue.Subscribe(AutoStopAI);
        
        fsmGraph.InitGraph(_asc);
        fsmGraph?.StartGraph();
        _isRunning = true;
    }

    private void AutoStopAI(float value)
    {
        if (value <= 0)
        {
            StopAI();
        }
    }

    public void StopAI()
    {
        if (!_isRunning) return;
        
        _isRunning = false;
        fsmGraph.StopGraph();
        onAfterEnd?.Invoke();
    }

    void Update()
    {
        fsmGraph?.Update();
    }

    public ActionNode GetCurrentActionNode()
    {
        return fsmGraph?.currentNode as ActionNode;
    }

    public IForceCancel GetCurrentCancelableNode()
    {
        return fsmGraph?.currentNode as IForceCancel;
    }

    public void CancelCurrentNode(bool success)
    {
        var currentAction = GetCurrentCancelableNode();
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