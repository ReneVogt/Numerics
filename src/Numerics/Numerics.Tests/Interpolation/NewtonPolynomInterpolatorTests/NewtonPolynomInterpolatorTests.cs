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

        var solution = Revo.Numerics.Interpolation.NewtonPolynomInterpolator.Interpolate(xs, ys);
        Assert.Equal(xs.Length, solution.Length);
        for (var i = 0; i < expected.Length; i++)
            AssertClose(expected[i], solution[i]);
        Validate(xs, ys, solution);
    }

    [Fact]
    public void Interpolate_EmptyArrays_ReturnsEmptyArray()
    {
        Assert.Empty(Revo.Numerics.Interpolation.NewtonPolynomInterpolator.Interpolate([], []));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Interpolate_NullArray_ThrowsArgumentNullException(bool nullX)
    {
        var exception = Assert.Throws<ArgumentNullException>(() =>
            Revo.Numerics.Interpolation.NewtonPolynomInterpolator.Interpolate(
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
            Revo.Numerics.Interpolation.NewtonPolynomInterpolator.Interpolate(new double[xLength], new double[yLength]));
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
            Revo.Numerics.Interpolation.NewtonPolynomInterpolator.Interpolate(xs, ys));
    }

    [Theory]
    [InlineData(-1.5)]
    [InlineData(0.25)]
    [InlineData(2.0)]
    [InlineData(4.0)]
    public void Interpolate_CubicPolynomial_EvaluatesBetweenAndBeyondNodes(double x)
    {
        var solution = Revo.Numerics.Interpolation.NewtonPolynomInterpolator.Interpolate(
            [-2, -0.5, 1, 3], [0, 1.125, 9, 65]);

        AssertClose(x * x * x + 3 * x * x + 3 * x + 2, Evaluate(solution, x));
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

        var solution = Revo.Numerics.Interpolation.NewtonPolynomInterpolator.Interpolate(xs, ys);

        Assert.Equal(originalX, xs);
        Assert.Equal(originalY, ys);
        Assert.NotSame(xs, solution);
        Assert.NotSame(ys, solution);
        var originalSolution = solution.ToArray();
        xs[0] = 100;
        ys[0] = 200;
        Assert.Equal(originalSolution, solution);
    }

    [Fact]
    public void Interpolate_SameArrayForBothCoordinates_ReturnsIdentityPolynomial()
    {
        double[] points = [-2, 1, 3];

        var solution = Revo.Numerics.Interpolation.NewtonPolynomInterpolator.Interpolate(points, points);

        Assert.Equal(new double[] { 0, 1, 0 }, solution);
        Assert.Equal(new double[] { -2, 1, 3 }, points);
    }

    static void Validate(double[] x, double[] y, double[] solution)
    {
        for (var i = 0; i < x.Length; i++)
            AssertClose(y[i], Evaluate(solution, x[i]));
    }

    static double Evaluate(double[] coefficients, double x)
    {
        var result = 0.0;
        for (var i = coefficients.Length - 1; i >= 0; i--)
            result = result * x + coefficients[i];
        return result;
    }

    static void AssertClose(double expected, double actual)
    {
        var tolerance = 1e-12 * Math.Max(1.0, Math.Abs(expected));
        Assert.True(Math.Abs(expected - actual) <= tolerance,
            $"Expected {expected:R}, actual {actual:R}, tolerance {tolerance:R}.");
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex MatrixSplitRegex();
}
