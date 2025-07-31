using GameAbilitySystem;

public class FoxFlame : FireProjectile
{
    private GameplayEffect _decreaseCount = new InstantGameplayEffect("FoxFireCount", -1);
    protected override bool CanActivate()
    {
        if (Asc.Attribute.Attributes["FoxFireCount"].CurrentValue.Value <= 0) return false;
        return true;
    }

    protected override void Activate()
    {
        Asc.ApplyGameplayEffect(Asc, _decreaseCount);
        base.Activate();
    }
}