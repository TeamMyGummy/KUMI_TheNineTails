using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Util;

//⚠️LanternObject가 존재하는 씬에 반드시 붙어있어야 함
public class Lantern : SceneSingleton<Lantern>
{
    private Action<int> Interacted;
    private readonly Dictionary<int, LanternObject> _lanternsObjects = new();
    private LanternState _lanternState;

    public void Awake()
    {
        DomainFactory.Instance.GetState(StateKey.Lantern, out _lanternState);
        Interacted -= Interact;
        Interacted += Interact;
    }

    public void Register(LanternObject lantern)
    {
        if (_lanternState.RecentCheckPoint == lantern.LanternKey)
        { 
            lantern.ChangeLanternState(LanternAppearance.Big);
        }
        else if (_lanternState.PassedCheckPoint.Contains(lantern.LanternKey))
        { 
            lantern.ChangeLanternState(LanternAppearance.Small);
        }
            
        lantern.Bind(Interacted);
        _lanternsObjects[lantern.LanternKey] = lantern;
    }

    private void Interact(int interactLantern)
    {
        //기존 체크포인트가 해당 씬 안에 있다면 그 체크포인트의 상태 변경
        if (_lanternsObjects.TryGetValue(_lanternState.RecentCheckPoint, out var lanternObject))
        {
            lanternObject.ChangeLanternState(LanternAppearance.Small);
        }
        
        //상태 업데이트
        _lanternState.PassedCheckPoint.Add(interactLantern);
        _lanternState.RecentCheckPoint = interactLantern;
        _lanternState.RecentScene = SceneLoader.GetCurrentSceneName();
    }
}
