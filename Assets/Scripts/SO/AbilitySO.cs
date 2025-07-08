using GameAbilitySystem;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AbilitySO")]
public class AbilitySO : ScriptableObject
{
    public string Name;
    public int UnlockFloor;
    public GameplayAbility Ability;
}
