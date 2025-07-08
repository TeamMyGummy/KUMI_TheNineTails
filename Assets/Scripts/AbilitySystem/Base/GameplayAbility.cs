using UnityEngine;

namespace GameAbilitySystem
{
    /// <summary>
    /// Ability의 실질적 데이터와 순수한 로직만을 저장
    /// </summary>
    public abstract class GameplayAbility : ScriptableObject
    {
        /// <summary>
        /// 실제 Ability 실행부
        /// </summary>
        public abstract void Activate(GameplayAbilitySpec spec, GameObject actor);

        /// <summary>
        /// Ability를 실행할 수 있는지 여부 판단. <br/>
        /// 해당 함수를 override 해서 Ability 실행 여부를 결정할 수 있음.
        /// </summary>
        /// <returns>실행 가능 여부</returns>
        public virtual bool CanActivate(GameplayAbilitySpec spec, GameObject actor) => true;
    }
}