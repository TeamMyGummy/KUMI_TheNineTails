using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster1_1 : Monster
{
    public AbilityKey abilityKey = AbilityKey.MonsterAttack;
    public AbilityKey abilityKey2 = AbilityKey.MonsterDoubleAttack;
    
    protected override void EnterAttackRange(){
        if (UnityEngine.Random.value < 0.5f)
        {
            asc.TryActivateAbility(abilityKey);
        }
        else
        {
            asc.TryActivateAbility(abilityKey2);
        }
        
    }
    
    
}
