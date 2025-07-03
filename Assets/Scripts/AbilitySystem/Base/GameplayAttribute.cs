using System;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem.Base
{
    public enum ModOperation
    {
        Additive,
        Override,
        Multiplicative
    }

    public enum AttributeName
    {   
        HP,
        공격력
    }

    [Serializable]
    public class Attribute
    {
        //public float BaseValue { get; private set; }
        //public float CurrentValue { get; private set; }
        //public float MaxValue { get; private set; }

        [SerializeField] private float BaseValue;
        [SerializeField] private float CurrentValue;
        [SerializeField] private float MaxValue;


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

    [Serializable]
    public struct AttributeEntry
    {
        public AttributeName AttributeName;
        public Attribute Attribute;
    }

    public class GameplayAttribute : MonoBehaviour
    {
        // 인스펙터 노출용
        [SerializeField]
        private List<AttributeEntry> _attributes = new();

        // 딕셔너리로 저장 (실제로 접근은 여기로)
        public Dictionary<AttributeName, Attribute> Attributes = new();

        private void Start()
        {
            foreach (var attribute in _attributes)
            {
                
                Attributes.Add(attribute.AttributeName, attribute.Attribute);
            }
        }
    }
}