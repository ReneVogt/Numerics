using Revo.Numerics.Interpolation;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Numerics.Tests.Interpolation.NewtonPolynomInterpolatorTests;

public sealed partial class NewtonPolynomInterpolatorTests
{
    [
        Theory,
        InlineData(
            "0 1",
            "0 1",
            "0 1"),
        InlineData(
            "1 2 3",
            "0 1 4",
            "1 -2 1"),
        InlineData("-3", "7", "7"),
        InlineData("-2 0 3 5", "0 0 0 0", "0 0 0 0"),
        InlineData("-2 0 3 5", "7 7 7 7", "7 0 0 0"),
        InlineData("-2 0 3 5", "7 3 -3 -7", "3 -2 0 0"),
        InlineData("-2 -0.5 1 3", "0 1.125 9 65", "2 3 3 1"),
        InlineData("3 1 -0.5 -2", "65 9 1.125 0", "2 3 3 1"),
        InlineData("1 -2 3 -0.5", "9 0 65 1.125", "2 3 3 1"),
        InlineData("-2 -1 0 1 2", "35 4 1 2 31", "1 -1 0 0 2"),
        InlineData("0.1 0.3 0.8", "0.48 0.295 0.445", "0.625 -1.625 1.75")
    ]
    public void Interpolate_CorrectResults(string x, string y, string coefficients)
    {
        var xs = MatrixSplitRegex().Split(x).Select(s => double.Parse(s, CultureInfo.InvariantCulture)).ToArray();
        var ys = MatrixSplitRegex().Split(y).Select(s => double.Parse(s, CultureInfo.InvariantCulture)).ToArray();
        var expected = MatrixSplitRegex().Split(coefficients).Select(s => double.Parse(s, CultureInfo.InvariantCulture)).ToArray();

        var interpolator = Interpolators.InterpolateNewtonPolynom(xs, ys);
        var solution = interpolator.Coefficients;
        Assert.Equal(xs.Length, solution.Length);
        for (var i = 0; i < expected.Length; i++)
            AssertClose(expected[i], solution[i]);
        Validate(xs, ys, interpolator);
    }

