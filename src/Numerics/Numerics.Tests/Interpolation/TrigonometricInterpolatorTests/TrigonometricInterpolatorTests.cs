using Revo.Numerics.Interpolation;

namespace Numerics.Tests.Interpolation.TrigonometricInterpolatorTests;

public sealed class TrigonometricInterpolatorTests
{
    // Contract: [a0, b0, a1, b1, ...], with b0 = 0 and omega = 2*pi/period.
    // For an even number N of nodes, the highest harmonic has only a cosine
    // coefficient; its sine coefficient is exposed as zero as well.
    public static TheoryData<string, double, double[], double[]> ReferenceCases => new()
    {
        { "constant", 5, [-0.6], [7, 0] },
        { "cosine", 1, [0, 0.5], [0, 0, 3, 0] },
        { "zero", 5, [0, 0.25, 0.5, 0.75], [0, 0, 0, 0, 0, 0] },
        { "constant", 1, [0, 0.2, 0.4, 0.6, 0.8], [7, 0, 0, 0, 0, 0] },
        { "cosine", 2 * Math.PI, [0, 1d / 3, 2d / 3], [0, 0, 3, 0] },
        { "sine", 5, [0, 1d / 3, 2d / 3], [0, 0, 0, -4] },
        { "mixed", 1, [0, 1d / 3, 2d / 3], [2, 0, 3, -4] },
        { "mixed", 5, [0, 0.25, 0.5, 0.75], [2, 0, 3, -4, 0, 0] },
        { "secondCosine", 5, [0, 0.25, 0.5, 0.75], [2, 0, 3, -4, 1.5, 0] },
        { "second", 2 * Math.PI, [0, 0.2, 0.4, 0.6, 0.8], [2, 0, 3, -4, 1.5, -0.75] },
        { "thirdCosine", 1, [0, 1d / 6, 2d / 6, 3d / 6, 4d / 6, 5d / 6], [2, 0, 3, -4, 1.5, -0.75, 2.25, 0] },
        { "third", 5, [0, 1d / 7, 2d / 7, 3d / 7, 4d / 7, 5d / 7, 6d / 7], [2, 0, 3, -4, 1.5, -0.75, 2.25, -1.25] },
        // Nonuniform nodes, then the same phases reordered and shifted by whole periods.
        { "second", 5, [0.03, 0.19, 0.42, 0.63, 0.86], [2, 0, 3, -4, 1.5, -0.75] },
        { "second", 5, [1.63, -0.81, 2.86, -0.97, 0.42], [2, 0, 3, -4, 1.5, -0.75] },
        { "secondCosine", 1, [0, 0.2, 0.5, 0.8], [2, 0, 3, -4, 1.5, 0] }
    };

    [Fact]
    public void SimpleCaseForEasyDebug_Odd()
    {
        var x = new[] { 0d, Math.PI/2, Math.PI };
        var y = new[] { 1d, Math.Cos(Math.PI/2), 0d };

        var interpolator = Interpolators.InterpolateTrigonometric(x, y, 2*Math.PI);
        Assert.Equal(y[0], interpolator.Evaluate(x[0]), 1e-8);
        Assert.Equal(y[1], interpolator.Evaluate(x[1]), 1e-8);
        Assert.Equal(y[2], interpolator.Evaluate(x[2]), 1e-8);
    }
    [Fact]
    public void SimpleCaseForEasyDebug_Even()
    {
        var x = new[] { 0d, Math.PI/2, Math.PI, 3 * Math.PI/2 };
        var y = new[] { 1d, Math.Cos(Math.PI/2), 0d, Math.Cos(3 * Math.PI/2) };

        var interpolator = Interpolators.InterpolateTrigonometric(x, y, 2*Math.PI);
        Assert.Equal(y[0], interpolator.Evaluate(x[0]), 1e-8);
        Assert.Equal(y[1], interpolator.Evaluate(x[1]), 1e-8);
        Assert.Equal(y[2], interpolator.Evaluate(x[2]), 1e-8);
        Assert.Equal(y[3], interpolator.Evaluate(x[3]), 1e-8);
    }

