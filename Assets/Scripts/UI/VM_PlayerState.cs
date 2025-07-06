using R3;
using UnityEngine;

namespace UI
{
    public class VM_PlayerState : MonoBehaviour
    {
        private AbilitySystem.Base.AbilitySystem _playerModel;
        public ReadOnlyReactiveProperty<float> Hp { get; private set; }

        void Awake()
        {
            _playerModel = DomainFactory.Instance.GetDomain<AbilitySystem.Base.AbilitySystem>(SaveKey.Player);
            Hp = _playerModel.Attribute.Attributes["HP"].CurrentValue;
        }
    }
}