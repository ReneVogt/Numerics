using Revo.Numerics;
using System.Numerics;

namespace Numerics.Tests.BigDecimalTests;

public sealed partial class BigDecimalTests
{
    #region Sqrt
    [Theory]
    [MemberData(nameof(ProvideFailingSqrtTestCases))]
    [Trait("BigDecimal", "Advanced")]
    public void Sqrt_Negative_ThrowsArgumentOutOfRange(TestableBigDecimal value, int precision)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => BigDecimal.Sqrt(value.Value, precision));
        BigDecimalContext.Precision = precision;
        Assert.Throws<ArgumentOutOfRangeException>(() => BigDecimal.Sqrt(value.Value));
    }
    [Theory]
    [MemberData(nameof(ProvideFailingSqrtTestCases))]
    [Trait("BigDecimal", "Advanced")]
    public void Sqrt_Negative_ThrowsArithmeticException(TestableBigDecimal value, int precision)
    {
        Assert.Throws<ArithmeticException>(() => value.Value.Sqrt(precision));
        BigDecimalContext.Precision = precision;
        Assert.Throws<ArithmeticException>(() => value.Value.Sqrt());
    }
    [Fact]
    [Trait("BigDecimal", "Advanced")]
    public void Sqrt_NoExponent_Square()
    {
        var sut = new BigDecimal(25);
        var expected = new BigDecimal(5);
        Assert.Equal(expected, sut.Sqrt(precision: 30));
    }
    [Fact]
    [Trait("BigDecimal", "Advanced")]
    public void Sqrt_NoExponent_NonSquare()
    {
        var sut = new BigDecimal(13);
        var expected = new BigDecimal(3605, -3);
        Assert.Equal(expected, sut.Sqrt(precision: 3));
    }
    [Fact]
    [Trait("BigDecimal", "Advanced")]
    public void Sqrt_PositiveOddExponent()
    {
        var sut = new BigDecimal(250);
        var expected = new BigDecimal(15811, -3);
        Assert.Equal(expected, sut.Sqrt(precision: 3));
    }
    [Fact]
    [Trait("BigDecimal", "Advanced")]
    public void Sqrt_PositiveEvenExponent_Square()
    {
        var sut = new BigDecimal(400);
        var expected = new BigDecimal(20);
        Assert.Equal(expected, sut.Sqrt(precision: 3));
    }
    [Fact]
    [Trait("BigDecimal", "Advanced")]
    public void Sqrt_PositiveEvenExponent_NonSquare()
    {
        var sut = new BigDecimal(300);
        var expected = new BigDecimal(17320, -3);
        Assert.Equal(expected, sut.Sqrt(precision: 3));
    }
    [Fact]
    [Trait("BigDecimal", "Advanced")]
    public void Sqrt_NegativeOddExponent()
    {
        var sut = new BigDecimal(7, -1);
        var expected = new BigDecimal(836, -3);
        Assert.Equal(expected, sut.Sqrt(precision: 3));
    }
    [Fact]
    [Trait("BigDecimal", "Advanced")]
    public void Sqrt_NegativeEvenExponent_Square()
    {
        var sut = new BigDecimal(25, -4);
        var expected = new BigDecimal(5, -2);
        Assert.Equal(expected, sut.Sqrt(precision: 2));
    }
    [Fact]
    [Trait("BigDecimal", "Advanced")]
    public void Sqrt_NegativeEvenExponent_NonSquare()
    {
        var sut = new BigDecimal(7, -4);
        var expected = new BigDecimal(26, -3);
        Assert.Equal(expected, sut.Sqrt(precision: 3));
    }
    [Fact]
    [Trait("BigDecimal", "Advanced")]
    public void Sqrt_StrangePrecisionProblem()
    {
        var sut = new BigDecimal(2, 2);
        var expected = new BigDecimal(14142, -3);
        Assert.Equal(expected, sut.Sqrt(precision: 3));
    }

    [Theory]
    [MemberData(nameof(ProvideSqrtTestCases))]
    [Trait("BigDecimal", "Advanced")]
    public void Sqrt_Precision(TestableBigDecimal value, TestableBigDecimal expected, int precision)
    {
        Assert.Equal(expected.Value, value.Value.Sqrt(precision));
        Assert.Equal(expected.Value, BigDecimal.Sqrt(value.Value, precision));
    }
    [Theory]
    [MemberData(nameof(ProvideSqrtTestCases))]
    [Trait("BigDecimal", "Advanced")]
    public void Sqrt_GlobalPrecision(TestableBigDecimal value, TestableBigDecimal expected, int precision)
    {
        BigDecimalContext.Precision = precision;
        Assert.Equal(expected.Value, value.Value.Sqrt(precision));
        Assert.Equal(expected.Value, BigDecimal.Sqrt(value.Value, precision));
    }
    public static TheoryData<TestableBigDecimal, int> ProvideFailingSqrtTestCases() => new()
    {
        { BigDecimal.NegativeOne, 0 },
        { BigDecimal.NegativeOne, 10 },
        { new BigDecimal(-123), 2 },
        { new BigDecimal(-12, 100), 1000},
        { new BigDecimal(-17, -120), 200}
    };
    public static TheoryData<TestableBigDecimal, TestableBigDecimal, int> ProvideSqrtTestCases() => new()
    {
        { BigDecimal.Zero, BigDecimal.Zero, 0 },
        { BigDecimal.Zero, BigDecimal.Zero, 10 },
        { BigDecimal.One, BigDecimal.One, 0 },
        { BigDecimal.One, BigDecimal.One, 10 },

        { new BigDecimal(25), new BigDecimal(5), 2 },
        { new BigDecimal(25, -2), new BigDecimal(5, -1), 2 },
        { new BigDecimal(25, 2), new BigDecimal(5, 1), 2 },
        { new BigDecimal(25, 4), new BigDecimal(5, 2), 2 },

        { new BigDecimal(1, 1), new BigDecimal(3162277, -6), 6 },
        { new BigDecimal(1, -1), new BigDecimal(316227, -6), 6 },

        { new BigDecimal(25, -5), new BigDecimal(158113883, -10), 10},
        { new BigDecimal(10000), new BigDecimal(1,2), 0},

        { new BigDecimal(2), BigDecimal.Parse("1414213562373095048801688 E-24"), 24 },
        { new BigDecimal(20), BigDecimal.Parse("44721359549995793928183473374 E-28"), 28 },
        { new BigDecimal(200), BigDecimal.Parse("1414213562373095048801688 E-23"), 23 },
        { new BigDecimal(200), BigDecimal.Parse("1414 E-2"), 2 },
        { new BigDecimal(20000), BigDecimal.Parse("1414213562373095048801688 E-22"), 22 },
        { new BigDecimal(25, -4), BigDecimal.Parse("5 E-2"), 2 },
        { new BigDecimal(25, -4), BigDecimal.Parse("5 E-2"), 12 },

        { new BigDecimal(2, 20), BigDecimal.Parse("1414213562373 E-2"), 2 },

        { new BigDecimal(300), BigDecimal.Parse("1732 E-2"), 2 }
    };
    #endregion
}
