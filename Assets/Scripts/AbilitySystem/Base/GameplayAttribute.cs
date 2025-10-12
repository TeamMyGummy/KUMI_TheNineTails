using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using R3;
using UnityEngine;

namespace GameAbilitySystem
{
    public enum EModOperation
    {
        Additive,
        Override,
        Multiplicative
    }

    [Serializable]
    public class Attribute
    {
        private AttributeSO _attributeSo;

        public Attribute(AttributeSO so)
        {
            _attributeSo = so;
            CurrentValue = new ReactiveProperty<float>(so.BaseValue);
            MaxValueRP = new ReactiveProperty<float>(so.MaxValue);
        }

        public float BaseValue => _attributeSo.BaseValue;
        public float Value => CurrentValue.CurrentValue;
        public ReactiveProperty<float> CurrentValue { get; private set; }
        public ReactiveProperty<float> MaxValueRP { get; private set; } // 🔹 MaxValue ReactiveProperty
        public float MaxValue => MaxValueRP.Value; // 항상 MaxValueRP 값 참조

        // 다른 속성을 최대값으로 삼는 구조 (ex. HP → MaxHP)
        [CanBeNull] public Attribute _maxValue { private get; set; }

        /// <summary>
        ///     값 수정. Additive, Override, Multiplicative 중 선택
        /// </summary>
        public void Modify(float delta, EModOperation op)
        {
            switch (op)
            {
                case EModOperation.Additive:
                    CurrentValue.Value += delta;
                    break;
                case EModOperation.Override:
                    CurrentValue.Value = delta;
                    break;
                case EModOperation.Multiplicative:
                    CurrentValue.Value *= delta;
                    break;
            }

            CurrentValue.Value = Mathf.Clamp(CurrentValue.Value, 0, MaxValue);
        }

        /// <summary>
        ///     현재 값을 강제로 설정 (Clamp 포함)
        /// </summary>
        public void SetCurrentValue(float value)
        {
            CurrentValue.Value = Mathf.Clamp(value, 0, MaxValue);
        }

        /// <summary>
        ///     기본값으로 초기화
        /// </summary>
        public void Reset()
        {
            CurrentValue.Value = BaseValue;
        }

        /// <summary>
        ///     최대값 1 증가
        /// </summary>
        public void IncreaseMaxValue()
        {
            MaxValueRP.Value += 1f;
        }

        /// <summary>
        ///     최대값을 수동으로 설정
        /// </summary>
        public void SetMaxValue(float value)
        {
            MaxValueRP.Value = value;
            if (CurrentValue.Value > MaxValue)
                CurrentValue.Value = MaxValue; // Clamp
        }
    }

    public class GameplayAttribute
    {
        public Dictionary<string, Attribute> Attributes = new();

        public void CreateAttribute(AttributeSO so)
        {
            Attributes.Add(so.AttributeName, new Attribute(so));
        }

        public void SetAttribute(Dictionary<string, float> dict)
        {
            foreach (var att in dict)
            {
                Attributes[att.Key].SetCurrentValue(att.Value);
                if (Attributes.TryGetValue($"Max{att.Key}", out var maxAtt)) Attributes[att.Key]._maxValue = maxAtt;
            }
        }

        public Dictionary<string, float> GetAttributeState()
        {
            var dict = new Dictionary<string, float>();
            foreach (var att in Attributes) dict[att.Key] = att.Value.CurrentValue.Value;
            return dict;
        }
    }
}