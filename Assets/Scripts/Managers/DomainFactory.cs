using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDomain<T>
{
    T Save();
    void Load(T dto);
}

public class DomainFactory : Singleton<DomainFactory>
{
    public AbilitySystem.Base.AbilitySystem PlayerASC { get; } = new();
}
