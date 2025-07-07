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
        
        public void Awake()
        {
            DomainFactory.Instance.GetDomain(SaveKey.Player, out _playerModel);
            Hp = _playerModel.Attribute.Attributes["HP"].CurrentValue;
        }
    }
}