    [Theory]
    [MemberData(nameof(ReferenceCases))]
    public void Interpolate_KnownFunction_ReturnsExpectedCoefficients(
        string function, double period, double[] phases, double[] expected)
    {
        var interpolator = CreateReference(function, period, phases);
        var actual = interpolator.Coefficients;

        AssertCoefficients(expected, actual);
        Assert.Equal(2 * (phases.Length / 2 + 1), actual.Length);
        Assert.Equal(0d, actual[1]);
        if (phases.Length % 2 == 0)
            Assert.Equal(0d, actual[^1]);
    }

    public static IEnumerable<object[]> ReferenceNodeCases => ReferenceCases.Select(data => data[..3]);

    [Theory]
    [MemberData(nameof(ReferenceNodeCases))]
    public void Interpolate_KnownFunction_RoundtripsNodes(
        string function, double period, double[] phases)
    {
        var interpolator = CreateReference(function, period, phases);

        foreach (var phase in phases)
            AssertClose(ReferenceValue(function, phase), interpolator.Evaluate(phase * period));
    }

    [Theory]
    [MemberData(nameof(ReferenceNodeCases))]
    public void Evaluate_KnownFunction_MatchesBetweenAndBeyondNodes(
        string function, double period, double[] phases)
    {
        var interpolator = CreateReference(function, period, phases);

        foreach (var phase in new[] { -1.37, -0.11, 0.07, 0.37, 0.91, 2.13 })
            AssertClose(ReferenceValue(function, phase), interpolator.Evaluate(phase * period));
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    public void Interpolate_ArbitraryValues_RoundtripsNodes(int count)
    {
        const double period = 5;
        var x = Enumerable.Range(0, count).Select(i => period * i / count).ToArray();
        double[] values = [3.25, -7, 0, 2, -0.125, 11, 4.5, -2];
        var y = values[..count];
        var interpolator = Interpolators.InterpolateTrigonometric(x, y, period);

        for (var i = 0; i < count; i++)
            AssertClose(y[i], interpolator.Evaluate(x[i]));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(2 * Math.PI)]
    public void Evaluate_RepeatsAfterWholePeriods(double period)
    {
        var interpolator = CreateReference("second", period, [0, 0.2, 0.4, 0.6, 0.8]);
        var x = 0.137 * period;
        var expected = ReferenceValue("second", 0.137);

        foreach (var shift in new[] { -3, -1, 0, 1, 4 })
            AssertClose(expected, interpolator.Evaluate(x + shift * period));
    }

    [Fact]
    public void Interpolate_EmptyArrays_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => Interpolators.InterpolateTrigonometric([], [], 5));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Interpolate_NullArray_ThrowsArgumentNullException(bool nullX)
    {
        var exception = Assert.Throws<ArgumentNullException>(() =>
            Interpolators.InterpolateTrigonometric(nullX ? null! : [0], nullX ? [7] : null!, 5));

        Assert.Equal(nullX ? "x" : "y", exception.ParamName);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 0)]
    [InlineData(2, 3)]
    [InlineData(3, 2)]
    public void Interpolate_DifferentLengths_ThrowsArgumentException(int xLength, int yLength)
    {
        var x = Enumerable.Range(0, xLength).Select(i => (double)i).ToArray();
        Assert.Throws<ArgumentException>(() =>
            Interpolators.InterpolateTrigonometric(x, new double[yLength], 5));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(double.NaN)]
    [InlineData(double.NegativeInfinity)]
    [InlineData(double.PositiveInfinity)]
    public void Interpolate_InvalidPeriod_ThrowsArgumentException(double period)
    {
        // A single valid node isolates period validation from matrix solvability.
        Assert.ThrowsAny<ArgumentException>(() =>
            Interpolators.InterpolateTrigonometric([0], [7], period));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(6)]
    public void Interpolate_DoesNotModifyOrAliasInputs(int count)
    {
        const double period = 5;
        var x = Enumerable.Range(0, count).Select(i => period * i / count).ToArray();
        var function = count == 1 ? "constant" : "second";
        var y = x.Select(value => ReferenceValue(function, value / period)).ToArray();
        var originalX = x.ToArray();
        var originalY = y.ToArray();
        var interpolator = Interpolators.InterpolateTrigonometric(x, y, period);
        var originalCoefficients = interpolator.Coefficients;

        Assert.Equal(originalX, x);
        Assert.Equal(originalY, y);
        Assert.NotSame(x, originalCoefficients);
        Assert.NotSame(y, originalCoefficients);
        Array.Fill(x, 100);
        Array.Fill(y, 200);

        Assert.Equal(originalCoefficients, interpolator.Coefficients);
        AssertClose(ReferenceValue(function, 0.137), interpolator.Evaluate(0.137 * period));
    }

    [Fact]
    public void Coefficients_ReturnsIndependentCopies_WithoutChangingEvaluation()
    {
        var interpolator = CreateReference("mixed", 5, [0, 1d / 3, 2d / 3]);
        var first = interpolator.Coefficients;
        var second = interpolator.Coefficients;

        AssertCoefficients([2, 0, 3, -4], first);
        Assert.NotSame(first, second);
        first[0] = 100;
        second[2] = 200;

        AssertCoefficients([100, 0, 3, -4], first);
        AssertCoefficients([2, 0, 200, -4], second);
        AssertCoefficients([2, 0, 3, -4], interpolator.Coefficients);
        AssertClose(ReferenceValue("mixed", 0.137), interpolator.Evaluate(0.137 * 5));
    }

    [Fact]
    public void Create_AndFactory_ReturnEquivalentInterpolators()
    {
        double[] x = [0, 1, 2, 3];
        double[] y = [6.5, -3.5, 0.5, 4.5];
        var direct = TrigonometricInterpolator.Create(x, y, 4);
        var factory = Interpolators.InterpolateTrigonometric(x, y, 4);

        AssertCoefficients([2, 0, 3, -4, 1.5, 0], direct.Coefficients);
        AssertCoefficients(direct.Coefficients, factory.Coefficients);
        AssertClose(ReferenceValue("secondCosine", 0.137), direct.Evaluate(0.137 * 4));
        AssertClose(direct.Evaluate(0.137 * 4), factory.Evaluate(0.137 * 4));
    }

    static IInterpolator CreateReference(string function, double period, double[] phases) =>
        Interpolators.InterpolateTrigonometric(
            [.. phases.Select(phase => phase * period)],
            [.. phases.Select(phase => ReferenceValue(function, phase))], period);

    // Explicit analytic functions, independent of the coefficient layout and implementation.
    static double ReferenceValue(string function, double phase)
    {
        var t = 2 * Math.PI * phase;
        return function switch
        {
            "zero" => 0,
            "constant" => 7,
            "cosine" => 3 * Math.Cos(t),
            "sine" => -4 * Math.Sin(t),
            "mixed" => 2 + 3 * Math.Cos(t) - 4 * Math.Sin(t),
            "secondCosine" => 2 + 3 * Math.Cos(t) - 4 * Math.Sin(t) + 1.5 * Math.Cos(2 * t),
            "second" => 2 + 3 * Math.Cos(t) - 4 * Math.Sin(t) + 1.5 * Math.Cos(2 * t) - 0.75 * Math.Sin(2 * t),
            "thirdCosine" => 2 + 3 * Math.Cos(t) - 4 * Math.Sin(t) + 1.5 * Math.Cos(2 * t) - 0.75 * Math.Sin(2 * t) + 2.25 * Math.Cos(3 * t),
            "third" => 2 + 3 * Math.Cos(t) - 4 * Math.Sin(t) + 1.5 * Math.Cos(2 * t) - 0.75 * Math.Sin(2 * t) + 2.25 * Math.Cos(3 * t) - 1.25 * Math.Sin(3 * t),
            _ => throw new ArgumentException("Unknown reference function.", nameof(function))
        };
    }

    static void AssertCoefficients(double[] expected, double[] actual)
    {
        Assert.Equal(expected.Length, actual.Length);
        for (var i = 0; i < expected.Length; i++)
            AssertClose(expected[i], actual[i]);
    }

    static void AssertClose(double expected, double actual)
    {
        var tolerance = 1e-12 * Math.Max(1.0, Math.Abs(expected));
        Assert.True(Math.Abs(expected - actual) <= tolerance,
            $"Expected {expected:R}, actual {actual:R}, tolerance {tolerance:R}.");
    }
}
