using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data;
using UnityEngine;
using Util;

namespace AbilitySystem.Base
{
    [CreateAssetMenu(menuName = "AbilitySystemSO")]
    public class AbilitySystemSO : ScriptableObject
    {
        public List<AttributeSO> AddAttributeSO;
    }
    
    [System.Serializable]
    public struct AbilityComponent
    {
        public string Name;
        public int UnlockFloor;
        public GameplayAbility Ability;
    }

    public class AbilitySystem : IDomain// : MonoBehaviour
    {
        [SerializeField]
        private List<AbilityComponent> _abilities = new();

        private readonly Dictionary<string, GameplayAbilitySpec> _grantedAbilities = new();
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
            return _grantedAbilities.TryAdd(key, new GameplayAbilitySpec(this, ability));
        }

        /// <summary>
        /// Ability를 모두 등록(캐릭터가 사용할 수 있게 됨) <br/>
        /// 주의) 만약 이미 Ability가 등록되어 있을 경우 무시됨
        /// </summary>
        public bool GrantAllAbilities()
        {
            foreach(AbilityComponent ac in _abilities)
            {
                if(ac.Ability != null)
                {
                    return _grantedAbilities.TryAdd(ac.Name, new GameplayAbilitySpec(this, ac.Ability));
                }
            }
            return false;
        }

        /// <summary>
        /// Ability를 특정 층까지 모두 등록(캐릭터가 사용할 수 있게 됨) <br/>
        /// 주의) 만약 이미 Ability가 등록되어 있을 경우 무시됨
        /// </summary>
        /// <param name="floor">Ability가 해금되는 층</param>
        public bool GrantAllAbilities(int floor)
        {
            if (_abilities.Count == 0) return false;

            foreach (AbilityComponent ac in _abilities)
            {
                if (ac.Ability != null)
                {
                    if (ac.UnlockFloor <= floor)
                    {
                        return _grantedAbilities.TryAdd(ac.Name, new GameplayAbilitySpec(this, ac.Ability));
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Ability를 실행
        /// </summary>
        /// <param name="key">실행할 스킬명</param>
        public void TryActivateAbility(string key)
        {
            if (_grantedAbilities.TryGetValue(key, out var ability)) ability.TryActivate();
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

        public object Save()
        {
            var asc = new ASCState();
            asc.Attributes = Attribute.GetAttributeState();
            return asc;
        }

        public void Load(object dto)
        {
            var ascState = (ASCState)dto;
            Attribute.SetAttribute(ascState.Attributes);
        }

        public void Init(string assetKey)
        {
            var SO = AssetLoader.Load<AbilitySystemSO>(assetKey);          
            foreach (var att in SO.AddAttributeSO)
            {
                Attribute.CreateAttribute(att);
            }
        }

        public bool CheckDto(object dto)
        {
            if (dto == null || dto is not ASCState)
            {
                Debug.LogError($"타입과 일치하지 않는 DTO를 받아오려고 시도함 | 예상한 타입: {GetType()}, 예상한 DTO: {typeof(ASCState)}");
                return false;
            }

            return true;
        }
    }
}
