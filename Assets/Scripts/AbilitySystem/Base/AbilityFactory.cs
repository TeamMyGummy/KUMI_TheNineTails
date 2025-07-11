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
    Jump,
    DoubleJump,
    Dash
}

public enum AbilityKey
{
    TestBlock,
    Attack,
    Jump,
    DoubleJump,
    Dash
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
    /// 모든 스킬이 종료될 때 반드시 호출해야 함(안 그러면 메모리에 쌓임!!!) <br/>
    /// 만약 바로 스킬이 끝나면 그냥 Activate에 넣으면 됨 <br/>
    /// ITickable의 경우 단순 메모리 쌓임 문제가 아니라 동작이 이상해질 수 있음
    /// </summary>
    public void EndAbility(GameplayAbility ability)
    {
        //todo: 오브젝트 풀링
        if(ability.IsTickable) 
            _tickables.Remove(ability as ITickable);
    }

    public void TryActivateAbility(GameplayAbilitySO abilitySo, GameObject actor, AbilitySystem asc)
    {
        if (!_cache.TryGetValue(abilitySo.skillName, out var ability))
        {
            ability = GetAbility(abilitySo, actor, asc);
        }

        ability.InitAbility(actor, asc, abilitySo);
        
        if(ability.TryActivate() && ability.IsTickable)
            _tickables.Add(ability as ITickable);
    }

    private GameplayAbility GetAbility(GameplayAbilitySO abilitySo, GameObject actor, AbilitySystem asc)
    {
        return abilitySo.skillName switch
        {
            AbilityName.TestBlock => new BlockAbility(),
            AbilityName.Attack => new PlayerAttack(),
            AbilityName.Jump => new Jump(),
            AbilityName.DoubleJump => new Jump(),
            AbilityName.Dash => new Dash(),
        };
    }
}
