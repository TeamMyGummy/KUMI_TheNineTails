using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster1 : Monster
{
    public AbilityKey abilityKey;
    
    protected override void EnterAttackRange(){
        asc.TryActivateAbility(abilityKey);
    }
}
