using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster6_2 : Monster
{
    public AbilityKey abilityKey = AbilityKey.MonsterAttack;
    public AbilityKey abilityKey2 = AbilityKey.MonsterAttack2;
    public AbilityKey abilityKey3 = AbilityKey.MonsterAttack;
    
    [Header("Attack Range Settings")]
    [SerializeField] private Vector2 shortAttackRange = new Vector2(4f, 2f);  // 휘두르기, 찌르기 범위
    [SerializeField] private Vector2 longAttackRange = new Vector2(10f, 2f);  // 돌진 찌르기 범위
    
    protected override void EnterShortAttackRange()
    {
        // 현재 몬스터의 공격범위 확인
        Vector2 currentRange = new Vector2(Data.DetectShortRangeX, Data.DetectShortRangeY);
        
        /*// 공격범위에 따른 스킬 선택 및 실행
        if (IsShortRange(currentRange))
        {
            ExecuteShortRangeAttack();
        }
        else if (IsLongRange(currentRange))
        {
            ExecuteLongRangeAttack();
        }*/
    }
    
    protected override void EnterLongAttackRange()
    {
        
    }
}
