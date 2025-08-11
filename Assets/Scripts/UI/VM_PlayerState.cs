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
        public ReadOnlyReactiveProperty<int> MaxFoxFireCountRP { get; private set; }
        public ReactiveProperty<int> HonbulCount { get; private set; } = new(0);

        public void Awake()
        {
            DomainFactory.Instance.GetDomain(DomainKey.Player, out _playerModel);
            if (_playerModel == null) return;

            var hpAttr = _playerModel.Attribute.Attributes["HP"];
            Hp = hpAttr.CurrentValue; // HP 변화 구독

            var ffAttr = _playerModel.Attribute.Attributes["FoxFireCount"];
            MaxFoxFireCountRP = ffAttr.MaxValueRP
                .Select(v => Mathf.FloorToInt(v))
                .ToReadOnlyReactiveProperty();

            FoxFireCount = ffAttr.CurrentValue
                .Select(v => (int)v)
                .ToReadOnlyReactiveProperty();

            FoxFireGauge = _playerModel.Attribute.Attributes["FoxFireGauge"]
                .CurrentValue.ToReadOnlyReactiveProperty();

            SkillCount = _playerModel.GrantedAbilityCount;

            // 혼불 갯수
            Observable.FromEvent<int>(
                h => Honbul.OnCollected += h,
                h => Honbul.OnCollected -= h
            )
            .Subscribe(total => HonbulCount.Value = total)
            .AddTo(this.destroyCancellationToken);
        }

        public float MaxHp
        {
            get
            {
                if (_playerModel == null) return 0f;
                return _playerModel.Attribute.Attributes["HP"].MaxValue;
            }
        }
    }
}
