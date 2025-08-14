using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster4_1 : Monster
{
    public AbilityKey abilityKey;
    
    protected override void EnterShortAttackRange(){
        
    }

    protected override void EnterLongAttackRange()
    {
        asc.TryActivateAbility(abilityKey);
    }
}