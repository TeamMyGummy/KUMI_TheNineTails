using GameAbilitySystem;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AbilitySystemSO")]
public class AbilitySystemSO : ScriptableObject
{
    public List<AttributeSO> AddAttributeSO;
    public List<AbilitySO> AddAbilitySO;

}