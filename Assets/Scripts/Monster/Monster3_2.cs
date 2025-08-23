using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster3_2 : Monster
{
    public AbilityKey abilityKey;
    public AbilityKey abilityKey2;
    public AbilityKey abilityKey3;
    
    private bool isAttaking = false;
    
    protected override void EnterShortAttackRange()
    {
        asc.TryActivateAbility(abilityKey);
    }

    protected override void EnterLongAttackRange()
    {
        if (!isAttaking)
        {
            int randomAttack = Random.Range(0, 2);
            
            if (randomAttack == 0)
            {
                StartCoroutine(Block(2f));
            }
            else
            {
                StartCoroutine(SonicWave(2f));
            }
        }
    }

    private IEnumerator Block(float time)
    {
        isAttaking = true;
        asc.TryActivateAbility(abilityKey2);
        yield return new WaitForSeconds(time);
        isAttaking = false;
    }
    
    private IEnumerator SonicWave(float time)
    {
        isAttaking = true;
        asc.TryActivateAbility(abilityKey3);
        yield return new WaitForSeconds(time);
        isAttaking = false;
    }
}