    [Fact]
    public void Interpolate_EmptyArrays_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            Interpolators.InterpolateNewtonPolynom([], []));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Interpolate_NullArray_ThrowsArgumentNullException(bool nullX)
    {
        var exception = Assert.Throws<ArgumentNullException>(() =>
            Interpolators.InterpolateNewtonPolynom(
                nullX ? null! : [], nullX ? [] : null!));

        Assert.Equal(nullX ? "x" : "y", exception.ParamName);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 0)]
    [InlineData(2, 3)]
    [InlineData(3, 2)]
    public void Interpolate_DifferentLengths_ThrowsArgumentException(int xLength, int yLength)
    {
        Assert.Throws<ArgumentException>(() =>
            Interpolators.InterpolateNewtonPolynom(new double[xLength], new double[yLength]));
    }

    [Theory]
    [InlineData("1 1 2", "3 3 5")]
    [InlineData("1 1 2", "3 4 5")]
    [InlineData("1 2 1", "3 5 3")]
    [InlineData("1 2 1", "3 5 4")]
    [InlineData("0 1 -0", "3 5 3")]
    public void Interpolate_DuplicateXCoordinates_ThrowsArgumentException(string x, string y)
    {
        var xs = MatrixSplitRegex().Split(x).Select(s => double.Parse(s, CultureInfo.InvariantCulture)).ToArray();
        var ys = MatrixSplitRegex().Split(y).Select(s => double.Parse(s, CultureInfo.InvariantCulture)).ToArray();

        Assert.Throws<ArgumentException>(() =>
            Interpolators.InterpolateNewtonPolynom(xs, ys));
    }

    [Theory]
    [InlineData(-1.5)]
    [InlineData(0.25)]
    [InlineData(2.0)]
    [InlineData(4.0)]
    public void Interpolate_CubicPolynomial_EvaluatesBetweenAndBeyondNodes(double x)
    {
        var interpolator = Interpolators.InterpolateNewtonPolynom(
            [-2, -0.5, 1, 3], [0, 1.125, 9, 65]);

        AssertClose(x * x * x + 3 * x * x + 3 * x + 2, interpolator.Evaluate(x));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    public void Interpolate_DoesNotModifyOrAliasInputs(int count)
    {
        double[] xs = [1, 2, 3];
        double[] ys = [0, 1, 4];
        xs = xs[..count];
        ys = ys[..count];
        var originalX = xs.ToArray();
        var originalY = ys.ToArray();

        var interpolator = Interpolators.InterpolateNewtonPolynom(xs, ys);

        Assert.Equal(originalX, xs);
        Assert.Equal(originalY, ys);
        Assert.NotSame(xs, interpolator.Coefficients);
        Assert.NotSame(ys, interpolator.Coefficients);
        var originalSolution = interpolator.Coefficients.ToArray();
        xs[0] = 100;
        ys[0] = 200;
        Assert.Equal(originalSolution, interpolator.Coefficients);
        AssertClose(count == 1 ? 0 : 2.25, interpolator.Evaluate(2.5));
    }

    [Fact]
    public void Coefficients_ReturnsIndependentCopies_WithoutChangingEvaluation()
    {
        var interpolator = Interpolators.InterpolateNewtonPolynom([1, 2, 3], [0, 1, 4]);
        var first = interpolator.Coefficients;
        var second = interpolator.Coefficients;

        Assert.NotSame(first, second);
        first[0] = 100;
        second[1] = 200;

        Assert.Equal([100, -2, 1], first);
        Assert.Equal([1, 200, 1], second);
        Assert.Equal([1, -2, 1], interpolator.Coefficients);
        AssertClose(2.25, interpolator.Evaluate(2.5));
    }

    [Theory]
    [InlineData(-10)]
    [InlineData(0)]
    [InlineData(4.5)]
    public void Interpolate_SinglePoint_EvaluatesConstantAwayFromNode(double x)
    {
        var interpolator = Interpolators.InterpolateNewtonPolynom([-3], [7]);

        Assert.Equal(7, interpolator.Evaluate(x));
    }

    [Fact]
    public void Interpolate_SameArrayForBothCoordinates_ReturnsIdentityPolynomial()
    {
        double[] points = [-2, 1, 3];

        var interpolator = Interpolators.InterpolateNewtonPolynom(points, points);

        Assert.Equal([0, 1, 0], interpolator.Coefficients);
        Assert.Equal([-2, 1, 3], points);
    }

    static void Validate(double[] x, double[] y, IInterpolator interpolator)
    {
        for (var i = 0; i < x.Length; i++)
            AssertClose(y[i], interpolator.Evaluate(x[i]));
    }

    static void AssertClose(double expected, double actual)
    {
        var tolerance = 1e-12 * Math.Max(1.0, Math.Abs(expected));
        Assert.True(Math.Abs(expected - actual) <= tolerance,
            $"Expected {expected:R}, actual {actual:R}, tolerance {tolerance:R}.");
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(2, 4)]
    [InlineData(0.5, 0.25)]
    [InlineData(3, 9)]
    public void Evaluate_TranslatedQuadratic_PreservesAccuracy(double offset, double expected)
    {
        var interpolator = Interpolators.InterpolateNewtonPolynom(
            [100000000, 100000001, 100000002], [0, 1, 4]);

        AssertClose(expected, interpolator.Evaluate(100000000 + offset));
        _ = interpolator.Coefficients;
        AssertClose(expected, interpolator.Evaluate(100000000 + offset));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1e155)]
    [InlineData(2e155)]
    public void Evaluate_ConstantAtLargeNodes_ReturnsConstant(double x)
    {
        var interpolator = Interpolators.InterpolateNewtonPolynom(
            [0, 1e155, 2e155], [7, 7, 7]);

        Assert.Equal(7, interpolator.Evaluate(x));
        _ = interpolator.Coefficients;
        Assert.Equal(7, interpolator.Evaluate(x));
    }

    [Fact]
    public void Coefficients_InputsChangedBeforeFirstAccess_UsesOriginalPoints()
    {
        double[] xs = [1, 2, 3];
        double[] ys = [0, 1, 4];
        var interpolator = Interpolators.InterpolateNewtonPolynom(xs, ys);

        xs[0] = 100;
        ys[0] = 200;

        AssertClose(2.25, interpolator.Evaluate(2.5));
        Assert.Equal([1, -2, 1], interpolator.Coefficients);
        AssertClose(2.25, interpolator.Evaluate(2.5));
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex MatrixSplitRegex();
}
