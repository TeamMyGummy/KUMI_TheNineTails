using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster1_2 : Monster
{
    public AbilityKey abilityKey = AbilityKey.MonsterAttack;
    public AbilityKey abilityKey2 = AbilityKey.MonsterDoubleAttack;
    public AbilityKey abilityKey3 = AbilityKey.MonsterSwordAttack; //검기용
    private Coroutine attackCoroutine;

    protected override void EnterShortAttackRange()
    {
        // BlockTag 확인
        if (!asc.TagContainer.Has(GameAbilitySystem.GameplayTags.BlockRunningAbility) && attackCoroutine == null)
        {
            if (UnityEngine.Random.value < 0.5f)
                attackCoroutine = StartCoroutine(AttackThenSword(abilityKey));
            else
                attackCoroutine = StartCoroutine(AttackThenSword(abilityKey2));
        }
    }

    protected override void EnterLongAttackRange()
    {
        throw new System.NotImplementedException();
    }

    private IEnumerator AttackThenSword(AbilityKey firstAttack)
    {
        asc.TryActivateAbility(firstAttack);

        // 첫 공격 모션 시간만큼 대기
        if(firstAttack == AbilityKey.MonsterAttack)
            yield return new WaitForSeconds(0.2f);
        else if(firstAttack == AbilityKey.MonsterDoubleAttack)
            yield return new WaitForSeconds(0.5f);
        
        asc.TryActivateAbility(abilityKey3);

        // 코루틴 끝나면 null로
        attackCoroutine = null;
    }

    
    
}
