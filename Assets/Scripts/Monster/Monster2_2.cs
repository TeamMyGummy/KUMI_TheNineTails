using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster2_2 : Monster
{
    public AbilityKey abilityKey;
	private bool _isAttacking = false;
    private int _cnt = 0;
    private float _attackDelay = 0.3f;
    private float _secondDelay = 0.6f;
    protected override void EnterShortAttackRange()
    {
        Debug.Log("Entering short");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            CharacterMovement playerMovement = player.GetComponent<CharacterMovement>();
            if (playerMovement != null)
            {
                // 몬스터에서 플레이어로의 방향 계산
                Vector2 monsterPosition = transform.position;
                Vector2 playerPosition = (Vector2)player.transform.position + Vector2.up;
                Vector2 knockbackDirection = playerPosition.x > monsterPosition.x ? Vector2.right : Vector2.left;
                
                // 넉백 적용 (6칸, 0.3초)
                playerMovement.ApplyKnockback(knockbackDirection, 6f, 0.3f);
            }
        }
    }

    protected override void EnterLongAttackRange()
    {
        if (!_isAttacking)
        {
            int randomAttack = Random.Range(0, 2);
            
            if (randomAttack == 0)
            {
                Debug.Log("Entering triple");
                StartCoroutine(TripleAttack(2f));
            }
            else
            {
                Debug.Log("Entering changed");
                StartCoroutine(ChangedAttack(2f));
            }
        }
    }

    private IEnumerator TripleAttack(float time)
    {
        _isAttacking = true;
        for (int i = 0; i < 3; i++)
        {
            asc.TryActivateAbility(abilityKey);

            if (i < 2)
            {
                yield return new WaitForSeconds(_attackDelay);
            }
        }
        yield return new WaitForSeconds(time);
        _isAttacking = false;
    }

    private IEnumerator ChangedAttack(float time)
    {
        _isAttacking = true;
        for (int i = 0; i < 3; i++)
        {
            asc.TryActivateAbility(abilityKey);
            if (i < 1)
            {
                yield return new WaitForSeconds(_attackDelay);
            }
            else if (i == 1)
            {
                yield return new WaitForSeconds(_secondDelay);
            }
        }
        yield return new WaitForSeconds(time);
        _isAttacking = false;
    }
}
