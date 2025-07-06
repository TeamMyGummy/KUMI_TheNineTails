using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Managers;
using UnityEngine;
using Util;

public interface IDomain
{
    /// <summary>
    /// 도메인을 저장하는 함수 <br/>
    /// 객체의 데이터를 반환하는 코드 필요
    /// </summary>
    /// <returns>json에 저장할 DTO</returns>
    object Save();
    /// <summary>
    /// 중앙 데이터에서 DTO를 로드해옴 <br/>
    /// 객체에 데이터를 반영하는 코드 필요
    /// </summary>
    /// <param name="dto">로드할 DTO</param>
    void Load(object dto);
    
    /// <summary>
    /// AssetLoader에서 데이터를 불러와 객체를 초기화(SO)하는 코드 작성
    /// </summary>
    /// <param name="assetKey">저장할 도메인이면 Domain/SaveKey(enum) 경로에서 에셋을 불러옴/몬스터 등 저장과 무관하면 알아서 불러오기</param>
    void Init(string assetKey);
    
    /// <summary>
    /// DTO 유효성 체크
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    bool CheckDto(object dto);
}

[DefaultExecutionOrder(-99)]
public class DomainFactory : Singleton<DomainFactory>
{
    private GameState _gameState;
    private readonly Dictionary<SaveKey, IDomain> _domains = new();

    new void Awake()
    {
        base.Awake();
        DataManager.Load("gamedata_0", out _gameState);
    }
    
    public void SaveGameData(string key)
    {
        foreach (var domain in _domains)
        {
            _gameState.Set(domain.Key, domain.Value.Save());
        }
        DataManager.Save(key, _gameState);
    }

    public void ClearStateAndReload(string key)
    {
        _domains.Clear();
        DataManager.Load(key, out _gameState);
    }

    public T GetDomain<T>(SaveKey key, Func<T> factory) where T : IDomain
    {
        if (_domains.TryGetValue(key, out var value))
        {
            return (T)value;
        }

        T domain = factory();
        domain.Init($"Domain/{key.ToString()}");
        var dto = _gameState.Get(key);
        if(domain.CheckDto(dto))
            domain.Load(dto);

        _domains.TryAdd(key, domain);
        
        return domain;
    }
    
    public void GetDomain<T>(SaveKey key, out T domain) where T : IDomain, new()
    {
        if (_domains.TryGetValue(key, out var value))
        {
            domain = (T)value;
        }

        domain = new();
        domain.Init($"Domain/{key.ToString()}");
        var dto = _gameState.Get(key);
        if(domain.CheckDto(dto))
            domain.Load(dto);

        _domains.TryAdd(key, domain);
    }
}
