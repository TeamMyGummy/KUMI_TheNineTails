using System;
using System.Collections;
using System.Collections.Generic;
using GameAbilitySystem;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using Util;

public enum AbilityName
{
    TestBlock,
    Attack,
}

public interface ITickable
{
    public void Update();
    public void FixedUpdate();
}

public class AbilityFactory : SceneSingleton<AbilityFactory>
{
    //캐싱 해야 함
    private Dictionary<AbilityName, GameplayAbility> _cache = new();
    private HashSet<ITickable> _tickables = new();

    public void Update()
    {
        foreach(var tickable in _tickables)
            tickable.Update();
    }

    public void FixedUpdate()
    {
        foreach(var tickable in _tickables)
            tickable.FixedUpdate();
    }
    
    /// <summary>
    /// Tickable 스킬이 종료될 시 반드시 RemoveTickable을 호출해야 함(안 그러면 메모리에 쌓임)
    /// </summary>
    /// <param name="tickable"></param>
    public void RemoveTickable(ITickable tickable)
    {
        _tickables.Remove(tickable);
    }

    public void TryActivateAbility(GameplayAbilitySO abilitySo, GameObject actor, AbilitySystem asc)
    {
        if (!_cache.TryGetValue(abilitySo.skillName, out var ability))
        {
            ability = GetAbility(abilitySo, actor, asc);
        }

        ability.SetGameplayAbility(actor, asc, abilitySo);
        
        if(ability.TryActivate() && ability is ITickable)
            _tickables.Add((ITickable)ability);
    }

    private GameplayAbility GetAbility(GameplayAbilitySO abilitySo, GameObject actor, AbilitySystem asc)
    {
        return abilitySo.skillName switch
        {
            AbilityName.TestBlock => new BlockAbility(),
            AbilityName.Attack => new PlayerAttack()
        };
    }
}
