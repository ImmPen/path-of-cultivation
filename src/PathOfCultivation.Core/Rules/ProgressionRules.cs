namespace PathOfCultivation.Core.Rules;

public readonly record struct ProgressQiResult(decimal Progress, decimal Qi);

public static class ProgressionRules
{
    private const decimal ProgressPerHour = 2m;
    private const decimal QiPerHour = 5m;

    public static ProgressQiResult AccruePractice(
        decimal currentProgress,
        decimal currentQi,
        decimal progressCap,
        decimal qiCap,
        decimal elapsedHours)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(progressCap);
        ArgumentOutOfRangeException.ThrowIfNegative(qiCap);
        ArgumentOutOfRangeException.ThrowIfNegative(currentProgress);
        ArgumentOutOfRangeException.ThrowIfNegative(currentQi);
        ArgumentOutOfRangeException.ThrowIfNegative(elapsedHours);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(currentProgress, progressCap);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(currentQi, qiCap);

        if (elapsedHours == 0m)
            return new(currentProgress, currentQi);

        return new(
            Accrue(currentProgress, progressCap, elapsedHours, ProgressPerHour),
            Accrue(currentQi, qiCap, elapsedHours, QiPerHour));
    }

    private static decimal Accrue(decimal current, decimal cap, decimal hours, decimal rate)
    {
        if (current == cap)
            return cap;

        try
        {
            // Normal decimal rounding applies; arbitrary scales need not retain exact sums.
            return Math.Min(cap, current + hours * rate);
        }
        catch (OverflowException)
        {
            // All operands are non-negative. Overflow of either operation exceeds any cap.
            return cap;
        }
    }
}
