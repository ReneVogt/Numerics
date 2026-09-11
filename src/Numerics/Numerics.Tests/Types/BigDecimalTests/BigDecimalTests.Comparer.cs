using Revo.Numerics;
using System.Globalization;
using System.Numerics;

namespace Numerics.Tests.BigDecimalTests;

public sealed partial class BigDecimalTests
{
    [Theory]
    [MemberData(nameof(ProvideCompareTestCases))]
    [Trait("BigDecimal", "Compare")]
    public void CompareGreaterLesser(TestableBigDecimal greater, TestableBigDecimal lesser)
    {
        var g = greater.Value;
        var l = lesser.Value;

        Assert.True(g > l);
        Assert.True(g >= l);
        Assert.False(g == l);
        Assert.True(g != l);
        Assert.False(g < l);
        Assert.False(g <= l);

        Assert.False(l > g);
        Assert.False(l >= g);
        Assert.False(l == g);
        Assert.True(l != g);
        Assert.True(l < g);
        Assert.True(l <= g);
    }
    [Theory]
    [MemberData(nameof(ProvideCompareTestCases))]
    [Trait("BigDecimal", "Compare")]
    public void CompareGreaterLesserByObject(TestableBigDecimal greater, TestableBigDecimal lesser)
    {
        var g = greater.Value;
        var l = lesser.Value;
        Assert.Equal(1, g.CompareTo((object)l));
        Assert.Equal(-1, l.CompareTo((object)g));
    }
    public static TheoryData<TestableBigDecimal, TestableBigDecimal> ProvideCompareTestCases() => new()
    {
        {new BigDecimal(decimal.MaxValue), new BigDecimal(decimal.MinValue)},
        {new BigDecimal(decimal.MaxValue), new BigDecimal(1, -28)},
        {new BigDecimal(1, -28), new BigDecimal(decimal.MinValue)},
        {new BigDecimal(123m), new BigDecimal(122m)},
        {new BigDecimal(decimal.MaxValue), new BigDecimal(BigInteger.Parse("123000111222333444555666777888999"), -30)},
        {BigDecimal.Zero, BigDecimal.NegativeOne },
        {BigDecimal.One, BigDecimal.Zero }
    };

    [Fact]
    [Trait("BigDecimal", "Compare")]
    public void Compare_Null_1() => Assert.Equal(1, BigDecimal.Zero.CompareTo(null));
    [Fact]
    [Trait("BigDecimal", "Compare")]
    public void Compare_NoBigDecimal_Throws() => Assert.Throws<ArgumentException>(() => BigDecimal.Zero.CompareTo(new object()));

    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("-1")]
    [InlineData("123456789012345")]
    [InlineData("-123456789012345")]
    [InlineData("1.02345678910")]
    [InlineData("-1.02345678910")]
    [InlineData("0.0000000000002345678910")]
    [InlineData("-0.0000000000002345678910")]
    [Trait("BigDecimal", "Compare")]
    public void Compare_Equal(string value)
    {
        var sut = new BigDecimal(decimal.Parse(value, CultureInfo.InvariantCulture));
        Assert.Equal(sut, sut);
        Assert.True(sut.Equals((object)sut));
        Assert.Equal(0, sut.CompareTo(sut));
        Assert.Equal(0, sut.CompareTo((object)sut));
    }
    [Fact]
    [Trait("BigDecimal", "Compare")]
    public void Compare_Equal_NoBigDecimal_False()
    {
        Assert.False(BigDecimal.One.Equals(null));
    }
}
