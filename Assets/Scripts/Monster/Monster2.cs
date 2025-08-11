using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster2 : Monster
{
    public AbilityKey abilityKey;
    protected override void EnterAttackRange()
    {
        asc.TryActivateAbility(abilityKey);
    }
}
