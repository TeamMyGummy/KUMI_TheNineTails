using UnityEngine;

namespace GameAbilitySystem
{
    /// <summary>
    /// Ability의 상태 정보를 저장
    /// </summary>
    public abstract class GameplayAbility
    {
        protected GameObject Actor;
        protected AbilitySystem Asc;

        //TryActivate 전에 반드시 선행되어야 함
        public virtual void SetGameplayAbility(GameObject actor, AbilitySystem asc, GameplayAbilitySO abilitySo)
        {
            Actor = actor;
            Asc = asc;
        }
        
        /// <summary>
        /// 실제 Ability 실행부
        /// </summary>
        protected abstract void Activate();

        /// <summary>
        /// Ability를 실행할 수 있는지 여부 판단. <br/>
        /// 해당 함수를 override 해서 Ability 실행 여부를 결정할 수 있음.
        /// </summary>
        /// <returns>실행 가능 여부</returns>
        protected virtual bool CanActivate() => true;
        
        /// <summary>
        /// Ability 실행 요청
        /// </summary>
        public bool TryActivate()
        {
            if (CanActivate())
            {
                Activate();
                return true;
            }

            return false;
        }
    }
}