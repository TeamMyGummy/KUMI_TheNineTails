
using System;
using GameAbilitySystem;
using UnityEngine;

namespace BehaviorTree.Leaf
{
    public class AbilityNode : LeafNode
    {
        [SerializeField] private AbilityKey abilityKey;
        [NonSerialized] private IBossAbility _bossAbility;

        protected override NodeState Start()
        {
            _bossAbility = btGraph.Context.ASC.TryActivateAbility(abilityKey) as IBossAbility;
            Debug.Assert(_bossAbility is not null);
            _bossAbility.OnNormalEnd -= EndAbility;
            _bossAbility.OnNormalEnd += EndAbility;
            
            return State = NodeState.Running;
        }
        
        protected override NodeState Update()
        {
            return State;
        }

        protected override void OnAbort()
        {
            _bossAbility.CancelAbility();
        }

        private void EndAbility()
        {
            State = NodeState.Success;
        }
    }
}
