using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem.Base;
using Unity.VisualScripting;


[CreateAssetMenu(menuName = "Ability/PlayerAttack")]
public class PlayerAttack : BlockAbility
{
    /// <summary>
    /// 실제 Ability 실행부
    /// </summary>
    public override void Activate(GameplayAbilitySpec spec) 
    {
        base.Activate(spec);
        Debug.Log("어빌리티 실행");
    }
    
}
