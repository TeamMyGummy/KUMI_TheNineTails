using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem.Base;
using Unity.VisualScripting;


public class PlayerAttack : GameplayAbility
{
    /// <summary>
    /// 실제 Ability 실행부
    /// </summary>
    protected override void Activate() 
    {
        Debug.Log("어빌리티 실행");
    }
    
}
