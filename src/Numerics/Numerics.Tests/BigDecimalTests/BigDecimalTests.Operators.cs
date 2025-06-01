using Revo.Numerics;
using System.Numerics;

namespace Numerics.Tests.BigDecimalTests;

public sealed partial class BigDecimalTests
{
    #region Unary
    [Fact]
    [Trait("BigDecimal", "Operators")]
    public void UnaryPlus_Identity()
    {
        Assert.Equal(BigDecimal.One, +BigDecimal.One);
        Assert.Equal(BigDecimal.NegativeOne, +BigDecimal.NegativeOne);
        Assert.Equal(BigDecimal.Zero, +BigDecimal.Zero);
    }
    [Fact]
    [Trait("BigDecimal", "Operators")]
    public void UnaryPlusPlus_PreIncrement()
    {
        var sut = new BigDecimal(1, 3);
        var test = ++sut;
        Assert.Equal(1001, test);
        Assert.Equal(1001, sut);
    }
    [Fact]
    [Trait("BigDecimal", "Operators")]
    public void UnaryPlusPlus_PostIncrement()
    {
        var sut = new BigDecimal(1, 3);
        var test = sut++;
        Assert.Equal(1000, test);
        Assert.Equal(1001, sut);
    }
    [Fact]
    [Trait("BigDecimal", "Operators")]
    public void UnaryMinus_Negation()
    {
        Assert.Equal((BigDecimal)(-123), -(BigDecimal)123);
        Assert.Equal((BigDecimal)(123), -(BigDecimal)(-123));
        Assert.Equal(BigDecimal.Zero, -BigDecimal.Zero);
    }
    [Fact]
    [Trait("BigDecimal", "Operators")]
    public void UnaryMinusMinus_PreDecrement()
    {
        var sut = new BigDecimal(1, 3);
        var test = --sut;
        Assert.Equal(999, test);
        Assert.Equal(999, sut);
    }
    [Fact]
    [Trait("BigDecimal", "Operators")]
    public void UnaryMinusMinus_PostDecrement()
    {
        var sut = new BigDecimal(1, 3);
        var test = sut--;
        Assert.Equal(1000, test);
        Assert.Equal(999, sut);
    }
    #endregion
    #region Shift
    [Fact]
    [Trait("BigDecimal", "Operators")]
    public void ShiftLeft_Negative_Right()
    {
        var value = new BigDecimal(1, 15);
        var result = value << -6;
        var expected = new BigDecimal(1, 9);
        Assert.Equal(expected, result);
    }
    [Fact]
    [Trait("BigDecimal", "Operators")]
    public void ShiftLeft_Positive_Left()
    {
        var value = new BigDecimal(1, 15);
        var result = value << 6;
        var expected = new BigDecimal(1, 21);
        Assert.Equal(expected, result);
    }
    [Fact]
    [Trait("BigDecimal", "Operators")]
    public void ShiftRight_Negative_Left()
    {
        var value = new BigDecimal(1, 15);
        var result = value >> -6;
        var expected = new BigDecimal(1, 21);
        Assert.Equal(expected, result);
    }
    [Fact]
    [Trait("BigDecimal", "Operators")]
    public void ShiftRight_Positive_Right()
    {
        var value = new BigDecimal(1, 15);
        var result = value >> 6;
        var expected = new BigDecimal(1, 9);
        Assert.Equal(expected, result);
    }
    [Theory]
    [MemberData(nameof(ProvideShiftTestCases))]
    [Trait("BigDecimal", "Operators")]
    public void Shift_PositiveAdd_MorePositive(TestableBigDecimal value, int shift, TestableBigDecimal expected) 
        => Assert.Equal(expected.Value, value.Value.Shift(shift));

