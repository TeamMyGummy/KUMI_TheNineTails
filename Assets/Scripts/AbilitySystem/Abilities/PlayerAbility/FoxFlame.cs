using GameAbilitySystem;

public class FoxFlame : FireProjectile
{
    private GameplayEffect _decreaseCount = new InstantGameplayEffect("FoxFireCount", -1);
    public override bool CanActivate()
    {
        if (Asc.Attribute.Attributes["FoxFireCount"].CurrentValue.Value <= 0) return false;
        return true;
    }

    protected override void Activate()
    {
        Asc.ApplyGameplayEffect(Asc, _decreaseCount);
        ResetFoxFireGauge();
        base.Activate();
    }

    private void ResetFoxFireGauge()
    {
        var gaugeAttr = Asc.Attribute.Attributes["FoxFireGauge"];
        float curGauge = gaugeAttr.CurrentValue.Value;
        if (curGauge > 0f)
        {
            Asc.ApplyGameplayEffect(Asc, new InstantGameplayEffect("FoxFireGauge", -curGauge));
        }
    }
}