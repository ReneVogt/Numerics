using Revo.Numerics;

namespace Numerics.Tests.BigDecimalTests;

public sealed partial class BigDecimalTests
{
    [Fact]
    [Trait("BigDecimal", "Properties")]
    public void One_One()
    {
        var sut = BigDecimal.One;
        Assert.Equal(1, sut.Mantissa);
        Assert.Equal(0, sut.Exponent);
        Assert.True(sut.Sign > 0);
    }
    [Fact]
    [Trait("BigDecimal", "Properties")]
    public void Zero_Zero()
    {
        var sut = BigDecimal.Zero;
        Assert.Equal(0, sut.Sign);
        Assert.Equal(0, sut.Mantissa);
        Assert.Equal(0, sut.Exponent);
    }
    [Fact]
    [Trait("BigDecimal", "Properties")]
    public void NegativeOne_NegativeOne()
    {
        var sut = BigDecimal.NegativeOne;
        Assert.True(sut.Sign < 0);
        Assert.Equal(-1, sut.Mantissa);
        Assert.Equal(0, sut.Exponent);
    }

    [Fact]
    [Trait("BigDecimal", "Properties")]
    public void Radix_10() => Assert.Equal(10, BigDecimal.Radix);
    [Fact]
    [Trait("BigDecimal", "Properties")]
    public void AdditiveIdentity_Zero() => Assert.Equal(BigDecimal.Zero, BigDecimal.AdditiveIdentity);
    [Fact]
    [Trait("BigDecimal", "Properties")]
    public void MultiplicativeIdentity_One() => Assert.Equal(BigDecimal.One, BigDecimal.MultiplicativeIdentity);

    [Fact]
    [Trait("BigDecimal", "Properties")]
    public void Sign_Negative_LessThanZero()
    {
        var sut = new BigDecimal(-123);
        Assert.True(sut.Sign < 0);
    }
    [Fact]
    [Trait("BigDecimal", "Properties")]
    public void Sign_Zero_Zero()
    {
        var sut = new BigDecimal(0);
        Assert.Equal(0, sut.Sign);
    }
    [Fact]
    [Trait("BigDecimal", "Properties")]
    public void Sign_Positive_GreaterThanZero()
    {
        var sut = new BigDecimal(123);
        Assert.True(sut.Sign > 0);
    }

    [Fact]
    [Trait("BigDecimal", "Properties")]
    public void Abs_Negative()
    {
        var sut = new BigDecimal(-12.34m);
        Assert.Equal(12.34m, BigDecimal.Abs(sut));
    }
    [Fact]
    [Trait("BigDecimal", "Properties")]
    public void Abs_Positive()
    {
        var sut = new BigDecimal(12.34m);
        Assert.Equal(12.34m, BigDecimal.Abs(sut));
    }
    [Fact]
    [Trait("BigDecimal", "Properties")]
    public void Abs_Zero() 
    {
        var sut = new BigDecimal(0);
        Assert.Equal(0, BigDecimal.Abs(sut));
    }

    [Fact]
    [Trait("BigDecimal", "Properties")]
    public void IsZero_Negative_False() => Assert.False(BigDecimal.IsZero(-12));
    [Fact]
    [Trait("BigDecimal", "Properties")]
    public void IsZero_Zero_True() => Assert.True(BigDecimal.IsZero(0));
    [Fact]
    [Trait("BigDecimal", "Properties")]
    public void IsZero_Positive_False() => Assert.False(BigDecimal.IsZero(12));

    [Fact]
    [Trait("BigDecimal", "Properties")]
    public void IsPositive_Negative_False() => Assert.False(BigDecimal.IsPositive(-12));
    [Fact]
    [Trait("BigDecimal", "Properties")]
    public void IsPositive_Zero_True() => Assert.True(BigDecimal.IsPositive(0));
    [Fact]
    [Trait("BigDecimal", "Properties")]
    public void IsPositive_Positive_True() => Assert.True(BigDecimal.IsPositive(12));

    [Fact]
    [Trait("BigDecimal", "Properties")]
    public void IsNegative_Negative_True() => Assert.True(BigDecimal.IsNegative(-12));
    [Fact]
    [Trait("BigDecimal", "Properties")]
    public void IsNegative_Zero_False() => Assert.False(BigDecimal.IsNegative(0));
    [Fact]
    [Trait("BigDecimal", "Properties")]
    public void IsNegative_Positive_False() => Assert.False(BigDecimal.IsNegative(12));

