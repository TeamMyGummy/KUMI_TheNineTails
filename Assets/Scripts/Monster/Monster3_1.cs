using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster3_1 : Monster
{
    public AbilityKey abilityKey;
    public AbilityKey abilityKey2;
    
    private bool isAttaking = false;
    
    protected override void EnterShortAttackRange()
    {
        asc.TryActivateAbility(abilityKey);
        _movement._animator.SetTrigger("Attack");
    }

    protected override void EnterLongAttackRange()
    {
        if (!isAttaking)
        {
            StartCoroutine(Block(2f));
        }
    }

    private IEnumerator Block(float time)
    {
        isAttaking = true;
        asc.TryActivateAbility(abilityKey2);
        _movement._animator.SetTrigger("Attack2");
        yield return new WaitForSeconds(time);
        isAttaking = false;
    }
}
