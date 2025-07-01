using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem.Base
{
    public class AbilitySystem : MonoBehaviour
    {
        private readonly Dictionary<string, GameplayAbility> _abilities = new();
        public readonly TagContainer TagContainer = new();
        public readonly GameplayAttribute Attribute = new();
        
        /// <summary>
        /// Ability를 등록(캐릭터가 사용할 수 있게 됨) <br/>
        /// 주의) 만약 이미 Ability가 등록되어 있을 경우 무시됨
        /// </summary>
        /// <param name="key">Ability를 호출할 Key 값</param>
        /// <param name="ability">Key에 바인딩 된 Ability</param>
        public bool GrantAbility(string key, GameplayAbility ability)
        {
            ability.Init(this);
            return _abilities.TryAdd(key, ability);
        }

        /// <summary>
        /// Ability를 실행
        /// </summary>
        /// <param name="key">실행할 스킬명</param>
        public void TryActivateAbility(string key)
        {
            if (_abilities.TryGetValue(key, out var ability)) ability.TryActivate();
        }

        /// <summary>
        /// Attribute 수정 연산 적용
        /// </summary>
        /// <param name="ownerAsc">시전자의 Ability System (현재 사용X 확장 시 사용 가능성 높아서 추가해둠)</param>
        /// <param name="effect">적용할 Effect</param>
        public void ApplyGameplayEffect(AbilitySystem ownerAsc, GameplayEffect effect)
        {
            effect.Apply(Attribute);
        }
    }
}
