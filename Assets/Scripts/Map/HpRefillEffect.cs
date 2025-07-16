using System.Collections;
using System.Collections.Generic;
using GameAbilitySystem;
using UnityEngine;

public class HpRefillEffect : GameplayEffect
{
    public HpRefillEffect(string attributeName)
        : base(attributeName, 0) { }

    public override void Apply(GameplayAttribute attribute)
    {
        if (!attribute.Attributes.TryGetValue(AttributeName, out var att)) return;
        
        att.Modify(att.MaxValue, ModOperation.Override);
        /*Debug.Log($"[{AttributeName}] 최대치 회복: {att.CurrentValue}");*/
    }
}
