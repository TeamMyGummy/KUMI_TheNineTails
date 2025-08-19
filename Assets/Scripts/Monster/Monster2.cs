using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster2 : Monster
{
    public AbilityKey abilityKey;
    private bool isAttaking = false;
    protected override void EnterShortAttackRange()
    {
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
        asc.TryActivateAbility(abilityKey);
        yield return new WaitForSeconds(time);
        isAttaking = false;
    }
}
