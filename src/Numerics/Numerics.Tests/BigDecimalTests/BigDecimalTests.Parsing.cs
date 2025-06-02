using Revo.Numerics;
using System.Globalization;

namespace Numerics.Tests.BigDecimalTests;

public sealed partial class BigDecimalTests
{
    [Fact]
    [Trait("BigDecimal", "Parsing")]
    public void Parse_Null_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => BigDecimal.Parse(null!));
        Assert.Throws<ArgumentNullException>(() => BigDecimal.Parse(null!, CultureInfo.InvariantCulture));
        Assert.Throws<ArgumentNullException>(() => BigDecimal.Parse(null!, NumberStyles.Integer, CultureInfo.InvariantCulture));
    }
    [Fact]
    [Trait("BigDecimal", "Parsing")]
    public void TryParse_Null_False()
    {
        Assert.False(BigDecimal.TryParse(null, out _));
        Assert.False(BigDecimal.TryParse(null, CultureInfo.InvariantCulture, out _));
        Assert.False(BigDecimal.TryParse(null, NumberStyles.Integer, CultureInfo.InvariantCulture, out _));
    }
    [Theory]
    [MemberData(nameof(ProvideValidParsingTestCases))]
    [Trait("BigDecimal", "Parsing")]
    public void Parsing_Valid(TestableBigDecimal expected, string s)
    {
        Assert.Equal(expected.Value, BigDecimal.Parse(s));
        Assert.Equal(expected.Value, BigDecimal.Parse(s, CultureInfo.InvariantCulture));
        Assert.Equal(expected.Value, BigDecimal.Parse(s, NumberStyles.Integer, CultureInfo.InvariantCulture));
        Assert.Equal(expected.Value, BigDecimal.Parse(s.AsSpan(), CultureInfo.InvariantCulture));
        Assert.Equal(expected.Value, BigDecimal.Parse(s.AsSpan(), NumberStyles.Integer, CultureInfo.InvariantCulture));

        Assert.True(BigDecimal.TryParse(s, out var result));
        Assert.Equal(expected.Value, result);
        Assert.True(BigDecimal.TryParse(s, CultureInfo.InvariantCulture, out result));
        Assert.Equal(expected.Value, result);
        Assert.True(BigDecimal.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out result));
        Assert.Equal(expected.Value, result);
        Assert.True(BigDecimal.TryParse(s.AsSpan(), CultureInfo.InvariantCulture, out result));
        Assert.Equal(expected.Value, result);
        Assert.True(BigDecimal.TryParse(s.AsSpan(), NumberStyles.Integer, CultureInfo.InvariantCulture, out result));
        Assert.Equal(expected.Value, result);
    }
    [Theory]
    [MemberData(nameof(ProvideInvalidParsingTestCases))]
    [Trait("BigDecimal", "Parsing")]
    public void Parsing_Invalid(string s) => Assert.Throws<FormatException>(() => BigDecimal.Parse(s));
    [Theory]
    [MemberData(nameof(ProvideRoundTripTestCases))]
    [Trait("BigDecimal", "Parsing")]
    public void Parsing_RoundTrip(TestableBigDecimal value) => Assert.Equal(value.Value, BigDecimal.Parse(value.Value.ToString()));
    public static TheoryData<TestableBigDecimal, string> ProvideValidParsingTestCases() => new()
    {
        { BigDecimal.Zero, "0" },
        { BigDecimal.NegativeOne, "-1" },
        { BigDecimal.One, "1" },
        { new BigDecimal(213, 7123), "213 E+7123" },
        { new BigDecimal(213, 7123), "213 E7123" },
        { new BigDecimal(213, 7123), "213 E7123" },
        { new BigDecimal(213, -7123), "213 E-7123" },
        { new BigDecimal(-213, 7123), "-213 E7123" },
        { new BigDecimal(-213, -7123), "-213 E-7123" }
    };
    public static TheoryData<string> ProvideInvalidParsingTestCases() =>
    [
        "1E3",
        "1E-3",
        "E1",
        "xE-2",
        "17Es",
        "nö"
    ];
    public static TheoryData<TestableBigDecimal> ProvideRoundTripTestCases() =>
    [
        BigDecimal.Zero,
        BigDecimal.One,
        new BigDecimal(-1m),
        new BigDecimal(999, 128),
        new BigDecimal(12, -34),
        new BigDecimal(-78912, 12334),
        new BigDecimal(-12000, -34),
        new BigDecimal(10000000, 30)
    ];   
}
