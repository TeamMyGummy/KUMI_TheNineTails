using UnityEngine;

namespace AbilitySystem.Base
{
    public abstract class GameplayAbility
    {
        protected AbilitySystem ASC;
        public void Init(AbilitySystem asc)
        {
            ASC = asc;
        }
        
        /// <summary>
        /// Ability 실행 요청
        /// </summary>
        public void TryActivate()
        {
            if(CanActivate()) Activate();
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
        protected virtual bool CanActivate()
        {
            return true;
        }
    }
}