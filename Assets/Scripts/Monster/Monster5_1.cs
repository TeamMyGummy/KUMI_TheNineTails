using System.Collections;
using UnityEngine;

public class Monster5_1 : Monster
{
    public AbilityKey abilityKey;
    private Coroutine explosionCoroutine;

    protected override void EnterShortAttackRange()
    {
        if (explosionCoroutine == null && !isDead)
            explosionCoroutine = StartCoroutine(ExplosionCountdown());
    }

    protected override void EnterLongAttackRange() { }

    private IEnumerator ExplosionCountdown()
    {
        float delay = asc.Attribute.Attributes["ExplosionDelay"].CurrentValue.Value;
        float t = 0f;

        while (t < delay)
        {
            //어그로 풀리거나, 죽으면 break
            if (!isAggro || isDead)
            {
                explosionCoroutine = null;
                yield break;
            }
            t += Time.deltaTime;
            yield return null;
        }

        if (isAggro && !isDead)
        {
            asc.TryActivateAbility(abilityKey);
            base.Die();
        }

        explosionCoroutine = null;
    }

    protected override void Die()
    {
        if (!isDead)
        {
            asc.TryActivateAbility(abilityKey);
            base.Die();
        }

        if (explosionCoroutine != null)
        {
            StopCoroutine(explosionCoroutine);
            explosionCoroutine = null;
        }
    }
}