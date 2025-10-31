using System;
using System.Collections.Generic;
using GameAbilitySystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using Util;

public class Lantern : SceneSingleton<Lantern>
{
    //public Action<int> Interacted;
    private readonly Dictionary<int, LanternObject> _lanternsObjects = new();
    public LanternState _lanternState;
    private AbilitySystem _player;

    public void Awake()
    {
        _lanternState = DomainFactory.Instance.Data.LanternState;
        /*Interacted -= Interact;
        Interacted += Interact;*/
    }

    public void Start()
    {
        _player = FindObjectOfType<Player>().ASC;
    }

    public Vector3 GetLanternPos(int key)
    {
        if (!_lanternsObjects.TryGetValue(key, out var lantern))
        {
            Debug.Log("현재 씬에서 호롱불을 찾을 수 없습니다. ");
            return FindObjectOfType<Player>().transform.position;
        }
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
        
        Debug.Log("이벤트 함수 연결");
        lantern.Interacted -= Interact;
        lantern.Interacted += Interact;
        //lantern.Bind(Interacted);
        _lanternsObjects[lantern.LanternKey] = lantern;
    }

    public void Interact(int interactLantern)
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
        Debug.Log(SceneLoader.GetCurrentSceneName() + "씬을 다음에 로드할 것입니다.");
        
        if (_lanternsObjects.TryGetValue(interactLantern, out var currentLantern))
        {
            _lanternState.RecentFloor = currentLantern.NumberOfFloor; 
            _lanternState.RecentSection = currentLantern.SectionName;
        }
        
        Debug.Log("5");
        _player.Attributes["HP"].SetCurrentValue(100);
        
        DomainFactory.Instance.SaveGameData();
    }
    
    public bool TryGetLanternObject(int key, out LanternObject obj)
    {
        return _lanternsObjects.TryGetValue(key, out obj);
    }
}
