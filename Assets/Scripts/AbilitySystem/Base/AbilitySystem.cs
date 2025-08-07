using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Data;
using R3;
using UnityEngine;
using UnityEngine.Serialization;
using Util;

namespace GameAbilitySystem
{
    public class AbilitySystem : BaseDomain<ASCState>// : MonoBehaviour
    {
        private readonly Dictionary<AbilityName, GameplayAbility> _abilityCache = new();
        private readonly Dictionary<AbilityName, GameplayAbilitySO> _abilities = new();
        private readonly Dictionary<AbilityKey, AbilityName> _grantedAbilities = new();
        public readonly TagContainer TagContainer = new();
        public readonly GameplayAttribute Attribute = new();
        private GameObject _actor;

        private readonly ReactiveProperty<int> _grantedAbilityCount = new(0);
        public ReadOnlyReactiveProperty<int> GrantedAbilityCount { get; private set; }

        /// <summary>
        /// Ability System에서 현재 씬 상태에 맞게 내부 상태를 변경
        /// </summary>
        /// <param name="actor">ASC를 사용하는 Actor GameObject</param>
        public void SetSceneState(GameObject actor)
        {
            _actor = actor;
            _abilityCache.Clear();
        }

        /// <summary>
        /// Ability를 등록(캐릭터가 사용할 수 있게 됨) <br/>
        /// 주의1) 만약 이미 Ability가 등록되어 있을 경우 덮어씀
        /// 주의2) 만약 해금할 수 없는 스킬이라면 false 반환 및 로그 발생
        /// </summary>
        public bool GrantAbility(AbilityKey key, AbilityName name)
        {
            if (!_abilities.TryGetValue(name, out var abilitySo)) return false;
            _grantedAbilities[key] = name;
            _grantedAbilityCount.Value = _grantedAbilities.Count;
            return true;
        }

        /// <summary>
        /// Ability를 모두 등록(캐릭터가 사용할 수 있게 됨) <br/>
        /// 주의) 만약 이미 Ability가 등록되어 있을 경우 무시됨
        /// </summary>
        public bool GrantAllAbilities()
        {
            foreach (var ac in _abilities)
            {
                if (ac.Value != null)
                {
                    GrantAbility(ac.Value.skillKey, ac.Key);
                }
            }
            return false;
        }

        /// <summary>
        /// Ability를 실행
        /// </summary>
        /// <param name="key">실행할 스킬명</param>
        public GameplayAbility TryActivateAbility(AbilityKey key)
        {
            if (!_grantedAbilities.TryGetValue(key, out var abilityName)) return null;
            if (!_abilities.TryGetValue(abilityName, out var abilitySo)) return null;

            if (!_abilityCache.TryGetValue(abilitySo.skillName, out var ability))
            {
                ability = AbilityFactory.Instance.GetAbility(abilitySo.skillName);
                ability.InitAbility(_actor, this, abilitySo);
                if (ability.CanReuse) _abilityCache.Add(abilitySo.skillName, ability);
            }

            if (ability.TryActivate() && ability.IsTickable)
                AbilityFactory.Instance.RegisterTickable(ability as ITickable);

            return ability;
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
            asc.GrantedAbilities = _grantedAbilities;
            return asc;
        }

        public override void Load(ASCState dto)
        {
            var ascState = dto;
            Attribute.SetAttribute(ascState.Attributes);
            foreach (var ability in dto.GrantedAbilities)
            {
                if (!_grantedAbilities.TryAdd(ability.Key, ability.Value))
                {
                    Debug.LogWarning("[AbilitySystem] 스킬 데이터에 키가 중복되는 스킬이 Grant 되었습니다.");
                }
            }
            _grantedAbilityCount.Value = _grantedAbilities.Count;
            GrantedAbilityCount = _grantedAbilityCount.ToReadOnlyReactiveProperty();
        }

        public override void Init(string assetKey)
        {
            var so = ResourcesManager.Instance.Load<AbilitySystemSO>(assetKey);
            Init(so);
        }
        
        public void Init(AbilitySystemSO so)
        {
            foreach (var att in so.AddAttributeSO)
            {
                Attribute.CreateAttribute(att);
            }

            foreach (var ability in so.AbilitySO)
            {
                _abilities.TryAdd(ability.skillName, ability);
            }

            GrantedAbilityCount = _grantedAbilityCount.ToReadOnlyReactiveProperty();
        }

        public bool IsGranted(AbilityKey key)
        {
            return _grantedAbilities.ContainsKey(key);
        }
        
        public GameplayAbilitySO GetSkillSO(AbilityKey key)
        {
            if (_grantedAbilities.TryGetValue(key, out var name))
            {
                _abilities.TryGetValue(name, out var so);
                return so;
            }
            return null;
        }
    }
}
