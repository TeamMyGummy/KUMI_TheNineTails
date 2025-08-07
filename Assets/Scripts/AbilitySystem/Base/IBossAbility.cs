using System;
using GameAbilitySystem;

public interface IBossAbility
{
    public Action OnNormalEnd { get; set; } //정상종료 시 호출

    public abstract void CancelAbility();
}
