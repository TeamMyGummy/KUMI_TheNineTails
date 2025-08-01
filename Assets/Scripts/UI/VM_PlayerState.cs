using Cysharp.Threading.Tasks;
using GameAbilitySystem;
using R3;
using UnityEngine;

namespace UI
{
    public class VM_PlayerState : MonoBehaviour
    {
        private AbilitySystem _playerModel;
        public ReadOnlyReactiveProperty<float> Hp { get; private set; }
        public ReadOnlyReactiveProperty<int> SkillCount { get; private set; }
        public ReadOnlyReactiveProperty<float> FoxFireGauge { get; private set; }
        public ReadOnlyReactiveProperty<int> FoxFireCount { get; private set; }

        public void Awake()
        {
            DomainFactory.Instance.GetDomain(DomainKey.Player, out _playerModel);

            if (_playerModel == null)
            {
                return;
            }

            Hp = _playerModel.Attribute.Attributes["HP"].CurrentValue;
            FoxFireGauge = _playerModel.Attribute.Attributes["FoxFireGauge"].CurrentValue.ToReadOnlyReactiveProperty();
            FoxFireCount = _playerModel.Attribute.Attributes["FoxFireCount"].CurrentValue.Select(x => (int)x).ToReadOnlyReactiveProperty();

            SkillCount = _playerModel.GrantedAbilityCount;
        }

    }
}