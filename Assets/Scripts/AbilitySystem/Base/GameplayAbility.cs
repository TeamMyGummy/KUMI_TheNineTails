using UnityEngine;

namespace GameAbilitySystem
{
    public abstract class GameplayAbility<TSO> : GameplayAbility where TSO : GameplayAbilitySO
    {
        public override void InitAbility(GameObject actor, AbilitySystem asc, GameplayAbilitySO abilitySo)
        {
            base.InitAbility(actor, asc, abilitySo);
            if (abilitySo is not TSO)
            {
                Debug.LogError("[BlockAbility] 타입이 일치하지 않거나 Null입니다. (AbilityFactory의 GetAbility에 정상적으로 스킬이 등록되었는지 확인 요망)");
            }
        }
    }
    /// <summary>
    /// Ability의 로직, 상태를 구현(상태 = 변수/데이터 = 상수) <br/>
    /// Ability 종료 시(상태가 더 이상 필요치 않아졌을 때)
    /// AbilityFactory.Instance.EndAbility() 호출 필수
    /// (만약 바로 상태가 없어졌을 시 Activate()에서 호출해도 됨 <br/>
    /// </summary>
    public abstract class GameplayAbility
    {
        protected GameObject Actor;
        protected AbilitySystem Asc;
        public bool IsTickable = false;

        //TryActivate 전에 반드시 선행됨<br/>
        //Awake 처럼 초기화 단을 담당함<br/>
        //만약 Tickable을 사용할 경우 반드시 override해서 IsTickable = true로 세팅해야 함
        public virtual void InitAbility(GameObject actor, AbilitySystem asc, GameplayAbilitySO abilitySo)
        {
            Actor = actor;
            Asc = asc;
            IsTickable = false;
        }

        public void UpdateActor(GameObject actor)
        {
            Actor = actor;
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