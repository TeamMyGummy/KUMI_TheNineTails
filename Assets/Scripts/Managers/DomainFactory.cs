using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public interface IDomain
{
    object Save();
    void Load(object dto);
}

public enum SaveKey{
    Player,
}

public class DomainFactory : Singleton<DomainFactory>
{
    private readonly Dictionary<SaveKey, IDomain> _domains = new();

    public bool HasDomain(SaveKey key)
    {
        return _domains.TryGetValue(key, out var value);
    }

    public bool TryGetDomain<TDomain>(SaveKey key, out TDomain outDomain)
    {
        if (_domains.TryGetValue(key, out var d) && d is TDomain)
        {
            outDomain = (TDomain)d;
            return true;
        }
        outDomain = default;
        return false;
    }

    public void RegisterDomain<TDto>(SaveKey key, IDomain domain)
    {
        if (_domains.ContainsKey(key)) return;
        domain.Load(DataManager.Load<TDto>(key));
        _domains.TryAdd(key, domain);
    }
        
    public TDomain GetDomain<TDomain>(SaveKey key)
    {
        return (TDomain)_domains[key];
    }

    public Dictionary<SaveKey, IDomain> GetAllDomains() => _domains;
}
