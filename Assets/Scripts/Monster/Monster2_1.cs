using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster2_1 : Monster
{
    public AbilityKey abilityKey;
	private bool isAttacking = false;
    private int cnt = 0;
    private float attackDelay = 0.5f;
    protected override void EnterShortAttackRange()
    {
        if (!isAttacking)
        {
            StartCoroutine(Block(2f));
        }
    }

    protected override void EnterLongAttackRange()
    {
        
    }

    private IEnumerator Block(float time)
    {
        isAttacking = true;
        for (int i = 0; i < 3; i++)
        {
            asc.TryActivateAbility(abilityKey);

            if (i < 2)
            {
                yield return new WaitForSeconds(attackDelay);
            }
        }
        yield return new WaitForSeconds(time);
        isAttacking = false;
    }
}
