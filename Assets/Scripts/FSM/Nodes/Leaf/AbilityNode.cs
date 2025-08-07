
using GameAbilitySystem;
using UnityEngine;

//BossFSM 외 FSM에서 사용할 수 없음
[CreateNodeMenu("FSM/Ability Node")]
public class AbilityNode : ActionNode
{
    [SerializeField] private AbilityKey abilityKey;
    [System.NonSerialized] private AbilitySystem _asc;
    [System.NonSerialized] private IBossAbility _ability;

    public void SetContext(AbilitySystem asc)
    {
        _asc = asc;
    }
    
    protected override void OnEnterAction()
    {
        _ability = _asc.TryActivateAbility(abilityKey) as IBossAbility;
        if (_ability is null)
        {
            isCompleted = true;
            result = false;
            Debug.Log("[BossFSM] 스킬이 존재하지 않아 노드가 비정상 종료됩니다. ");
            return;
        }

        _ability.OnNormalEnd -= NormalComplete;
        _ability.OnNormalEnd += NormalComplete;
    }
    
    public override void ForceComplete(bool success)
    {
        base.ForceComplete(success);
        _ability.CancelAbility();
    }

    protected override void OnExecuteAction()
    {
        
    }

    private void NormalComplete()
    {
        isCompleted = true;
        result = true;
    }
}
