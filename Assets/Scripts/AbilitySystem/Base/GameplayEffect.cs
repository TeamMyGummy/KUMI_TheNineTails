using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AbilitySystem.Base
{
    /// <summary>
    /// GameplayAttribute 값을 바꾸는 Game effect
    /// InstantGameplayEffect, DurationGameplayEffect 두 가지로 사용
    /// </summary>
    /// <param name="attributeName">Attribute 이름. 'AttributeName' Enum 값</param>
    /// <param name="delta">Effect를 적용할 값</param>
    /// <param name="mod">
    /// Effect 적용 방식(Additive, Override, Multiplicative)
    /// 생략 시 Additive가 기본값
    /// </param>
    public abstract class GameplayEffect
    {
        public string AttributeName { get; private set; }
        public float Delta { get; private set; }
        public ModOperation Mod = ModOperation.Additive;

        protected GameplayEffect(string attributeName, float delta)
        {
            AttributeName = attributeName;
            Delta = delta;
        }

        protected GameplayEffect(string attributeName, float delta, ModOperation mod)
        {
            AttributeName = attributeName;
            Delta = delta;
            Mod = mod;
        }

        public abstract void Apply(GameplayAttribute attribute);
    }

    /// <summary>
    /// GameplayEffect : Effect 값을 즉시 적용
    /// </summary>
    public class InstantGameplayEffect : GameplayEffect
    {
        public InstantGameplayEffect(string attributeName, float delta)
            : base(attributeName, delta) { }
        public InstantGameplayEffect(string attributeName, float delta, ModOperation mod)
            : base(attributeName, delta, mod) { }

        public override void Apply(GameplayAttribute attribute)
        {
            if (!attribute.Attributes.TryGetValue(AttributeName, out var att)) return;
            att.Modify(Delta, Mod);
        }
    }

    /// <summary>
    /// GameplayEffect : Effect 값을 일정 시간 지속적으로 적용
    /// duration: 5, interval: 1일 때 -> 1초 간격으로 5초 동안 Effect 적용
    /// </summary>
    /// <param name="duration">적용 시간</param>
    /// <param name="interval">적용 간격</param>
    public class DurationGameplayEffect : GameplayEffect
    {
        public float Duration;
        public float Interval;

        public DurationGameplayEffect(string attributeName, float delta, float duration, float interval)
            : base(attributeName, delta)
        {
            Duration = duration;
            Interval = interval;
        }

        public DurationGameplayEffect(string attributeName, float delta, ModOperation mod, float duration, float interval)
            : base(attributeName, delta, mod)
        {
            Duration = duration;
            Interval = interval;
        }

        public override async void Apply(GameplayAttribute attribute)
        {
            if (!attribute.Attributes.TryGetValue(AttributeName, out var att)) return;
            await ApplyWithInterval(att);
        }

        private async UniTask ApplyWithInterval(Attribute attribute)
        {
            int repeat = (int)(Duration / Interval);

            for (int i = 0; i < repeat; i++)
            {
                attribute.Modify(Delta, Mod);
                await UniTask.Delay(TimeSpan.FromSeconds(Interval), DelayType.DeltaTime);
            }
        }
    }
}