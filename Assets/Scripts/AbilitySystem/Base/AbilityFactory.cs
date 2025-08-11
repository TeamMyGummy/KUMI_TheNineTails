using System;
using System.Collections;
using System.Collections.Generic;
using GameAbilitySystem;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using Util;

public enum AbilityName
{
    Attack = 1,
    PlayerAttack,
    Jump,
    DoubleJump,
    Dash,
    MonsterAttack,
    Parrying,
    FoxFire,
    MonsterRush,
    MonsterDoubleAttack,
    FireVariousProjectile,
    FireProjectile,
}

public enum AbilityKey
{
    None = -1,
    Attack = 1,
    PlayerAttack,
    Jump,
    DoubleJump,
    Dash,
    MonsterAttack,
    Parrying,
    FoxFire,
    MonsterRush,
    MonsterDoubleAttack,
    BossTestAbility,
    FireVariousProjectile,
    FireProjectile,
}

public interface ITickable
{
    public void Update();
    public void FixedUpdate();
}

public class AbilityFactory : SceneSingleton<AbilityFactory>
{
    //인스턴스를 돌려쓰는 스킬
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
        if(ability.IsTickable) 
            _tickables.Remove(ability as ITickable);
    }

    public void RegisterTickable(ITickable tickableAbility)
    {
        _tickables.Add(tickableAbility);
    }

    public GameplayAbility GetAbility(AbilityName abilityName)
    {
        return abilityName switch
        {
            AbilityName.Attack => new PlayerAttack(),
            AbilityName.PlayerAttack => new PlayerAttack(),
            AbilityName.Jump => new Jump(),
            AbilityName.DoubleJump => new Jump(),
            AbilityName.Dash => new Dash(),
            AbilityName.MonsterAttack => new MonsterAttack(),
            AbilityName.Parrying => new Parrying(),
            AbilityName.FoxFire => new FoxFlame(),
            AbilityName.MonsterRush => new MonsterRush(),
            AbilityName.MonsterDoubleAttack => new MonsterDoubleAttack(),
            AbilityName.FireVariousProjectile => new FireVariousProjectile(),
            AbilityName.FireProjectile => new FireProjectile(),
            _ => throw new ArgumentOutOfRangeException(nameof(abilityName), abilityName, null)
        };
    }
}
