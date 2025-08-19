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
        
        if (key == DomainKey.Player)
        {
            CharacterMovement cm = GetComponent<CharacterMovement>();
            Vector2 knockbackDirection = cm.GetCharacterSpriteDirection() * (-1);
            cm.ApplyKnockback(knockbackDirection, 6f, 0.3f);
            GetComponent<Player>().Hurt();
        }
    }
    
    /// <summary>
    /// 데미지를 입었을 때 처리 (데미지를 입힌 방향이 필요할 때)
    /// </summary>
    /// <param name="key">데미지를 입는 대상</param>
    /// <param name="damage">입은 데미지 양</param>
    /// <param name="direction">데미지를 입힌 방향</param>
    public void GetDamage(DomainKey key, float damage, Vector2 direction)
    {
        // GE
        AbilitySystem asc;
        DomainFactory.Instance.GetDomain(key, out asc);
        GameplayAttribute att = asc.Attribute;

        InstantGameplayEffect effect = new("HP", damage * (-1));
        effect.Apply(att);
        
        Debug.Log($"Damage: {damage}, HP: {att.Attributes["HP"].CurrentValue}");

        if (key == DomainKey.Player)
        {
            CharacterMovement cm = GetComponent<CharacterMovement>();
            cm.ApplyKnockback(direction, 6f, 0.3f);
            GetComponent<Player>().Hurt();
        }
    }
    
    /// <summary>
    /// 데미지를 입었을 때 처리 (플레이어 제외)
    /// </summary>
    /// <param name="asc">데미지를 입는 대상이 가지고 있는 Ability System</param>
    /// <param name="damage">입은 데미지 양</param>
    public void GetDamage(AbilitySystem asc, float damage)
    {
        // GE
        GameplayAttribute att = asc.Attribute;

        InstantGameplayEffect effect = new("HP", damage * (-1));
        effect.Apply(att);
        
        Debug.Log($"Damage: {damage}, HP: {att.Attributes["HP"].CurrentValue}");
    }
}
