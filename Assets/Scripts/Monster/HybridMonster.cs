using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HybridMonster : Monster
{
    public AbilityKey abilityKey = AbilityKey.FireProjectile;
    public AbilityKey abilityKey2 = AbilityKey.MonsterAttack;
    
    protected override void EnterShortAttackRange(){
        
            asc.TryActivateAbility(abilityKey2);
        
        
    }
    
    protected override void EnterLongAttackRange()
    {
        if (IsPlayerInShortRange())
        {
            asc.TryActivateAbility(abilityKey2);
        }

        else
        {
            asc.TryActivateAbility(abilityKey);
        }
    }
    
    
}