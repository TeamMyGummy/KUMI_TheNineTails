using GameAbilitySystem;
using System.Collections.Generic;
using GameAbilitySystem;
using UnityEngine;

[CreateAssetMenu(menuName = "AbilitySystemSO")]
public class AbilitySystemSO : ScriptableObject
{
    public List<AttributeSO> AddAttributeSO;
    public List<GameplayAbilitySO> AbilitySO;
}