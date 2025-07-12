using System.Collections;
using System.Collections.Generic;
using GameAbilitySystem;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    /// <summary>
    /// 데미지를 입었을 때 처리
    /// </summary>
    /// <param name="key">데미지를 입는 대상</param>
    /// <param name="damage">입은 데미지 양</param>
    public void GetDamage(DomainKey key, float damage)
    {
        // GE
        AbilitySystem asc;
        DomainFactory.Instance.GetDomain(key, out asc);
        GameplayAttribute att = asc.Attribute;

        InstantGameplayEffect effect = new("HP", damage * (-1));
        effect.Apply(att);
        
        Debug.Log($"Damage: {damage}, HP: {att.Attributes["HP"].CurrentValue}");
    }
}
