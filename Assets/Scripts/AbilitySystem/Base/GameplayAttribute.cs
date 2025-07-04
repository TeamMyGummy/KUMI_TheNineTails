using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace AbilitySystem.Base
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
        [JsonProperty]
        public float BaseValue { get; private set; }
        [JsonProperty]
        public float CurrentValue { get; private set; }
        [JsonProperty]
        public float MaxValue { get; private set; }
        
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

    public class GameplayAttribute : MonoBehaviour
    {
        public Dictionary<string, Attribute> Attributes;

        public void SetAttribute(Dictionary<string, Attribute> dict)
        {
            Attributes = dict;
        }
    }
}