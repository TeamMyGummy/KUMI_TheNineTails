using System;
using Cysharp.Threading.Tasks;
using GameAbilitySystem;

public static class GameplayExtensions
{
    public static async UniTask DelayOneFrame(this GameplayAbility ability)
    {
        await UniTask.Yield(PlayerLoopTiming.Update);
        AbilityFactory.Instance.EndAbility(ability);
    }
}
