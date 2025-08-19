using System.Collections;
using System.Collections.Generic;
using GameAbilitySystem;
using UnityEngine;

public class LiverExtraction : BlockAbility<BlockAbilitySO>
{
    private AbilitySequenceSO _sequenceSO;
    private AbilityTask _task;
    
    public override void InitAbility(GameObject actor, AbilitySystem asc, GameplayAbilitySO abilitySo)
    {
        base.InitAbility(actor, asc, abilitySo);
        
        _sequenceSO = abilitySo.skillSequence;
        _task = new AbilityTask(actor, actor.GetComponentInChildren<Camera>(), _sequenceSO);
    }

    protected override void Activate()
    {
        base.Activate();
        
        _task.Execute();
    }
}
