using System;
using System.Collections;
using System.Collections.Generic;
using AbilitySystem;
using AbilitySystem.Base;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 스킬 사용 중일 시 다른 스킬 블락 목적으로 만들어짐 <br/>
/// 점프, 등을 제외한 모든 스킬이 해당 클래스를 상속받아야 됨 (250701 기획 상)
/// </summary>
public class BlockAbility : GameplayAbility
{
    public float BlockTimer = 0.0f;
    
    /// <summary>
    /// ⚠️반드시 부모(해당 클래스)의 Activate를 먼저 호출할 것<br/>
    /// 그래야만 다른 스킬들이 블락됨
    /// </summary>
    protected override void Activate()
    {
        ASC.TagContainer.AddWithDuration(GameplayTags.BlockRunningAbility, BlockTimer).Forget();
    }

    protected override bool CanActivate()
    {
        return !ASC.TagContainer.Has(GameplayTags.BlockRunningAbility);
    }
}
