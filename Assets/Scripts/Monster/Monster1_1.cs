using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster1_1 : Monster
{
    public AbilityKey abilityKey = AbilityKey.MonsterAttack;
    public AbilityKey abilityKey2 = AbilityKey.MonsterDoubleAttack;

    protected override void EnterShortAttackRange(){
        if (UnityEngine.Random.value < 0.5f)
        {
            asc.TryActivateAbility(abilityKey);
            _movement._animator.SetTrigger("Attack");
        }
        else
        {
            asc.TryActivateAbility(abilityKey2);
            _movement._animator.SetTrigger("Attack2");
        }
        
    }
    
    protected override void EnterLongAttackRange()
    {
        
    }
    
    
}
