using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAbilitySystem;

[CreateAssetMenu(menuName = "Ability/Jump")]
public class JumpSO : AbilitySO
{
    public int MaxJumpCount;
    public float JumpPower;
}
