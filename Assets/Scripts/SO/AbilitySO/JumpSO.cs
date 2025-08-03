using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAbilitySystem;

[CreateAssetMenu(menuName = "Ability/Jump")]
public class JumpSO : GameplayAbilitySO
{
    public int MaxJumpCount;
    public float JumpPower;
}