    [Fact]
    [Trait("BigDecimal", "Properties")]
    public void IsInteger_Zero_True() => Assert.True(BigDecimal.IsInteger(0));
    [Fact]
    [Trait("BigDecimal", "Properties")]
    public void IsInteger_PostiveInt_True() => Assert.True(BigDecimal.IsInteger(123));
    [Fact]
    [Trait("BigDecimal", "Properties")]
    public void IsInteger_NegativeInt_True() => Assert.True(BigDecimal.IsInteger(-123));
    [Fact]
    [Trait("BigDecimal", "Properties")]
    public void IsInteger_PostiveFraction_False() => Assert.False(BigDecimal.IsInteger(123.17m));
    [Fact]
    [Trait("BigDecimal", "Properties")]
    public void IsInteger_NegativeFraction_False() => Assert.False(BigDecimal.IsInteger(-123.17m));

    [Fact]
    [Trait("BigDecimal", "Properties")]
    public void IsEvenInteger_Zero_True() => Assert.True(BigDecimal.IsEvenInteger(0));
    [Fact]
    [Trait("BigDecimal", "Properties")]
    public void IsEvenInteger_PostiveEvenInt_True() => Assert.True(BigDecimal.IsEvenInteger(124));
    [Fact]
    [Trait("BigDecimal", "Properties")]
    public void IsEvenInteger_PostiveOddInt_False() => Assert.False(BigDecimal.IsEvenInteger(125));
    [Fact]
    [Trait("BigDecimal", "Properties")]
    public void IsEvenInteger_NegativeEvenInt_True() => Assert.True(BigDecimal.IsEvenInteger(-124));
    [Fact]
    [Trait("BigDecimal", "Properties")]
    public void IsEvenInteger_NegativeOddInt_False() => Assert.False(BigDecimal.IsEvenInteger(-125));
    [Fact]
    [Trait("BigDecimal", "Properties")]
    public void IsEvenInteger_PositiveFraction_False() => Assert.False(BigDecimal.IsEvenInteger(124.2m));
    [Fact]
    [Trait("BigDecimal", "Properties")]
    public void IsEvenInteger_NegativeFraction_False() => Assert.False(BigDecimal.IsEvenInteger(-124.2m));

    [Fact]
    [Trait("BigDecimal", "Properties")]
    public void IsOddInteger_Zero_False() => Assert.False(BigDecimal.IsOddInteger(0));
    [Fact]
    [Trait("BigDecimal", "Properties")]
    public void IsOddInteger_PostiveOddInt_True() => Assert.True(BigDecimal.IsOddInteger(123));
    [Fact]
    [Trait("BigDecimal", "Properties")]
    public void IsOddInteger_PostiveEventInt_False() => Assert.False(BigDecimal.IsOddInteger(124));
    [Fact]
    [Trait("BigDecimal", "Properties")]
    public void IsOddInteger_NegativeEvenInt_False() => Assert.False(BigDecimal.IsOddInteger(-124));
    [Fact]
    [Trait("BigDecimal", "Properties")]
    public void IsOddInteger_NegativeOddInt_True() => Assert.True(BigDecimal.IsOddInteger(-125));
    [Fact]
    [Trait("BigDecimal", "Properties")]
    public void IsOddInteger_PositiveFraction_False() => Assert.False(BigDecimal.IsOddInteger(124.2m));
    [Fact]
    [Trait("BigDecimal", "Properties")]
    public void IsOddInteger_NegativeFraction_False() => Assert.False(BigDecimal.IsOddInteger(-124.2m));

