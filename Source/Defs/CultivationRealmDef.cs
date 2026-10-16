using Verse;

namespace PathOfCultivation;

public sealed class CultivationRealmDef : Def
{
    public int order;
    public float progressCap;
    public float qiCap;
    public CultivationRealmDef? nextRealm;
    public float breakthroughQiCost;
    public float breakthroughDurationHours;

    public override IEnumerable<string> ConfigErrors()
    {
        foreach (string error in base.ConfigErrors())
            yield return error;

        if (order < 0)
            yield return $"{defName}: order must be non-negative.";
        if (!float.IsFinite(progressCap) || progressCap < 0f)
            yield return $"{defName}: progressCap must be finite and non-negative.";
        if (!float.IsFinite(qiCap) || qiCap < 0f)
            yield return $"{defName}: qiCap must be finite and non-negative.";
        if (!float.IsFinite(breakthroughQiCost) || breakthroughQiCost < 0f)
            yield return $"{defName}: breakthroughQiCost must be finite and non-negative.";
        if (!float.IsFinite(breakthroughDurationHours) || breakthroughDurationHours < 0f)
            yield return $"{defName}: breakthroughDurationHours must be finite and non-negative.";

        if (breakthroughQiCost > qiCap)
            yield return $"{defName}: breakthroughQiCost must not exceed qiCap.";

        if (nextRealm is null)
        {
            if (breakthroughQiCost != 0f || breakthroughDurationHours != 0f)
                yield return $"{defName}: breakthrough parameters must be zero when nextRealm is absent.";
        }
        else
        {
            if (nextRealm == this)
                yield return $"{defName}: nextRealm must not reference this realm.";
            if (progressCap <= 0f)
                yield return $"{defName}: progressCap must be positive when nextRealm is set.";
            if (breakthroughDurationHours <= 0f)
                yield return $"{defName}: breakthroughDurationHours must be positive when nextRealm is set.";
        }
    }
}
