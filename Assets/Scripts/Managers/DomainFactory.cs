using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using Util;
using Debug = UnityEngine.Debug;

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
}

public abstract class BaseDomain<TDto> : IDomain
{
    public abstract void Init(string path);

    public abstract void Load(TDto dto); 
    public abstract TDto Save();

    void IDomain.Load(object dto)
    {
        if (CheckDto(dto))
            Load((TDto)dto);
    }

    object IDomain.Save()
    {
        return Save();
    }

    public bool CheckDto(object dto)
    {
        if (dto is null)
        {
            Debug.Log("[Domain] SaveData가 존재하지 않습니다. ");
            return false;
        }
        if (dto is not TDto)
        {
            Debug.LogError($"[Domain] DTO 타입 불일치 | 예상: {typeof(TDto)}, 실제: {dto.GetType()}");
            return false;
        }

        return true;
    }

    public Type DtoType => typeof(TDto);
}


[DefaultExecutionOrder(-99)]
public class DomainFactory : Singleton<DomainFactory>
{
    private GameState _gameState;
    private readonly Dictionary<DomainKey, IDomain> _domains = new();
    public SingletonData Data;
    public const string Savekey = "gamedata_0";

    new void Awake()
    {
        base.Awake();
        DataManager.Load(Savekey, out _gameState);
        if (_gameState is null) _gameState = new();
        Data = _gameState.SingletonData;
    }
    
    public void SaveGameData()
    {
        foreach (var domain in _domains)
        {
            _gameState.Set(domain.Key, domain.Value.Save());
        }
        DataManager.Save(Savekey, _gameState);
    }

    public void ClearStateAndReload()
    {
        _domains.Clear();
        DataManager.Load(Savekey, out _gameState);
        SceneLoader.LoadScene(Data.LanternState.RecentScene);
    }

    public T GetDomain<T>(DomainKey key, Func<T> factory) where T : IDomain
    {
        if (_domains.TryGetValue(key, out var value))
        {
            return (T)value;
        }

        T domain = factory();
        domain.Init($"Domain/{key.ToString()}");
        var dto = _gameState.Get(key);
        domain.Load(dto);

        _domains.TryAdd(key, domain);
        
        return domain;
    }
    
    public void GetDomain<T>(DomainKey key, out T domain) where T : IDomain, new()
    {
        if (_domains.TryGetValue(key, out var value))
        {
            domain = (T)value;
            return;
        }

        domain = new();
        domain.Init($"Domain/{key.ToString()}");
        var dto = _gameState.Get(key);
        domain.Load(dto);

        _domains.TryAdd(key, domain);
    }
}