    [Theory]
    [MemberData(nameof(ProvideSimplePropertyTestData))]
    [Trait("BigDecimal", "Properties")]
    public void IsCanonical_True(TestableBigDecimal value) => Assert.True(BigDecimal.IsCanonical(value.Value));
    [Theory]
    [MemberData(nameof(ProvideSimplePropertyTestData))]
    [Trait("BigDecimal", "Properties")]
    public void IsFinite_True(TestableBigDecimal value) => Assert.True(BigDecimal.IsFinite(value.Value));
    [Theory]
    [MemberData(nameof(ProvideSimplePropertyTestData))]
    [Trait("BigDecimal", "Properties")]
    public void IsNaN_False(TestableBigDecimal value) => Assert.False(BigDecimal.IsNaN(value.Value));
    [Theory]
    [MemberData(nameof(ProvideSimplePropertyTestData))]
    [Trait("BigDecimal", "Properties")]
    public void IsComplexNumber_False(TestableBigDecimal value) => Assert.False(BigDecimal.IsComplexNumber(value.Value));
    [Theory]
    [MemberData(nameof(ProvideSimplePropertyTestData))]
    [Trait("BigDecimal", "Properties")]
    public void IsRealNumber_True(TestableBigDecimal value) => Assert.True(BigDecimal.IsRealNumber(value.Value));
    [Theory]
    [MemberData(nameof(ProvideSimplePropertyTestData))]
    [Trait("BigDecimal", "Properties")]
    public void IsImaginaryNumber_False(TestableBigDecimal value) => Assert.False(BigDecimal.IsImaginaryNumber(value.Value));
    [Theory]
    [MemberData(nameof(ProvideSimplePropertyTestData))]
    [Trait("BigDecimal", "Properties")]
    public void IsInfinity_False(TestableBigDecimal value) => Assert.False(BigDecimal.IsInfinity(value.Value));
    [Theory]
    [MemberData(nameof(ProvideSimplePropertyTestData))]
    [Trait("BigDecimal", "Properties")]
    public void IsPositivieInfinity_False(TestableBigDecimal value) => Assert.False(BigDecimal.IsPositiveInfinity(value.Value));
    [Theory]
    [MemberData(nameof(ProvideSimplePropertyTestData))]
    [Trait("BigDecimal", "Properties")]
    public void IsNegativeInfinity_False(TestableBigDecimal value) => Assert.False(BigDecimal.IsNegativeInfinity(value.Value));
    [Theory]
    [MemberData(nameof(ProvideSimplePropertyTestData))]
    [Trait("BigDecimal", "Properties")]
    public void IsNormal_True(TestableBigDecimal value) => Assert.True(BigDecimal.IsNormal(value.Value));
    [Theory]
    [MemberData(nameof(ProvideSimplePropertyTestData))]
    [Trait("BigDecimal", "Properties")]
    public void IsSubnormal_False(TestableBigDecimal value) => Assert.False(BigDecimal.IsSubnormal(value.Value));
    public static TheoryData<TestableBigDecimal> ProvideSimplePropertyTestData() =>
    [
        new BigDecimal(0),
        new BigDecimal(12000),
        new BigDecimal(-12000),
        new BigDecimal(0.00000012m),
        new BigDecimal(-0.000014m)
    ];

    [Theory]
    [MemberData(nameof(ProvideMagnitudeTestData))]
    [Trait("BigDecimal", "Properties")]
    public void MaxMagnitude(TestableBigDecimal lesser, TestableBigDecimal greater)
    {
        Assert.Equal(greater.Value, BigDecimal.MaxMagnitude(lesser.Value, greater.Value));
        Assert.Equal(greater.Value, BigDecimal.MaxMagnitude(greater.Value, lesser.Value));

        Assert.Equal(greater.Value, BigDecimal.MaxMagnitudeNumber(lesser.Value, greater.Value));
        Assert.Equal(greater.Value, BigDecimal.MaxMagnitudeNumber(greater.Value, lesser.Value));

        Assert.Equal(lesser.Value, BigDecimal.MinMagnitude(lesser.Value, greater.Value));
        Assert.Equal(lesser.Value, BigDecimal.MinMagnitude(greater.Value, lesser.Value));

        Assert.Equal(lesser.Value, BigDecimal.MinMagnitudeNumber(lesser.Value, greater.Value));
        Assert.Equal(lesser.Value, BigDecimal.MinMagnitudeNumber(greater.Value, lesser.Value));
    }
    public static TheoryData<TestableBigDecimal, TestableBigDecimal> ProvideMagnitudeTestData() => new() {
        {new BigDecimal(0), new BigDecimal(1)},
        {new BigDecimal(-1), new BigDecimal(1.2m)},
        { new BigDecimal(10), new BigDecimal(-20)},
        { new BigDecimal(-0.5m), new BigDecimal(-0.7m)},
    };
}
