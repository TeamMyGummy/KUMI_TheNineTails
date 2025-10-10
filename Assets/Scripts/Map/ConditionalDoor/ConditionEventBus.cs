using System;

public static class ConditionEventBus
{
    public static event Action<ConditionKey> OnConditionMet;

    public static void Raise(ConditionKey key)
    {
        OnConditionMet?.Invoke(key);
    }
}