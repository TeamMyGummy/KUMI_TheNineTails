namespace GameAbilitySystem
{
    /// <summary>
    /// Ability의 상태 정보를 저장
    /// </summary>
    public class GameplayAbilitySpec
    {
        public AbilitySystem Asc;
        private readonly GameplayAbility _ability;
        
        public GameplayAbilitySpec(AbilitySystem asc, GameplayAbility ability)
        {
            Asc = asc;
            _ability = ability;
        }
        
        /// <summary>
        /// Ability 실행 요청
        /// </summary>
        public void TryActivate()
        {
            if(_ability.CanActivate(this)) 
                _ability.Activate(this);
        }
    }
}