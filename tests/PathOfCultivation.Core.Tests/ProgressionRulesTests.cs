using Microsoft.VisualStudio.TestTools.UnitTesting;
using PathOfCultivation.Core.Rules;

namespace PathOfCultivation.Core.Tests;

[TestClass]
public sealed class ProgressionRulesTests
{
    [TestMethod]
    public void TwoHoursAccrueFourProgressAndTenQi()
    {
        Assert.AreEqual(new ProgressQiResult(4m, 10m),
            ProgressionRules.AccruePractice(0m, 0m, 100m, 20m, 2m));
    }

    [TestMethod]
    public void ZeroDurationPreservesValidState()
    {
        Assert.AreEqual(new ProgressQiResult(1.25m, 19.75m),
            ProgressionRules.AccruePractice(1.25m, 19.75m, 100m, 20m, 0m));
    }

    [TestMethod]
    [DataRow(96, 10, 100, 20)]
    [DataRow(99, 19, 100, 20)]
    [DataRow(0, 20, 4, 20)]
    [DataRow(100, 0, 100, 10)]
    public void CapsAreReachedAndClippedIndependently(
        int progress, int qi, int expectedProgress, int expectedQi)
    {
        Assert.AreEqual(new ProgressQiResult(expectedProgress, expectedQi),
            ProgressionRules.AccruePractice(progress, qi, 100m, 20m, 2m));
    }

    [TestMethod]
    public void ZeroProgressCapAllowsQiToGrowAndSaturate()
    {
        Assert.AreEqual(new ProgressQiResult(0m, 10m),
            ProgressionRules.AccruePractice(0m, 0m, 0m, 30m, 2m));
        Assert.AreEqual(new ProgressQiResult(0m, 30m),
            ProgressionRules.AccruePractice(0m, 10m, 0m, 30m, 10m));
    }

    [TestMethod]
    public void BothZeroCapsRemainZero()
    {
        Assert.AreEqual(new ProgressQiResult(0m, 0m),
            ProgressionRules.AccruePractice(0m, 0m, 0m, 0m, decimal.MaxValue));
    }

    [TestMethod]
    [DataRow(10000, 1, 2, 5)]
    [DataRow(500000, 50, 100, 20)]
    public void PartitionedTimeMatchesSingleAccrualExactly(
        int steps, int hours, int expectedProgress, int expectedQi)
    {
        var expected = new ProgressQiResult(expectedProgress, expectedQi);
        Assert.AreEqual(expected,
            ProgressionRules.AccruePractice(0m, 0m, 100m, 20m, hours));

        var result = new ProgressQiResult(0m, 0m);
        for (int i = 0; i < steps; i++)
            result = ProgressionRules.AccruePractice(
                result.Progress, result.Qi, 100m, 20m, 0.0001m);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void ExcessIsDiscardedAcrossCalls()
    {
        var full = ProgressionRules.AccruePractice(0m, 0m, 100m, 20m, 100m);
        Assert.AreEqual(new ProgressQiResult(100m, 20m), full);
        Assert.AreEqual(full,
            ProgressionRules.AccruePractice(full.Progress, full.Qi, 100m, 20m, 2m));
        Assert.AreEqual(new ProgressQiResult(0m, 0m),
            ProgressionRules.AccruePractice(0m, 0m, 100m, 20m, 0m));
    }

    [TestMethod]
    public void InvalidInputsThrowWithCorrectParameterEvenAtZeroDuration()
    {
        foreach (decimal hours in new[] { 0m, 2m })
        {
            (decimal Progress, decimal Qi, decimal ProgressCap, decimal QiCap, string Parameter)[] cases =
            [
                (-1m, 0m, 100m, 20m, "currentProgress"),
                (0m, -1m, 100m, 20m, "currentQi"),
                (0m, 0m, -1m, 20m, "progressCap"),
                (0m, 0m, 100m, -1m, "qiCap"),
                (101m, 0m, 100m, 20m, "currentProgress"),
                (0m, 21m, 100m, 20m, "currentQi")
            ];
            foreach (var input in cases)
            {
                var exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
                    ProgressionRules.AccruePractice(input.Progress, input.Qi,
                        input.ProgressCap, input.QiCap, hours));
                Assert.AreEqual(input.Parameter, exception.ParamName);
            }
        }

        var negativeDuration = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            ProgressionRules.AccruePractice(0m, 0m, 100m, 20m, -1m));
        Assert.AreEqual("elapsedHours", negativeDuration.ParamName);
    }

    [TestMethod]
    public void MaximumDurationSafelySaturatesOrdinaryAndMaximumCaps()
    {
        Assert.AreEqual(new ProgressQiResult(100m, 20m),
            ProgressionRules.AccruePractice(0m, 0m, 100m, 20m, decimal.MaxValue));
        Assert.AreEqual(new ProgressQiResult(decimal.MaxValue, decimal.MaxValue),
            ProgressionRules.AccruePractice(0m, 0m,
                decimal.MaxValue, decimal.MaxValue, decimal.MaxValue));
    }

    [TestMethod]
    public void LargeCurrentValuesSafelySaturateWhenAdditionWouldOverflow()
    {
        decimal current = decimal.MaxValue - 1m;
        Assert.AreEqual(new ProgressQiResult(decimal.MaxValue, decimal.MaxValue),
            ProgressionRules.AccruePractice(current, current,
                decimal.MaxValue, decimal.MaxValue, 1m));
        Assert.AreEqual(new ProgressQiResult(current + 0.2m, current + 0.5m),
            ProgressionRules.AccruePractice(current, current,
                decimal.MaxValue, decimal.MaxValue, 0.1m));
    }

    [TestMethod]
    public void TinyDurationDoesNotPrematurelySaturateQi()
    {
        Assert.AreEqual(new ProgressQiResult(0m, 0.0000000000000000000000000005m),
            ProgressionRules.AccruePractice(0m, 0m, 0m,
                0.0000000000000000000000000007m,
                0.0000000000000000000000000001m));
    }
}
