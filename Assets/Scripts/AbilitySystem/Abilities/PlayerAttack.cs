using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameAbilitySystem;
using UnityEngine;
using Unity.VisualScripting;


[CreateAssetMenu(menuName = "Ability/PlayerAttack")]
public class PlayerAttack : BlockAbility, ITickable
{
    /// <summary>
    /// 실제 Ability 실행부
    /// </summary>
    protected override void Activate() 
    {
        base.Activate();
        Debug.Log("어빌리티 실행");
        EndSkill().Forget();
    }

    public void Update()
    {
        Debug.Log("~~어빌리티 실행중~~");
    }

    public void FixedUpdate()
    {
        
    }

    public async UniTask EndSkill()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_so.BlockTimer), DelayType.DeltaTime);
        AbilityFactory.Instance.RemoveTickable(this);
    }
}
