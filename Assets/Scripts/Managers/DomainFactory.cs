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
    private Dictionary<SaveKey, IDomain> _domains = new();

    public bool HasDomain(SaveKey key)
    {
        return _domains.TryGetValue(key, out var value);
    }

    public bool TryGetDomain<T>(SaveKey key, T outDomain)
    {
        if (_domains.TryGetValue(key, out var d))
        {
            outDomain = (T)d;
            return true;
        }
        return false;
    }

    public void RegisterDomain<T>(SaveKey key, IDomain domain)
    {
        if (_domains.ContainsKey(SaveKey.Player)) return;
        domain.Load(DataManager.Load<T>(key));
        _domains.TryAdd(key, domain);
    }
        
    public T GetDomain<T>(SaveKey key)
    {
        return (T)_domains[key];
    }

    public Dictionary<SaveKey, IDomain> GetAllDomains() => _domains;
}
