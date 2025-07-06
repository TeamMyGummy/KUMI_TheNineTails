using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using R3;
using UnityEngine;

namespace AbilitySystem.Base
{
    public enum ModOperation
    {
        Additive,
        Override,
        Multiplicative
    }

    [CreateAssetMenu(menuName = "AttributeSO")]
    public class AttributeSO : ScriptableObject
    {
        public string AttributeName;
        public float BaseValue;
        public float MaxValue;
    }

    [Serializable]
    public class Attribute
    {
        public float CurrentValue { get; private set; }
        private AttributeSO _attributeSo;
        public float BaseValue => _attributeSo.BaseValue;
        public float MaxValue => _attributeSo.MaxValue;
        
        public event Action<float>? OnValueChanged;

        public Attribute(float baseValue, float maxValue)
        {
            BaseValue = baseValue;
            CurrentValue = baseValue;
            MaxValue = maxValue;
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
                    CurrentValue += delta;
                    break;
                case ModOperation.Override:
                    CurrentValue = delta;
                    break;
                case ModOperation.Multiplicative:
                    CurrentValue *= delta;
                    break;
            }
            CurrentValue = Mathf.Clamp(CurrentValue, 0, MaxValue);
            OnValueChanged?.Invoke(CurrentValue);
        }

        /// <summary>
        /// 기본 값으로 값을 변경
        /// </summary>
        public void Reset()
        {
            CurrentValue = BaseValue;
            OnValueChanged?.Invoke(CurrentValue);
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