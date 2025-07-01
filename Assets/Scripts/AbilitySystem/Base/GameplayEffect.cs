using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AbilitySystem.Base
{
    public abstract class GameplayEffect
    {
        public string AttributeName;
        public float Delta;
        public ModOperation Mod = ModOperation.Additive;

        public abstract void Apply(GameplayAttribute attribute);
    }

    public class InstantGameplayEffect : GameplayEffect
    {
        public override void Apply(GameplayAttribute attribute)
        {
            if (!attribute.Attributes.TryGetValue(AttributeName, out var att)) return;
            att.Modify(Delta, Mod);
        }
    }

    public class DurationGameplayEffect : GameplayEffect
    {
        public float Duration;
        public float Interval;
        
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