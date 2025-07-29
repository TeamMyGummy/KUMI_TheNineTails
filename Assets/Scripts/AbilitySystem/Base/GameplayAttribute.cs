using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using R3;
using UnityEngine;

namespace GameAbilitySystem
{
    public enum ModOperation
    {
        Additive,
        Override,
        Multiplicative
    }

    [Serializable]
    public class Attribute
    {
        private AttributeSO _attributeSo;
        public float BaseValue => _attributeSo.BaseValue;
        public ReactiveProperty<float> CurrentValue { get; private set; }
        public float MaxValue => _maxValue is null ? _attributeSo.MaxValue : _maxValue.CurrentValue.Value;
        [CanBeNull] public Attribute _maxValue { private get; set; }
        
        //public event Action<float>? OnValueChanged;

        public Attribute(AttributeSO so)
        {
            _attributeSo = so;
            CurrentValue = new ReactiveProperty<float>(so.BaseValue);
        }
        
        /// <summary>
        /// 값을 수정 <br/>
        /// 만약 0보다 작거나 MaxValue보다 클 시 0, MaxValue로 Clamp
        /// </summary>
        /// <param name="delta">변경할 양</param>
        public void Modify(float delta, ModOperation op)
        {
            //만약 확장성이 필요해지면 인터페이스로 바꾸겠음
            switch (op)
            {
                case ModOperation.Additive:
                    CurrentValue.Value += delta;
                    break;
                case ModOperation.Override:
                    CurrentValue.Value = delta;
                    break;
                case ModOperation.Multiplicative:
                    CurrentValue.Value *= delta;
                    break;
            }
            CurrentValue.Value = Mathf.Clamp(CurrentValue.Value, 0, MaxValue);
            //OnValueChanged?.Invoke(CurrentValue);
        }

        public void SetCurrentValue(float value)
        {
            CurrentValue.Value = value;
            //OnValueChanged?.Invoke(CurrentValue);
        }

        /// <summary>
        /// 기본 값으로 값을 변경
        /// </summary>
        public void Reset()
        {
            CurrentValue.Value = BaseValue;
            //OnValueChanged?.Invoke(CurrentValue);
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
                if(Attributes.TryGetValue($"Max{att.Key}", out var maxAtt))
                {
                    Attributes[att.Key]._maxValue = maxAtt;
                }
            }
        }

        public Dictionary<string, float> GetAttributeState()
        {
            var dict = new Dictionary<string, float>();
            foreach (var att in Attributes)
            {
                dict[att.Key] = att.Value.CurrentValue.Value;
            }

            return dict;
        }
    }
}