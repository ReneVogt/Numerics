using Revo.Numerics.Interpolation;

namespace Numerics.Tests.Interpolation.NewtonPolynomInterpolatorTests;

public sealed partial class NewtonPolynomInterpolatorTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Interpolate_OverflowingCoordinateDifference_PreservesLine(bool reverse)
    {
        VerifyExtremeLine([-1e308, 1e308], [-1, 1], 1e-308, 1e308, 1, reverse);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Interpolate_OverflowingOrdinateDifference_PreservesLine(bool reverse)
    {
        VerifyExtremeLine([-1, 1], [-1e308, 1e308], 1e308, 1, 1e308, reverse);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Interpolate_BothDifferencesOverflow_PreservesLine(bool reverse)
    {
        VerifyExtremeLine([-1e308, 1e308], [-1e308, 1e308], 1, 1e308, 1e308, reverse);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Interpolate_MaximumFiniteCoordinates_PreservesIdentity(bool reverse)
    {
        VerifyExtremeLine([-double.MaxValue, double.MaxValue], [-double.MaxValue, double.MaxValue],
            1, double.MaxValue, double.MaxValue, reverse);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Interpolate_HigherOrderCoordinateDifferenceOverflows_PreservesQuadratic(bool reverse)
    {
        double[] xs = [-1e308, 0, 1e308];
        double[] ys = [1e308, 0, 1e308];
        if (reverse)
        {
            Array.Reverse(xs);
            Array.Reverse(ys);
        }

        var interpolator = Interpolators.InterpolateNewtonPolynom(xs, ys);
        var coefficients = interpolator.Coefficients;
        Assert.All(coefficients, value => Assert.True(double.IsFinite(value)));
        Assert.InRange(Math.Abs(coefficients[2] / 1e-308 - 1), 0, 2e-15);
        foreach (var fraction in new[] { -1.0, -0.5, 0, 0.5, 1.0 })
        {
            var actual = interpolator.Evaluate(fraction * 1e308);
            Assert.True(double.IsFinite(actual));
            Assert.InRange(Math.Abs(actual / 1e308 - fraction * fraction), 0, 2e-15);
        }
    }

    static void VerifyExtremeLine(double[] xs, double[] ys, double slope, double xScale, double yScale, bool reverse)
    {
        if (reverse)
        {
            Array.Reverse(xs);
            Array.Reverse(ys);
        }

        var interpolator = Interpolators.InterpolateNewtonPolynom(xs, ys);
        var coefficients = interpolator.Coefficients;
        Assert.Equal(2, coefficients.Length);
        Assert.All(coefficients, value => Assert.True(double.IsFinite(value)));
        Assert.InRange(Math.Abs(coefficients[0] / yScale), 0, 2e-15);
        // Dividing by the expected slope ensures that zero cannot pass for 1e-308.
        Assert.InRange(Math.Abs(coefficients[1] / slope - 1), 0, 2e-15);

        foreach (var fraction in new[] { -1.0, -0.75, -0.25, 0, 0.25, 0.75, 1.0 })
        {
            var actual = interpolator.Evaluate(fraction * xScale);
            Assert.True(double.IsFinite(actual));
            Assert.InRange(Math.Abs(actual / yScale - fraction), 0, 2e-15);
        }
    }

    [Fact]
    public void Evaluate_ConstantAcrossOverflowingCoordinateDifference_ReturnsConstant()
    {
        var interpolator = Interpolators.InterpolateNewtonPolynom([-1e308, 1e308], [7, 7]);

        Assert.Equal([7, 0], interpolator.Coefficients);
        Assert.Equal(7, interpolator.Evaluate(1e308));
    }

    [Fact]
    public void Interpolate_SubnormalCoordinateDifference_PreservesDistinctNodes()
    {
        var interpolator = Interpolators.InterpolateNewtonPolynom(
            [double.Epsilon, 2 * double.Epsilon], [double.Epsilon, 2 * double.Epsilon]);

        Assert.Equal([0, 1], interpolator.Coefficients);
        Assert.Equal(double.Epsilon, interpolator.Evaluate(double.Epsilon));
        Assert.Equal(2 * double.Epsilon, interpolator.Evaluate(2 * double.Epsilon));
    }

    [Fact]
    public void Interpolate_SubnormalSlope_PreservesNonzeroCoefficient()
    {
        var interpolator = Interpolators.InterpolateNewtonPolynom([-1, 1], [-double.Epsilon, double.Epsilon]);

        Assert.Equal([0, double.Epsilon], interpolator.Coefficients);
        Assert.Equal(-double.Epsilon, interpolator.Evaluate(-1));
        Assert.Equal(double.Epsilon, interpolator.Evaluate(1));
    }

    [Fact]
    public void Interpolate_OverflowingNewtonCoefficient_ThrowsArithmeticException()
    {
        Assert.Throws<ArithmeticException>(() =>
            Interpolators.InterpolateNewtonPolynom([0, double.Epsilon], [0, 1]));
    }

    [Fact]
    public void Interpolate_NewtonCoefficientUnderflowsToZero_ThrowsArithmeticException()
    {
        Assert.Throws<ArithmeticException>(() =>
            Interpolators.InterpolateNewtonPolynom([-1e308, 0, 1e308], [1, 0, 1]));
        Assert.Throws<ArithmeticException>(() =>
            Interpolators.InterpolateNewtonPolynom([0, 2], [0, double.Epsilon]));
    }

    [Fact]
    public void Coefficients_OverflowingProductWithFiniteSum_PreservesLine()
    {
        var interpolator = Interpolators.InterpolateNewtonPolynom([-2, -1], [-1e308, 0]);

        Assert.Equal([1e308, 1e308], interpolator.Coefficients);
        Assert.Equal(-1e308, interpolator.Evaluate(-2));
        Assert.Equal(0, interpolator.Evaluate(-1));
        Assert.Equal(1e308, interpolator.Evaluate(0));
    }

    [Fact]
    public void Coefficients_UnrepresentableMonomialCoefficient_ThrowsOnlyOnAccess()
    {
        var interpolator = Interpolators.InterpolateNewtonPolynom(
            [1, 2], [-double.MaxValue, -double.MaxValue / 2]);

        Assert.Equal(-double.MaxValue, interpolator.Evaluate(1));
        Assert.Equal(-double.MaxValue / 2, interpolator.Evaluate(2));
        Assert.Throws<ArithmeticException>(() => interpolator.Coefficients);
        Assert.Equal(-double.MaxValue / 2, interpolator.Evaluate(2));
    }

    [Fact]
    public void Coefficients_MonomialCoefficientUnderflowsToZero_ThrowsOnlyOnAccess()
    {
        var interpolator = Interpolators.InterpolateNewtonPolynom([double.Epsilon, 1], [0, 0.25]);

        Assert.Equal(0, interpolator.Evaluate(double.Epsilon));
        Assert.Equal(0.25, interpolator.Evaluate(1));
        Assert.Throws<ArithmeticException>(() => interpolator.Coefficients);
        Assert.Equal(0.25, interpolator.Evaluate(1));
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void Interpolate_NonfiniteInput_RejectsCoordinate(double value)
    {
        Assert.Equal("x", Assert.Throws<ArgumentException>(() =>
            Interpolators.InterpolateNewtonPolynom([value], [1])).ParamName);
        Assert.Equal("y", Assert.Throws<ArgumentException>(() =>
            Interpolators.InterpolateNewtonPolynom([1], [value])).ParamName);
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void Evaluate_NonfiniteCoordinate_ThrowsArgumentOutOfRangeException(double value)
    {
        var interpolator = Interpolators.InterpolateNewtonPolynom([0, 1], [0, 1]);

        Assert.Throws<ArgumentOutOfRangeException>(() => interpolator.Evaluate(value));
    }

    [Fact]
    public void Evaluate_UnrepresentableValue_ThrowsArithmeticException()
    {
        var interpolator = Interpolators.InterpolateNewtonPolynom([0, 1], [0, double.MaxValue]);

        Assert.Throws<ArithmeticException>(() => interpolator.Evaluate(2));
    }
}
