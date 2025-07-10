using UnityEngine;

namespace GameAbilitySystem
{
    /// <summary>
    /// Ability의 실질적 데이터와 순수한 로직만을 저장
    /// </summary>
    [CreateAssetMenu(menuName = "Ability/GameplayAbility")]
    public class GameplayAbilitySO : ScriptableObject
    {
        public AbilityName skillName;
    }
}