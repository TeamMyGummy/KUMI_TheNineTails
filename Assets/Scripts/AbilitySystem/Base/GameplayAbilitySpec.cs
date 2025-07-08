using UnityEngine;

namespace GameAbilitySystem
{
    /// <summary>
    /// Ability의 상태 정보를 저장
    /// </summary>
    public class GameplayAbilitySpec
    {
        public AbilitySystem Asc;
        private readonly GameplayAbility _ability;
        private readonly GameObject _actor;
        
        public GameplayAbilitySpec(AbilitySystem asc, GameplayAbility ability, GameObject actor)
        {
            Asc = asc;
            _ability = ability;
            _actor = actor;
        }
        
        /// <summary>
        /// Ability 실행 요청
        /// </summary>
        public void TryActivate()
        {
            if(_ability.CanActivate(this, _actor)) 
                _ability.Activate(this, _actor);
        }
    }
}