using System.Collections;
using System.Collections.Generic;
using GameAbilitySystem;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

public class LiverExtraction : BlockAbility<BlockAbilitySO>
{
    private AbilitySequenceSO _sequenceSO;
    private AbilityTask _task;

    private float _skillTime;
    
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
        Actor.GetComponent<Player>().ChangeState(PlayerStateType.LiverExtraction);
        
        SkillTimer(1.0f).Forget();
    }
    
    
    private async UniTask SkillTimer(float duration)
    {
        _skillTime = duration;
        while (_skillTime > 0.0f)
        {
            _skillTime -= Time.deltaTime;
            await UniTask.Yield();
        }
        Actor.GetComponent<Player>().ChangeState(PlayerStateType.Idle);
        _task.Canceled();;
    }
}
