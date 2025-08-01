using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster5 : Monster
{
    public AbilityKey abilityKey;
    
    protected override void EnterAttackRange()
    {
        StartCoroutine(WaitingExplosionDelay());
        
    }

    private IEnumerator WaitingExplosionDelay()
    {
        float explosionDelay = asc.Attribute.Attributes["ExplosionDelay"].CurrentValue.Value;
        yield return new WaitForSeconds(explosionDelay);
        asc.TryActivateAbility(abilityKey);
        Die();
    }
    
}