    public static TheoryData<TestableBigDecimal, int, TestableBigDecimal> ProvideShiftTestCases() => new()
    {
        { new BigDecimal(123, -1234), -700, new BigDecimal(123, -1934) },
        { new BigDecimal(123, -1234), 700, new BigDecimal(123, -534) },
        { new BigDecimal(123, 1234), -700, new BigDecimal(123, 534) },
        { new BigDecimal(123, 1234), 700, new BigDecimal(123, 1934) }

    };
    #endregion
    #region Add
    [Theory]
    [MemberData(nameof(ProvideAddTestCases))]
    [Trait("BigDecimal", "Operators")]
    public void Add(TestableBigDecimal s1, TestableBigDecimal s2, TestableBigDecimal expectedSum)
    {
        Assert.Equal(expectedSum.Value, s1.Value + s2.Value);
    }
    public static TheoryData<TestableBigDecimal, TestableBigDecimal, TestableBigDecimal> ProvideAddTestCases() => new()
    {
        { BigDecimal.Zero, BigDecimal.Zero, BigDecimal.Zero },
        { BigDecimal.One, BigDecimal.Zero, BigDecimal.One },
        { BigDecimal.Zero, BigDecimal.One, BigDecimal.One },
        { BigDecimal.NegativeOne, BigDecimal.Zero, BigDecimal.NegativeOne },
        { BigDecimal.Zero, BigDecimal.NegativeOne, BigDecimal.NegativeOne },
        { BigDecimal.One, BigDecimal.NegativeOne, BigDecimal.Zero },
        { BigDecimal.NegativeOne, BigDecimal.One, BigDecimal.Zero },
        { BigDecimal.NegativeOne, BigDecimal.NegativeOne, new BigDecimal(-2)},
        { BigDecimal.One, BigDecimal.One, new BigDecimal(2)},
        { new BigDecimal(17), new BigDecimal(-6), new BigDecimal(11)},
        { new BigDecimal(-17), new BigDecimal(6), new BigDecimal(-11)},
        { new BigDecimal(-6), new BigDecimal(17), new BigDecimal(11)},
        { new BigDecimal(6), new BigDecimal(-17), new BigDecimal(-11)},
        { new BigDecimal(1, 2), new BigDecimal(1, -2), new BigDecimal(10001, -2)}, // 100 + 0.01 = 100.01 = 10001E-2
        { new BigDecimal(123, 11), new BigDecimal(7, -6), new BigDecimal(BigInteger.Parse("12300000000000000007"), -6)}
    };
    #endregion
    #region Subtract
    [Theory]
    [MemberData(nameof(ProvideSubtractTestCases))]
    [Trait("BigDecimal", "Operators")]
    public void Subtract(TestableBigDecimal minuend, TestableBigDecimal subtrahend, TestableBigDecimal expectedDifference)
    {
        Assert.Equal(expectedDifference.Value, minuend.Value - subtrahend.Value);
    }
    public static TheoryData<TestableBigDecimal, TestableBigDecimal, TestableBigDecimal> ProvideSubtractTestCases() => new()
    {
        { BigDecimal.Zero, BigDecimal.Zero, BigDecimal.Zero },
        { BigDecimal.One, BigDecimal.Zero, BigDecimal.One },
        { BigDecimal.Zero, BigDecimal.One, BigDecimal.NegativeOne },
        { BigDecimal.NegativeOne, BigDecimal.Zero, BigDecimal.NegativeOne },
        { BigDecimal.Zero, BigDecimal.NegativeOne, BigDecimal.One },
        { BigDecimal.One, BigDecimal.NegativeOne, new BigDecimal(2)},
        { BigDecimal.NegativeOne, BigDecimal.One, new BigDecimal(-2)},
        { BigDecimal.NegativeOne, BigDecimal.NegativeOne, BigDecimal.Zero},
        { BigDecimal.One, BigDecimal.One, BigDecimal.Zero},
        { new BigDecimal(17), new BigDecimal(-6), new BigDecimal(23)},
        { new BigDecimal(-17), new BigDecimal(6), new BigDecimal(-23)},
        { new BigDecimal(-6), new BigDecimal(17), new BigDecimal(-23)},
        { new BigDecimal(6), new BigDecimal(-17), new BigDecimal(23)},
        { new BigDecimal(1, 2), new BigDecimal(1, -2), new BigDecimal(9999, -2)}, // 100 - 0.01 = 99.99 = 9999E-2
        { new BigDecimal(123, 11), new BigDecimal(7, -6), new BigDecimal(BigInteger.Parse("12299999999999999993"), -6)}
    };
    #endregion
    #region Multiply
    [Theory]
    [MemberData(nameof(ProvideMultiplyTestCases))]
    [Trait("BigDecimal", "Operators")]
    public void Multiply(TestableBigDecimal f1, TestableBigDecimal f2, TestableBigDecimal expectedProduct)
    {
        Assert.Equal(expectedProduct.Value, f1.Value * f2.Value);
    }
    public static TheoryData<TestableBigDecimal, TestableBigDecimal, TestableBigDecimal> ProvideMultiplyTestCases() => new()
    {
        { BigDecimal.Zero, BigDecimal.Zero, BigDecimal.Zero },
        { BigDecimal.One, BigDecimal.Zero, BigDecimal.Zero },
        { BigDecimal.Zero, BigDecimal.One, BigDecimal.Zero },
        { BigDecimal.NegativeOne, BigDecimal.Zero, BigDecimal.Zero},
        { BigDecimal.Zero, BigDecimal.NegativeOne, BigDecimal.Zero },
        { BigDecimal.One, BigDecimal.NegativeOne, BigDecimal.NegativeOne},
        { BigDecimal.NegativeOne, BigDecimal.One, BigDecimal.NegativeOne},
        { BigDecimal.NegativeOne, BigDecimal.NegativeOne, BigDecimal.One},
        { BigDecimal.One, BigDecimal.One, BigDecimal.One},
        { new BigDecimal(17), new BigDecimal(-6), new BigDecimal(-102)},
        { new BigDecimal(-17), new BigDecimal(6), new BigDecimal(-102)},
        { new BigDecimal(-6), new BigDecimal(17), new BigDecimal(-102)},
        { new BigDecimal(6), new BigDecimal(-17), new BigDecimal(-102)},
        { new BigDecimal(1, 2), new BigDecimal(1, -2), BigDecimal.One},
        { new BigDecimal(123, 11), new BigDecimal(7, -6), new BigDecimal(861, 5)}
    };
    #endregion
}
