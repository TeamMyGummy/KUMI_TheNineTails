﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Util;

public class Lantern : SceneSingleton<Lantern>
{
    private Action<int> Interacted;
    private readonly Dictionary<int, LanternObject> _lanternsObjects = new();
    private LanternState _lanternState;

    public void Awake()
    {
        _lanternState = DomainFactory.Instance.Data.LanternState;
        Interacted -= Interact;
        Interacted += Interact;
    }

    public Vector3 GetLanternPos(int key)
    {
        if(!_lanternsObjects.TryGetValue(key, out var lantern)) Debug.Log("현재 씬에서 호롱불을 찾을 수 없습니다. ");
        return lantern.transform.position;
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
        
        DomainFactory.Instance.SaveGameData();
    }
}
