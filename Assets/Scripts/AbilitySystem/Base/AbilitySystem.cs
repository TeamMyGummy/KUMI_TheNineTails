using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Data;
using UnityEngine;
using UnityEngine.Serialization;
using Util;

namespace GameAbilitySystem
{
    public class AbilitySystem : BaseDomain<ASCState>// : MonoBehaviour
    {
        private readonly Dictionary<AbilityName, GameplayAbilitySO> _abilities = new();
        private readonly HashSet<AbilityName> _grantedAbilities = new();
        public readonly TagContainer TagContainer = new();
        public readonly GameplayAttribute Attribute = new();
        private GameObject _actor;
        
        /// <summary>
        /// Ability System을 사용하는 Actor 정보를 저장 <br/>
        /// </summary>
        /// <param name="actor">ASC를 사용하는 Actor GameObject</param>
        public void SetActor(GameObject actor)
        {
            _actor = actor;
        }

        /// <summary>
        /// Ability를 등록(캐릭터가 사용할 수 있게 됨) <br/>
        /// 주의) 만약 이미 Ability가 등록되어 있을 경우 무시됨
        /// </summary>
        /// <param name="key">Ability를 호출할 Key 값</param>
        /// <param name="abilitySo">Key에 바인딩 된 Ability</param>
        public bool GrantAbility(AbilityName ability)
        {
            return _grantedAbilities.Add(ability);
        }

        /// <summary>
        /// Ability를 모두 등록(캐릭터가 사용할 수 있게 됨) <br/>
        /// 주의) 만약 이미 Ability가 등록되어 있을 경우 무시됨
        /// </summary>
        public bool GrantAllAbilities()
        {
            foreach(var ac in _abilities)
            {
                if(ac.Value != null)
                {
                    return _grantedAbilities.Add(ac.Key);
                }
            }
            return false;
        }

        /// <summary>
        /// Ability를 실행
        /// </summary>
        /// <param name="key">실행할 스킬명</param>
        /// todo: 사용할 수 없는 ability를 grant하면 문제됨(로그쓰기)
        public void TryActivateAbility(AbilityName key)
        {
            if (_grantedAbilities.TryGetValue(key, out var ability)) 
                AbilityFactory.Instance.TryActivateAbility(_abilities[ability], _actor, this);
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

        public override ASCState Save()
        {
            var asc = new ASCState();
            asc.Attributes = Attribute.GetAttributeState();
            asc.GrantedAbilities = _grantedAbilities.ToList();
            return asc;
        }

        public override void Load(ASCState dto)
        {
            var ascState = dto;
            Attribute.SetAttribute(ascState.Attributes);
            foreach (var ability in dto.GrantedAbilities)
            {
                _grantedAbilities.Add(ability);
            }
        }

        public override void Init(string assetKey)
        {
            var so = AssetLoader.Load<AbilitySystemSO>(assetKey);          
            foreach (var att in so.AddAttributeSO)
            {
                Attribute.CreateAttribute(att);
            }

            foreach (var ability in so.AbilitySO)
            {
                _abilities.TryAdd(ability.skillName, ability);
            }
        }
    }
}
