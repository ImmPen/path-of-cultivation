using Verse;

namespace PathOfCultivation;

public enum CultivationTechniqueEffectKind
{
    PassiveHediff
}

public sealed class CultivationTechniqueDef : Def
{
    public CultivationRealmDef? minRealm;
    public float learnDurationHours;
    public CultivationTechniqueEffectKind? effectKind;
    public HediffDef? effectDef;

    public override IEnumerable<string> ConfigErrors()
    {
        foreach (string error in base.ConfigErrors())
            yield return error;

        if (minRealm is null)
            yield return $"{defName}: minRealm is required.";
        if (!float.IsFinite(learnDurationHours) || learnDurationHours <= 0f)
            yield return $"{defName}: learnDurationHours must be finite and positive.";
        if (effectKind != CultivationTechniqueEffectKind.PassiveHediff)
            yield return $"{defName}: effectKind must be PassiveHediff.";
        if (effectDef is null)
            yield return $"{defName}: effectDef is required.";
    }
}
