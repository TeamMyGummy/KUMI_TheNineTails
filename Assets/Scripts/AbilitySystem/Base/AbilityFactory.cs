using System;
using System.Collections;
using System.Collections.Generic;
using GameAbilitySystem;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using Util;

//Player용 스킬 = 1~99
//Monster용 스킬 = 100~699 - 몬스터 번호가 백의 자리
//Boss용 스킬 = 1000 - 층수가 천의 자리
public enum AbilityName
{
    //플레이어
    Attack = 1,
    PlayerAttack,
    Jump,
    DoubleJump,
    Dash,
    Parrying,
    FoxFire,
    FireProjectile,
    LiverExtraction,
    
    //몬스터
    MonsterAttack = 100,
    MonsterRush,
    MonsterDoubleAttack,
    MonsterSwordAttack,
    FireToPlayer,
    FireSonicWave,
    MonsterAttack2,
    MonsterAttack3,
    GunFire,
    
    //보스
    FireVariousProjectile = 1000,
}

public enum AbilityKey
{
    None = -1,
    //플레이어
    Attack = 1,
    PlayerAttack,
    Jump,
    DoubleJump,
    Dash,
    Parrying,
    FoxFire,
    FireProjectile,
    LiverExtraction,
    
    //몬스터
    MonsterAttack = 100,
    MonsterRush,
    MonsterDoubleAttack,
    MonsterSwordAttack,
    FireToPlayer,
    FireSonicWave,
    MonsterAttack2,
    MonsterAttack3,
    GunFire,
    
    //보스
    FireVariousProjectile = 1000,
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
            AbilityName.LiverExtraction => new LiverExtraction(),
            AbilityName.MonsterRush => new MonsterRush(),
            AbilityName.MonsterDoubleAttack => new MonsterDoubleAttack(),
            AbilityName.FireVariousProjectile => new FireVariousProjectile(),
            AbilityName.FireProjectile => new FireToPlayer(),
            AbilityName.FireToPlayer => new FireToPlayer(),
            AbilityName.MonsterSwordAttack => new MonsterSwordAttack(),
            AbilityName.FireSonicWave => new FireSonicWave(),
            AbilityName.MonsterAttack2 => new MonsterAttack2(),
            AbilityName.GunFire => new GunFire(),
            _ => throw new ArgumentOutOfRangeException(nameof(abilityName), abilityName, null)
        };
    }
}
