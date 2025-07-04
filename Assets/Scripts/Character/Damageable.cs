using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem.Base;

public class Damageable : MonoBehaviour
{
    /// <summary>
    /// 데미지를 입었을 때 처리
    /// </summary>
    /// <param name="damage">입은 데미지 양</param>
    public void GetDamage(float damage)
    {
        // 피격 애니메이션

        // GE
        GameplayAttribute att = GetComponent<GameplayAttribute>();

        InstantGameplayEffect effect = new("HP", damage * (-1));
        effect.Apply(att);
    }
}
