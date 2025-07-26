using UnityEngine;

namespace GameAbilitySystem
{
    /// <summary>
    /// Ability의 실질적 데이터와 순수한 로직만을 저장
    /// </summary>
    [CreateAssetMenu(menuName = "Ability/GameplayAbility")]
    public class GameplayAbilitySO : ScriptableObject
    {
        //Key로 호출하고 Name으로 선택(Ex. 같은 <패링> 키여도 다른 스킬 실행 가능하도록)
        public AbilityKey skillKey;
        public AbilityName skillName;
        public Sprite skillIcon;
        public string description;
    }
}