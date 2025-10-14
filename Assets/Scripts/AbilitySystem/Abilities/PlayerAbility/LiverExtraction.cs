using System.Collections;
using System.Collections.Generic;
using GameAbilitySystem;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

public class LiverExtraction : BlockAbility<BlockAbilitySO>
{
    public bool IsUsingLiverExtraction { get; private set; }
    
    private AbilitySequenceSO _sequenceSO;
    private AbilityTask _task;
    private AttackRange _attackRange;

    private float _skillTime;
    
    public override void InitAbility(GameObject actor, AbilitySystem asc, GameplayAbilitySO abilitySo)
    {
        base.InitAbility(actor, asc, abilitySo);
        
        _sequenceSO = abilitySo.skillSequence;
        //_task = new AbilityTask(actor, actor.GetComponentInChildren<Camera>(), _sequenceSO);
        _attackRange = Actor.GetComponentInChildren<AttackRange>();
    }

    protected override void Activate()
    {
        base.Activate();
        
        IsUsingLiverExtraction = true;
        //_task.Execute();
        
        // collider 설정
        if (_attackRange != null)
        {
            _attackRange.SpawnAttackRange();
            _attackRange.EnableAttackCollider(false);
            _attackRange.EnableAttackCollider(true);
        }
        
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
        
        IsUsingLiverExtraction = false;
        
        Actor.GetComponent<PlayerController>().OnDisableLiverExtraction();
        _attackRange.EnableAttackCollider(false);
        //_task.Canceled();;
    }
}
