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
    #region Divide
    [Theory]
    [MemberData(nameof(ProvideDivisionByZeroTestCases))]
    [Trait("BigDecimal", "Operators")]
    public void Division_ByZero_Throws(TestableBigDecimal divident)
    {
        Assert.Throws<DivideByZeroException>(() => divident.Value / BigDecimal.Zero);
        Assert.Throws<DivideByZeroException>(() => divident.Value.DivideBy(BigDecimal.Zero));
        Assert.Throws<DivideByZeroException>(() => divident.Value.DivideBy(BigDecimal.Zero, 10));
        Assert.Throws<DivideByZeroException>(() => BigDecimal.DivRem(divident.Value, BigDecimal.Zero));
        Assert.Throws<DivideByZeroException>(() => BigDecimal.DivRem(divident.Value, BigDecimal.Zero, 10));
        Assert.Throws<DivideByZeroException>(() => divident.Value % BigDecimal.Zero);
        Assert.Throws<DivideByZeroException>(() => divident.Value.ModulusBy(BigDecimal.Zero));
    }
    [Theory]
    [MemberData(nameof(ProvideDivRemTestCases))]
    [Trait("BigDecimal", "Operators")]
    public void DivRem_Precision(TestableBigDecimal divident, TestableBigDecimal divisor, TestableBigDecimal quotient, TestableBigDecimal remainder, int precision)
    {
        var (q, m) = BigDecimal.DivRem(divident.Value, divisor.Value, precision);
        Assert.Equal(quotient.Value, q);
        Assert.Equal(remainder.Value, m);
    }
    [Theory]
    [MemberData(nameof(ProvideDivRemTestCases))]
    [Trait("BigDecimal", "Operators")]
    public void DivRem_GlobalPrecision(TestableBigDecimal divident, TestableBigDecimal divisor, TestableBigDecimal quotient, TestableBigDecimal remainder, int precision)
    {
        BigDecimalContext.Precision = precision;
        var (q, m) = BigDecimal.DivRem(divident.Value, divisor.Value);
        Assert.Equal(quotient.Value, q);
        Assert.Equal(remainder.Value, m);
    }
    [Theory]
    [MemberData(nameof(ProvideDivisionTestCases))]
    [Trait("BigDecimal", "Operators")]
    public void Division_Precision(TestableBigDecimal divident, TestableBigDecimal divisor, TestableBigDecimal quotient, int precision)
        => Assert.Equal(quotient.Value, divident.Value.DivideBy(divisor.Value, precision));
    [Theory]
    [MemberData(nameof(ProvideDivisionTestCases))]
    [Trait("BigDecimal", "Operators")]
    public void Division_GlobalPrecision(TestableBigDecimal divident, TestableBigDecimal divisor, TestableBigDecimal quotient, int precision)
    {
        BigDecimalContext.Precision = precision;
        Assert.Equal(quotient.Value, divident.Value / divisor.Value);
        Assert.Equal(quotient.Value, divident.Value.DivideBy(divisor.Value));
    }
    [Theory]
    [MemberData(nameof(ProvideModulusTestCases))]
    [Trait("BigDecimal", "Operators")]
    public void Modulus_GlobalPrecision(TestableBigDecimal divident, TestableBigDecimal divisor, TestableBigDecimal remainder)
    {
        Assert.Equal(remainder.Value, divident.Value % divisor.Value);
        Assert.Equal(remainder.Value, divident.Value.ModulusBy(divisor.Value));
    }
    public static TheoryData<TestableBigDecimal> ProvideDivisionByZeroTestCases() => [.. ProvideDivRemTestCases().Select(n => (TestableBigDecimal)n[0])];
    public static TheoryData<TestableBigDecimal, TestableBigDecimal, TestableBigDecimal, int> ProvideDivisionTestCases()
    {
        var data = new TheoryData<TestableBigDecimal, TestableBigDecimal, TestableBigDecimal, int>();
        foreach (var row in ProvideDivRemTestCases())
            data.Add((TestableBigDecimal)row[0], (TestableBigDecimal)row[1], (TestableBigDecimal)row[2], (int)row[4]);
        return data;
    }
    public static TheoryData<TestableBigDecimal, TestableBigDecimal, TestableBigDecimal> ProvideModulusTestCases()
    {
        var data = new TheoryData<TestableBigDecimal, TestableBigDecimal, TestableBigDecimal>();
        foreach (var row in ProvideDivRemTestCases())
            data.Add((TestableBigDecimal)row[0], (TestableBigDecimal)row[1], (TestableBigDecimal)row[3]);
        return data;
    }
    public static TheoryData<TestableBigDecimal, TestableBigDecimal, TestableBigDecimal, TestableBigDecimal, int> ProvideDivRemTestCases() => new()
    {
        { BigDecimal.Zero, BigDecimal.One, BigDecimal.Zero, BigDecimal.Zero, 10},
        { BigDecimal.Zero, BigDecimal.NegativeOne, BigDecimal.Zero, BigDecimal.Zero, 10 },
        { BigDecimal.One, BigDecimal.One, BigDecimal.One, BigDecimal.Zero, 10 },
        { BigDecimal.One, BigDecimal.NegativeOne, BigDecimal.NegativeOne, BigDecimal.Zero, 10 },
        { BigDecimal.NegativeOne, BigDecimal.One, BigDecimal.NegativeOne, BigDecimal.Zero, 10 },
        { BigDecimal.NegativeOne, BigDecimal.NegativeOne, BigDecimal.One, BigDecimal.Zero, 10 },
        { BigDecimal.One, new BigDecimal(2), new BigDecimal(5, -1), BigDecimal.One, 10 },

        { new BigDecimal(7, 3), new BigDecimal(3), new BigDecimal(2333333, -3), BigDecimal.One, 3 },
        { new BigDecimal(3, -2), new BigDecimal(2, -3), new BigDecimal(15), BigDecimal.Zero, 3 },
        { new BigDecimal(-7, -1), new BigDecimal(3), new BigDecimal(-233, -3), new BigDecimal(-7, -1), 3 }
    };
    #endregion
}
